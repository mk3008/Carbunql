using Carbunql.Extensions;
using Carbunql.Tables;
using Carbunql.Values;
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

	private void InjectUserFilter(SelectQuery sq, string filteredTable, out bool isSuccess)
	{
		var filterQuery = new SelectQuery(@"
select 
	userid
from
	userdepartments 
where 
	departmentid = (
		select
			departmentid
		from
			userdepartments 
		where 
			userid = @currentUser
)
    ");
		isSuccess = false;

		foreach (var q in sq.GetInternalQueries())
		{
			var t = q.GetSelectableTables().Where(x => x.Table is PhysicalTable pt && pt.Table.IsEqualNoCase(filteredTable)).FirstOrDefault();
			if (t == null) continue;

			q.AddComment("user filter injected");
			var cnd = new InExpression(new ColumnValue(t, "CreatedBy"), new QueryContainer(filterQuery));
			q.Where(cnd);
			q.AddParameter("@currentUser", 1);
			isSuccess = true;
			return;
		}
	}

	[Fact]
	public void InjectWhereTest()
	{
		var sq = new SelectQuery(@"
select a.* from articles as a order by CreatedOn desc
");

		var filtered = false;
		InjectUserFilter(sq, "articles", out filtered);
		Output.WriteLine(sq.ToText());

		Assert.True(filtered);
		var expect = @"/*
  @currentUser = 1
*/
/* user filter injected */
SELECT
    a.*
FROM
    articles AS a
WHERE
    a.CreatedBy IN (
        SELECT
            userid
        FROM
            userdepartments
        WHERE
            departmentid = (
                SELECT
                    departmentid
                FROM
                    userdepartments
                WHERE
                    userid = @currentUser
            )
    )
ORDER BY
    CreatedOn DESC".Replace("\r", "").Replace("\n", "");

		Assert.Equal(expect, sq.ToText().Replace("\r", "").Replace("\n", ""));
	}

	[Fact]
	public void InjectWhereTest_Joined()
	{
		var sq = new SelectQuery(@"
select 
    c.categoryname
    , a.*
from
   articles as a
   inner join categories c on a.categoryid = c.categoryid
order by
   a.CreatedOn desc
");

		var filtered = false;
		InjectUserFilter(sq, "categories", out filtered);
		Output.WriteLine(sq.ToText());

		Assert.True(filtered);
		var expect = @"/*
  @currentUser = 1
*/
/* user filter injected */
SELECT
    c.categoryname,
    a.*
FROM
    articles AS a
    INNER JOIN categories AS c ON a.categoryid = c.categoryid
WHERE
    c.CreatedBy IN (
        SELECT
            userid
        FROM
            userdepartments
        WHERE
            departmentid = (
                SELECT
                    departmentid
                FROM
                    userdepartments
                WHERE
                    userid = @currentUser
            )
    )
ORDER BY
    a.CreatedOn DESC".Replace("\r", "").Replace("\n", "");

		Assert.Equal(expect, sq.ToText().Replace("\r", "").Replace("\n", ""));
	}

	[Fact]
	public void InjectWhereTest_SubQuery()
	{
		var sq = new SelectQuery(@"
select 
    a2.*
from
	(
		select a1.* from articles as a1
    ) a2
order by
   a2.CreatedOn desc
");

		var filtered = false;
		InjectUserFilter(sq, "articles", out filtered);
		Output.WriteLine(sq.ToText());

		Assert.True(filtered);
		var expect = @"/*
  @currentUser = 1
*/
SELECT
    a2.*
FROM
    (
        /* user filter injected */
        SELECT
            a1.*
        FROM
            articles AS a1
        WHERE
            a1.CreatedBy IN (
                SELECT
                    userid
                FROM
                    userdepartments
                WHERE
                    departmentid = (
                        SELECT
                            departmentid
                        FROM
                            userdepartments
                        WHERE
                            userid = @currentUser
                    )
            )
    ) AS a2
ORDER BY
    a2.CreatedOn DESC".Replace("\r", "").Replace("\n", "");

		Assert.Equal(expect, sq.ToText().Replace("\r", "").Replace("\n", ""));
	}

	[Fact]
	public void InjectWhereTest_CTE()
	{
		var sq = new SelectQuery(@"
with
a2 as (
	select a1.* from articles as a1
)
select 
    a2.*
from
	a2
order by
   a2.CreatedOn desc
");

		var filtered = false;
		InjectUserFilter(sq, "articles", out filtered);
		Output.WriteLine(sq.ToText());

		Assert.True(filtered);
		var expect = @"/*
  @currentUser = 1
*/
WITH
    a2 AS (
        /* user filter injected */
        SELECT
            a1.*
        FROM
            articles AS a1
        WHERE
            a1.CreatedBy IN (
                SELECT
                    userid
                FROM
                    userdepartments
                WHERE
                    departmentid = (
                        SELECT
                            departmentid
                        FROM
                            userdepartments
                        WHERE
                            userid = @currentUser
                    )
            )
    )
SELECT
    a2.*
FROM
    a2
ORDER BY
    a2.CreatedOn DESC".Replace("\r", "").Replace("\n", "");

		Assert.Equal(expect, sq.ToText().Replace("\r", "").Replace("\n", ""));
	}
}
