﻿using Carbunql.Analysis;
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
dat(line_id, name, unit_price, quantity, tax_rate) as ( 
    values
    (1, 'apple' , 105, 5, 0.07),
    (2, 'orange', 203, 3, 0.07),
    (3, 'banana', 233, 9, 0.07),
    (4, 'tea'   , 309, 7, 0.08),
    (5, 'coffee', 555, 9, 0.08),
    (6, 'cola'  , 456, 2, 0.08)
)
select line_id, name, unit_price, quantity, tax_rate from dat";

        var builder = new FractionAdjustmentQueryBuilder("line_id", "unit_price", "quantity", "tax_rate");
        var sq = builder.Execute(sql, "price", "tax");

        Monitor.Log(sq);

        var lst = sq.GetTokens().ToList();

        Assert.Equal(346, lst.Count());
    }
}

/// <summary>
/// Calculate tax rate.
/// The tax amount is calculated for each tax rate, and any fractions are rounded down.
/// Also find the tax amount for each line item.
/// The line item tax amount is also rounded down, but adjusted to match the total tax amount.
/// The order of priority for adjustment is "in descending order of fractions".
/// </summary>
public class FractionAdjustmentQueryBuilder
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sortColumn">
    /// A column that is unique when sorted.
    /// It is necessary to specify so that the fractional apportionment does not fluctuate.
    /// </param>
    /// <param name="unitPriceColumn">
    /// unit price column
    /// </param>
    /// <param name="quantityColumn">
    /// quantity column
    /// </param>
    /// <param name="taxRateColumn">
    /// Tax rate column (e.g. 0.05)
    /// </param>
    public FractionAdjustmentQueryBuilder(string sortColumn, string unitPriceColumn, string quantityColumn, string taxRateColumn)
    {
        SortColumn = sortColumn;
        UnitPriceColumn = unitPriceColumn;
        QuantitytColumn = quantityColumn;
        TaxRateColumn = taxRateColumn;
    }

    public string SortColumn { get; init; }
    public string UnitPriceColumn { get; init; }
    public string QuantitytColumn { get; init; }
    public string TaxRateColumn { get; init; }
    private string DatasourceTable { get; set; } = "_datasource";
    private string TaxSummaryTable { get; set; } = "_summary";
    private string NonTaxPriceColumn { get; set; } = "_non_tax_price";
    private string RawTaxColumn { get; set; } = "_raw_tax";
    private string TotalTaxColumn { get; set; } = "_total_tax";
    private string CumulativeColumn { get; set; } = "_cumulative";
    private string PriorityColumn { get; set; } = "_priority";
    private string AdjustTaxColumn { get; set; } = "_adjust_tax";

    public CTEQuery Execute(string sql, string priceColumn, string taxColumn)
    {
        var columns = new List<string>();
        var createCTE = () =>
        {
            var q = QueryParser.Parse(sql);
            var sq = q.GetSelectQuery();
            columns = sq!.SelectClause!.Select(x => x.Alias).ToList();

            q = GenerateCalcPriceQuery(q);
            q = GenerateCalcTaxQuery(q, taxColumn);
            return q.ToCTE(DatasourceTable);
        };

        var createQuery = () =>
        {
            var q = GenerateDetailQuery(taxColumn);
            q = GenerateCalcAdjustTaxQuery(q);
            q = GenerateCalcTaxQuery(q, columns, priceColumn, taxColumn);
            return q;
        };

        var cte = createCTE();
        cte.WithClause.Add(GenerateTaxSummaryQuery().ToCommonTable(TaxSummaryTable));
        cte.Query = createQuery();

        return cte;
    }

    private QueryBase GenerateCalcPriceQuery(QueryBase query)
    {
        /*
        select
            d.*,
            d.unit_price * d.amount as non_tax_price
        from
            (...) as d
        */
        var (q, d) = query.ToSubQuery("d");
        var sq = q.GetSelectQuery();
        sq.SelectAll(d);
        // dat.unit_price * dat.amount as price
        sq.Select(() =>
        {
            ValueBase v = new ColumnValue(d, UnitPriceColumn);
            v = v.Expression("*", new ColumnValue(d, QuantitytColumn));
            return v;
        }).As(NonTaxPriceColumn);

        return q;
    }

    private QueryBase GenerateCalcTaxQuery(QueryBase query, string taxColumn)
    {

        /*
        select  
            d.*,
            trunc(d.non_tax_price * (1 + d.tax_rate)) - d.non_tax_price as tax,
                    (d.non_tax_price * (1 + d.tax_rate)) - d.non_tax_price as raw_tax
        from
            (...) d
        */

        var (q, d) = query.ToSubQuery("d");
        var sq = q.GetSelectQuery();
        sq.SelectAll(d);

        ValueBase exp = new ColumnValue(d, NonTaxPriceColumn);
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
            v = v.Expression("-", new ColumnValue(d, NonTaxPriceColumn));
            return v;
        }).As(taxColumn);

        //(d.price * (1 + d.tax_rate)) - d.price as raw_tax
        sq.Select(() =>
        {
            ValueBase v = exp.ToGroup();
            v = v.Expression("-", new ColumnValue(d, NonTaxPriceColumn));
            return v;
        }).As(RawTaxColumn);

        return q;
    }

    private QueryBase GenerateTaxSummaryQuery()
    {
        /*
        select
            d.tax_rate,
            trunc(sum(raw_tax)) as total_tax
        from
            dataource d
        group by
            d.tax_rate
        */
        var sq = new SelectQuery();
        var d = sq.From(DatasourceTable).As("d");

        // d.tax_rate,
        var groupkey = sq.Select(d, TaxRateColumn);

        // trunc(sum(raw_tax)) as total_tax
        sq.Select(() =>
        {
            return new FunctionValue("trunc", () =>
            {
                return new FunctionValue("sum", new ColumnValue(d, RawTaxColumn));
            });
        }).As(TotalTaxColumn);

        // group by d.tax_rate
        sq.Group(groupkey);

        return sq;
    }

    private QueryBase GenerateDetailQuery(string taxColumn)
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
        var sq = new SelectQuery();
        var d = sq.From(DatasourceTable).As("d");
        var s = d.InnerJoin(TaxSummaryTable).As("s").On(d, TaxRateColumn);

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

    private QueryBase GenerateCalcAdjustTaxQuery(QueryBase detailQuery)
    {
        /*
        select
            d.*,
            case when d.total_tax - d.cumulative >= d.priority then 1 else 0 end as adjust_tax
        from
            (...) as d
        */

        var (q, d) = detailQuery.ToSubQuery("d");
        var sq = q.GetSelectQuery();

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

        return q;
    }

    private QueryBase GenerateCalcTaxQuery(QueryBase query, List<string> columns, string priceColumn, string taxColumn)
    {
        /*
        select
            line_id,
            name,
            unit_price,
            amount,
            tax_rate,
            non_tax_price + tax + adjust_tax as price,
            tax + adjust_tax as tax
        from
            (...) as d
        */
        var (q, d) = query.ToSubQuery("d");
        var sq = q.GetSelectQuery();

        // line_id, ..., price,
        columns.ForEach(x => sq.Select(d, x));

        ValueBase tax = new ColumnValue(d, taxColumn);
        tax = tax.Expression("+", new ColumnValue(d, AdjustTaxColumn));

        // non_tax_price + tax + adjust_tax as price
        sq.Select(() =>
        {
            ValueBase v = new ColumnValue(d, NonTaxPriceColumn);
            v = v.Expression("+", tax);
            return v;
        }).As(priceColumn);

        // tax + adjust_tax as tax
        sq.Select(tax).As(taxColumn);

        return q;
    }
}