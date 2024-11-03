using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Carbunql;
using Carbunql.Analysis;
using Carbunql.Building;
using Carbunql.Extensions;

class Program
{
    static void Main(string[] args) => BenchmarkRunner.Run<Test>();
}

public class Test
{
    public Test()
    {
        SqModelQuery = GetSqModelQuery();
        CarbunqlQuery = GetCarbunqlQuery();
    }

    public static string Sql = @"with
dat(line_id, name, unit_price, amount, tax_rate) as ( 
    values
    (1, 'apple' , 105, 5, 0.07),
    (2, 'orange', 203, 3, 0.07),
    (3, 'banana', 233, 9, 0.07),
    (4, 'tea'   , 309, 7, 0.08),
    (5, 'coffee', 555, 9, 0.08),
    (6, 'cola'  , 456, 2, 0.08)
),
detail as (
    select  
        q.*,
        trunc(q.price * (1 + q.tax_rate)) - q.price as tax,
        q.price * (1 + q.tax_rate) - q.price as raw_tax
    from
        (
            select
                dat.*,
                (dat.unit_price * dat.amount) as price
            from
                dat
        ) q
), 
tax_summary as (
    select
        d.tax_rate,
        trunc(sum(raw_tax)) as total_tax
    from
        detail d
    group by
        d.tax_rate
)
select 
   line_id,
    name,
    unit_price,
    amount,
    tax_rate,
    price,
    price + tax as tax_included_price,
    tax
from
    (
        select
            line_id,
            name,
            unit_price,
            amount,
            tax_rate,
            price,
            tax + adjust_tax as tax
        from
            (
                select
                    q.*,
                    case when q.total_tax - q.cumulative >= q.priority then 1 else 0 end as adjust_tax
                from
                    (
                        select  
                            d.*, 
                            s.total_tax,
                            sum(d.tax) over (partition by d.tax_rate) as cumulative,
                            row_number() over (partition by d.tax_rate order by d.raw_tax % 1 desc, d.line_id) as priority
                        from
                            detail d
                            inner join tax_summary s on d.tax_rate = s.tax_rate
                    ) q
            ) q
    ) q
order by 
    line_id";

    private SqModel.SelectQuery SqModelQuery { get; init; }

    private SqModel.SelectQuery GetSqModelQuery()
    {
        var sq = SqModel.Analysis.SqlParser.Parse(Sql);
        sq.Parameters = new();
        for (int i = 0; i < 100; i++)
        {
            sq.Parameters.Add(":" + i, i);
        }
        return sq;
    }

    private SelectQuery CarbunqlQuery { get; init; }

    private SelectQuery GetCarbunqlQuery()
    {
        var sq = SelectQuery.Parse(Sql);
        for (int i = 0; i < 100; i++)
        {
            sq.AddParameter(":" + i, i);
        }
        return sq;
    }

    [Benchmark]
    public string CarbunqlLexerRead()
    {
        var reader = new SqlLexReader(Sql);
        while (!string.IsNullOrEmpty(reader.Read())) { }

        return "success";// sq.ToQuery().CommandText;
    }

    [Benchmark]
    public string SqModelParse()
    {
        var sq = SqModel.Analysis.SqlParser.Parse(Sql);
        return "success";// sq.ToQuery().CommandText;
    }

    [Benchmark]
    public string SqModelCommandText()
    {
        return SqModelQuery.ToQuery().CommandText;
    }

    [Benchmark]
    public string CarbunqlDeepCopy()
    {
        var sq = CarbunqlQuery.DeepCopy();
        return "success";// sq.GetTokens().ToString(" ");
    }

    [Benchmark]
    public string CarbunqlParse()
    {
        var sq = QueryParser.Parse(Sql);
        return "success";// sq.GetTokens().ToString(" ");
    }

    [Benchmark]
    public string CarbunqlToOneLineText()
    {
        return CarbunqlQuery.ToOneLineText();
    }

    [Benchmark]
    public string CarbunqlToText()
    {
        return CarbunqlQuery.ToText();
    }
}