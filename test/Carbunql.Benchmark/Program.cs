using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Carbunql;
using Carbunql.Analysis;
using Carbunql.Extensions;

class Program
{
	static void Main(string[] args) => BenchmarkRunner.Run<Test>();
}

public class Test
{
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

	private SqModel.SelectQuery sqmodel = SqModel.Analysis.SqlParser.Parse(Sql);

	private IReadQuery carbunql = QueryParser.Parse(Sql);

	[Benchmark]
	public string SqModelParse()
	{
		var sq = SqModel.Analysis.SqlParser.Parse(Sql);
		return "success";// sq.ToQuery().CommandText;
	}

	[Benchmark]
	public string SqModelString()
	{
		return sqmodel.ToQuery().CommandText;
	}

	[Benchmark]
	public string CarbunqlDeepCopy()
	{
		var sq = carbunql.DeepCopy();
		return "success";// sq.GetTokens().ToString(" ");
	}

	[Benchmark]
	public string CarbunqlParse()
	{
		var sq = QueryParser.Parse(Sql);
		return "success";// sq.GetTokens().ToString(" ");
	}

	[Benchmark]
	public string CarbunqlString()
	{
		return carbunql.GetTokens(null).ToText();
	}

	[Benchmark]
	public string CarbunqlFormatString()
	{
		var cmd = new CommandTextBuilder();
		return cmd.Execute(carbunql);
	}
}