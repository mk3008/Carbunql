using Xunit.Abstractions;

namespace Carbunql.Building.Test;

public class GetInternalQueriesTest
{
	private readonly ITestOutputHelper Output;

	public GetInternalQueriesTest(ITestOutputHelper output)
	{
		Output = output;
	}

	[Fact]
	public void CountTest()
	{
		var sq = new SelectQuery(@"
with
cte_a as (
	--index:0 cte query
    select
        a.id
    from
       table_a as a
)
--index:1 default
select
    a.id
from
    cte_a as a
union all
--index:2 union query
select
	b.id
from
	table_b as b
union all
--index:3
select
	c.id
from
	table_c as c
	inner join table_d as d on c.id = d.id
	inner join (
		--index:4 sub query
		select * from table_e as e
	) as e on d.id = e.id and d.key = (
		--index:5 join condition query
		select 5
	)
where
	c.id in (
		--index:6 in query
		select f.id from table_f as f
	)
	and d.id = (
		--index:7 condition value query
		select 7
	)
	and exists (
		--index:8 exists query
		select * from table_g as g where c.id = g.id
	)
union all
--index:9
select
	--index:10 inline query
	(select 1) as id");

		var cnt = 0;
		foreach (var q in sq.GetInternalQueries())
		{
			Output.WriteLine($"index : {cnt}");
			Output.WriteLine(q.ToText());
			cnt++;
		}

		Assert.Equal(11, cnt);
	}
}
