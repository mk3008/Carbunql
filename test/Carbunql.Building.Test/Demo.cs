using Carbunql.Analysis;
using Carbunql.Clauses;
using Carbunql.Values;
using Xunit.Abstractions;

namespace Carbunql.Building.Test;

public class Demo
{
    private readonly QueryCommandMonitor Monitor;

    public Demo(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void FractionAdjustment_Function()
    {
        var sql = @"
with
dat(line_id, name, unit_price, amount, tax_rate) as ( 
    values
    (1, 'apple' , 105, 5, 0.07),
    (2, 'orange', 203, 3, 0.07),
    (3, 'banana', 233, 9, 0.07),
    (4, 'tea'   , 309, 7, 0.08),
    (5, 'coffee', 555, 9, 0.08),
    (6, 'cola'  , 456, 2, 0.08)
)
select line_id, name, unit_price, amount, tax_rate from dat";

        var builder = new FractionAdjustmentQueryBuilder("line_id", "unit_price", "amount", "tax_rate");
        var sq = builder.Execute(sql, "price", "tax");

        Monitor.Log(sq);

        var lst = sq.GetTokens().ToList();

        Assert.Equal(346, lst.Count());
    }
}

public class FractionAdjustmentQueryBuilder
{
    public FractionAdjustmentQueryBuilder(string sortColumn, string unitPriceColumn, string amountColumn, string taxRateColumn)
    {
        SortColumn = sortColumn;
        UnitPriceColumn = unitPriceColumn;
        AmountColumn = amountColumn;
        TaxRateColumn = taxRateColumn;
    }

    public string SortColumn { get; init; }
    public string UnitPriceColumn { get; init; }
    public string AmountColumn { get; init; }
    public string TaxRateColumn { get; init; }
    private string DatasourceTable { get; set; } = "_datasource";
    private string SummaryTable { get; set; } = "_summary";
    private string RawTaxColumn { get; set; } = "_raw_tax";
    private string TotalTaxColumn { get; set; } = "_total_tax";
    private string CumulativeColumn { get; set; } = "_cumulative";
    private string PriorityColumn { get; set; } = "_priority";
    private string AdjustTaxColumn { get; set; } = "_adjust_tax";

    public SelectQuery Execute(string sql, string priceColumn, string taxColumn)
    {
        var sq = QueryParser.Parse(sql) as SelectQuery;
        var columns = sq!.SelectClause!.Select(x => x.Alias).ToList();

        sq = GenerateCalcPriceQuery(sq, priceColumn);
        sq = AddDatasourceCTE(sq, priceColumn, taxColumn);
        sq = AddSummaryCTE(sq);
        sq = GenerateDetailQuery(sq, taxColumn);
        sq = GenerateCalcAdjustTaxQuery(sq);
        sq = GenerateCalcTaxQuery(sq, columns, priceColumn, taxColumn);

        return sq;
    }

    private SelectQuery GenerateCalcPriceQuery(SelectQuery query, string priceColumn)
    {
        /*
        select
            d.*,
            d.unit_price * d.amount as price
        from
            (...) as d
        */
        var (sq, d) = query.ToSubQuery("d");
        sq.SelectAll(d);
        // dat.unit_price * dat.amount as price
        sq.Select(() =>
        {
            ValueBase v = new ColumnValue(d, UnitPriceColumn);
            v = v.Expression("*", new ColumnValue(d, AmountColumn));
            return v;
        }).As(priceColumn);

        return sq;
    }

    private SelectQuery AddDatasourceCTE(SelectQuery query, string priceColumn, string taxColumn)
    {

        /*
        with
        datasource as (
            select  
                d.*,
                trunc(d.price * (1 + d.tax_rate)) - d.price as tax,
                     (d.price * (1 + d.tax_rate)) - d.price as raw_tax
            from
                (...) d
        )
        */

        var (sq, d) = query.ToSubQuery("d");
        sq.SelectAll(d);

        ValueBase exp = new ColumnValue(d, priceColumn);
        exp = exp.Expression("*", () =>
        {
            ValueBase y = new LiteralValue("1");
            y = y.Expression("+", new ColumnValue(d, TaxRateColumn));
            return y.ToGroup();
        });

        //trunc(d.price * (1 + d.tax_rate)) - d.price as tax
        sq.Select(() =>
        {
            ValueBase v = new FunctionValue("trunc", exp);
            v = v.Expression("-", new ColumnValue(d, priceColumn));
            return v;
        }).As(taxColumn);

        //(d.price * (1 + d.tax_rate)) - d.price as raw_tax
        sq.Select(() =>
        {
            ValueBase v = exp.ToGroup();
            v = v.Expression("-", new ColumnValue(d, priceColumn));
            return v;
        }).As(RawTaxColumn);

        return sq.ToCTE(DatasourceTable);
    }

    private SelectQuery AddSummaryCTE(SelectQuery query)
    {
        /*
        with
        datasource as (...),
        summary as (
            select
                d.tax_rate,
                trunc(sum(raw_tax)) as total_tax
            from
                dataource d
            group by
                d.tax_rate
        )
        */
        var d = query.From(DatasourceTable).As("d");

        // d.tax_rate,
        var groupkey = query.Select(d, TaxRateColumn);

        // trunc(sum(raw_tax)) as total_tax
        query.Select(() =>
        {
            return new FunctionValue("trunc", () =>
            {
                return new FunctionValue("sum", new ColumnValue(d, RawTaxColumn));
            });
        }).As(TotalTaxColumn);

        // group by d.tax_rate
        query.Group(groupkey);

        return query.ToCTE(SummaryTable);
    }

    private SelectQuery GenerateDetailQuery(SelectQuery sq, string taxColumn)
    {
        /*
        select  
            d.*, 
            s.total_tax,
            sum(d.tax) over (partition by d.tax_rate) as cumulative,
            row_number() over (partition by d.tax_rate order by d.raw_tax % 1 desc, d.line_id) as priority
        from
            detail d
            inner join tax_summary s on d.tax_rate = s.tax_rate
        */
        var d = sq.From(DatasourceTable).As("d");
        var s = d.InnerJoin(SummaryTable).As("s").On(d, TaxRateColumn);

        sq.SelectAll(d);
        sq.Select(s, TotalTaxColumn);
        sq.Select(() =>
        {
            return new FunctionValue("sum", new ColumnValue(d, taxColumn), () =>
            {
                var wf = new WindowFunction();
                wf.AddPartition(new ColumnValue(d, TaxRateColumn));
                return wf;
            });
        }).As(CumulativeColumn);
        sq.Select(() =>
        {
            return new FunctionValue("row_number", () =>
            {
                var wf = new WindowFunction();
                wf.AddPartition(new ColumnValue(d, TaxRateColumn));
                wf.AddOrder(() =>
                {
                    ValueBase v = new ColumnValue(d, RawTaxColumn);
                    v = v.Expression("%", new LiteralValue("1"));
                    return v.ToSortable(isAscending: false);
                });
                wf.AddOrder(new ColumnValue(d, SortColumn).ToSortable());
                return wf;
            });
        }).As(PriorityColumn);
        return sq;
    }

    private SelectQuery GenerateCalcAdjustTaxQuery(SelectQuery query)
    {
        /*
        select
            d.*,
            case when d.total_tax - d.cumulative >= d.priority then 1 else 0 end as adjust_tax
        from
            (...) as d
        */

        var (sq, d) = query.ToSubQuery("d");

        // d.*,
        sq.SelectAll(d);

        // case
        //    when d.total_tax - d.cumulative >= d.priority then 1
        //    else 0
        // end as adjust_tax
        sq.Select(() =>
        {
            var exp = new CaseExpression();
            exp.When(() =>
            {
                ValueBase v = new ColumnValue(d, TotalTaxColumn);
                v = v.Expression("-", new ColumnValue(d, CumulativeColumn));
                v = v.Expression(">=", new ColumnValue(d, PriorityColumn));
                return v;
            }).Then(new LiteralValue("1"));
            exp.Else(new LiteralValue("0"));
            return exp;
        }).As(AdjustTaxColumn);

        return sq;
    }

    private SelectQuery GenerateCalcTaxQuery(SelectQuery query, List<string> columns, string priceColumn, string taxColumn)
    {
        /*
        select
            line_id,
            name,
            unit_price,
            amount,
            tax_rate,
            price + tax + adjust_tax as price,
            tax + adjust_tax as tax
        from
            (...) as d
        */
        var (sq, d) = query.ToSubQuery("d");

        // line_id, ..., price,
        columns.ForEach(x => sq.Select(d, x));

        ValueBase tax = new ColumnValue(d, taxColumn);
        tax = tax.Expression("+", new ColumnValue(d, AdjustTaxColumn));

        // price + tax + adjust_tax as price
        sq.Select(() =>
        {
            ValueBase v = new ColumnValue(d, priceColumn);
            v = v.Expression("+", tax);
            return v;
        }).As(priceColumn);

        // tax + adjust_tax as tax
        sq.Select(tax).As(taxColumn);

        return sq;
    }
}
