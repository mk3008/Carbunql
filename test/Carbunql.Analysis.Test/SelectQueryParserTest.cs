using Carbunql.Tables;
using Xunit.Abstractions;

namespace Carbunql.Analysis.Test;

public class SelectQueryParserTest
{
	private readonly QueryCommandMonitor Monitor;

	public SelectQueryParserTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
	}

	[Fact]
	public void SortSample()
	{
		var text = @"
select
    *
from 
    table_a a
order by 
    a.name nulls first,
    a.val desc,
    a.table_a_id";

		var item = QueryParser.Parse(text) as SelectQuery;
		if (item == null) throw new Exception();

		Monitor.Log(item);

		var lst = item.GetTokens().ToList();

		Assert.Equal(20, lst.Count);

		Assert.NotNull(item.SelectClause);
		Assert.Single(item.SelectClause);
		Assert.NotNull(item.OrderClause);
		Assert.Equal(3, item.OrderClause!.Count());

		Assert.Equal("table_a", lst[3].Text);
		Assert.Equal("as", lst[4].Text);
		Assert.Equal("a", lst[5].Text);
		Assert.Equal("order by", lst[6].Text);
		Assert.Equal("a", lst[7].Text);
		Assert.Equal(".", lst[8].Text);
		Assert.Equal("name", lst[9].Text);
	}

	[Fact]
	public void NumericSample()
	{
		var text = @"
select
    1 as v1,
    -1 as v2,
    1-1 as v3,
    -1 * 1 as v4,
    +1 * 1 as v5
";

		var item = QueryParser.Parse(text) as SelectQuery;
		if (item == null) throw new Exception();

		Monitor.Log(item);

		var lst = item.GetTokens().ToList();
		Assert.Equal(26, lst.Count);

		Assert.Equal(",", lst[8].Text);
		Assert.Equal("1", lst[9].Text);
		Assert.Equal("-", lst[10].Text);
		Assert.Equal("1", lst[11].Text);
		Assert.Equal("as", lst[12].Text);
		Assert.Equal("v3", lst[13].Text);

		Assert.Equal(",", lst[14].Text);
		Assert.Equal("-1", lst[15].Text);
		Assert.Equal("*", lst[16].Text);
		Assert.Equal("1", lst[17].Text);
		Assert.Equal("as", lst[18].Text);
		Assert.Equal("v4", lst[19].Text);

		Assert.Equal(",", lst[20].Text);
		Assert.Equal("+1", lst[21].Text);
		Assert.Equal("*", lst[22].Text);
		Assert.Equal("1", lst[23].Text);
		Assert.Equal("as", lst[24].Text);
		Assert.Equal("v5", lst[25].Text);
	}

	[Fact]
	public void CaseSample()
	{
		var text = @"
select
    1 as v0
    , case a.id when 1 then 'a' when 2 then 'b' end as v1
    , case a.id when 1 then 'a' when 2 then 'b' else null end as v2
    , case when a.id = 1 then 'a' when a.id = 2 then 'b' end as v3
    , case when a.id = 1 then 'a' when a.id = 2 then 'b' else null end as v4
    , concat( 
        case a.id when 1 then 'a' when 2 then 'b' end,
        'test', 
        '123', 
        case when a.id = 1 then 'a' when a.id = 2 then 'b' end
      ) as text
    , 9 as v9
from 
    table_a a";

		var item = QueryParser.Parse(text) as SelectQuery;
		if (item == null) throw new Exception();

		Monitor.Log(item);

		var lst = item.GetTokens().ToList();
		Assert.Equal(132, lst.Count);
	}


	[Fact]
	public void RelationSample()
	{
		var text = @"
select
    a.table_a_id as id,
    3.14 as val,
    (a.val + b.val) * 2 as calc, 
    b.table_b_id,
    c.table_c_id
from 
    table_a a
    inner join table_b b on a.table_a_id = b.table_a_id and b.visible = true
    left join table_c c on a.table_a_id = c.table_a_id
    right outer join table_d d on a.table_a_id = d.table_a_id";

		var item = QueryParser.Parse(text) as SelectQuery;
		if (item == null) throw new Exception();
		Monitor.Log(item);

		var lst = item.GetTokens().ToList();
		Assert.Equal(78, lst.Count);

		Assert.NotNull(item.SelectClause);
		Assert.Equal(5, item.SelectClause!.Count);
		Assert.Equal(3, item.FromClause!.Relations!.Count());

		var tablenames = item.GetPhysicalTables().ToList();
		Assert.Equal(4, tablenames.Count);
		Assert.Equal("table_a", tablenames[0].GetTableFullName());
		Assert.Equal("table_b", tablenames[1].GetTableFullName());
		Assert.Equal("table_c", tablenames[2].GetTableFullName());
		Assert.Equal("table_d", tablenames[3].GetTableFullName());
	}

	[Fact]
	public void RelationSample_NoAlias()
	{
		var text = @"
select * from a inner join b on a.id = b.id";

		var item = QueryParser.Parse(text) as SelectQuery;
		if (item == null) throw new Exception();
		Monitor.Log(item);

		var lst = item.GetTokens().ToList();
		Assert.Equal(14, lst.Count);

		Assert.Equal("a", lst[3].Text);
		Assert.Equal("inner join", lst[4].Text);
		Assert.Equal("b", lst[5].Text);
	}

	[Fact]
	public void RelationSample_Comma()
	{
		var text = @"select * from a, b where a.id = b.id";

		var item = QueryParser.Parse(text) as SelectQuery;
		if (item == null) throw new Exception();
		Monitor.Log(item);

		var lst = item.GetTokens().ToList();
		Assert.Equal(14, lst.Count);

		Assert.Equal("a", lst[3].Text);
		Assert.Equal(",", lst[4].Text);
		Assert.Equal("b", lst[5].Text);
		Assert.Equal("where", lst[6].Text);
	}

	[Fact]
	public void GroupSample()
	{
		var text = @"
select
    a.name,
    a.sub_name,
    sum(a.val) as val
from 
    table_a a
group by
    a.name,
    a.sub_name
having
    sum(a.val) > 0
    and sum(a.val) < 10";

		var item = QueryParser.Parse(text) as SelectQuery;
		if (item == null) throw new Exception();
		Monitor.Log(item);

		var lst = item.GetTokens().ToList();
		Assert.Equal(47, lst.Count);

		Assert.NotNull(item.SelectClause);
		Assert.Equal(3, item.SelectClause!.Count);
		Assert.NotNull(item.GroupClause);
		Assert.Equal(2, item.GroupClause!.Count());
		Assert.NotNull(item.HavingClause);
	}

	[Fact]
	public void UnionSample()
	{
		var text = @"
select
    a.id
from
    table_a as a
union
select
    b.id
from
    table_b as b
union all
select
    c.id
from
    table_c as c";

		var item = QueryParser.Parse(text) as SelectQuery;
		if (item == null) throw new Exception();
		Monitor.Log(item);

		var lst = item.GetTokens().ToList();
		Assert.Equal(26, lst.Count);

		/*
		Function 'GetSelectableTables' is obsolete.
		Use the functions 'GetInternalQueries' and 'GetTables'.
		*/
		var tables = item.GetInternalQueries().SelectMany(x => x.GetSelectableTables()).ToList();

		Assert.Equal(3, tables.Count);
		Assert.Equal("table_a", tables[0].Table.GetTableFullName());
		Assert.Equal("table_b", tables[1].Table.GetTableFullName());
		Assert.Equal("table_c", tables[2].Table.GetTableFullName());

		var tablenames = tables.Where(x => x.Table is PhysicalTable).Select(x => ((PhysicalTable)x.Table).Table).ToList();
		Assert.Equal(3, tablenames.Count());
		Assert.Equal("table_a", tablenames[0]);
		Assert.Equal("table_b", tablenames[1]);
		Assert.Equal("table_c", tablenames[2]);
	}

	[Fact]
	public void WithSample()
	{
		var text = @"
with
a as (
    select
        a.id
    from
        table_a as a
), 
b as (
    select
        a.id
    from
        a
)
select
    *
from
    b";

		var item = new SelectQuery(text);
		Monitor.Log(item);

		var lst = item.GetTokens().ToList();
		Assert.Equal(28, lst.Count);

		Assert.NotNull(item.GetWithClause());

		/*
		Function 'GetSelectableTables' is obsolete.
		Use the functions 'GetInternalQueries' and 'GetTables'.
		*/
		var tables = item.GetInternalQueries().SelectMany(x => x.GetSelectableTables()).ToList();

		Assert.Equal(3, tables.Count);
		Assert.Equal("table_a", tables[0].Table.GetTableFullName());
		Assert.Equal("a", tables[1].Table.GetTableFullName());
		Assert.Equal("b", tables[2].Table.GetTableFullName());

		var tables2 = item.GetPhysicalTables().ToList();
		Assert.Equal(3, tables2.Count);
		Assert.Equal("table_a", tables2[0].GetTableFullName());
		Assert.Equal("a", tables2[1].GetTableFullName());
		Assert.Equal("b", tables2[2].GetTableFullName());
	}

	[Fact]
	public void LimitSample()
	{
		var text = @"
select
    a.id
from
    table_a as a
limit 10";

		var item = QueryParser.Parse(text) as SelectQuery;
		if (item == null) throw new Exception();
		Monitor.Log(item);

		var lst = item.GetTokens().ToList();
		Assert.Equal(10, lst.Count);
	}

	[Fact]
	public void LimitSample_NoAlias()
	{
		var text = @"
select
    a.id
from
    table_a
limit 10";

		var item = QueryParser.Parse(text) as SelectQuery;
		if (item == null) throw new Exception();
		Monitor.Log(item);

		var lst = item.GetTokens().ToList();
		Assert.Equal(8, lst.Count());

		Assert.Equal("from", lst[4].Text);
		Assert.Equal("table_a", lst[5].Text);
		Assert.Equal("limit", lst[6].Text);
		Assert.Equal("10", lst[7].Text);
	}

	[Fact]
	public void LimitSample_Postgres()
	{
		var text = @"
select
    a.id
from
    table_a as a
limit 10 offset 3";

		var item = QueryParser.Parse(text) as SelectQuery;
		if (item == null) throw new Exception();
		Monitor.Log(item);

		var lst = item.GetTokens().ToList();
		Assert.Equal(12, lst.Count);
	}

	[Fact]
	public void LimitSample_MySQL()
	{
		var text = @"
select
    a.id
from
    table_a as a
limit 3, 10";

		var item = QueryParser.Parse(text) as SelectQuery;
		if (item == null) throw new Exception();
		Monitor.Log(item);

		var lst = item.GetTokens().ToList();
		Assert.Equal(12, lst.Count);
	}

	[Fact]
	public void FunctionTableSample()
	{
		var text = @"SELECT current_date + s.a AS dates FROM generate_series(0,14,7) AS s(a)";

		var item = QueryParser.Parse(text);
		if (item == null) throw new Exception();
		Monitor.Log(item);

		var lst = item.GetTokens().ToList();
		Assert.Equal(22, lst.Count);
	}

	[Fact]
	public void Sample()
	{
		var text = @"
with
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

		var item = QueryParser.Parse(text);
		Monitor.Log(item);

		var lst = item.GetTokens().ToList();
		Assert.Equal(330, lst.Count);

		Assert.NotNull(item.GetWithClause());
	}

	[Fact]
	public void SemicolonBreak()
	{
		var text = @"
select
    a.id
from
    table_a; as a
limit 10";

		var item = QueryParser.Parse(text) as SelectQuery;
		if (item == null) throw new Exception();
		Monitor.Log(item);

		var lst = item.GetTokens().ToList();
		Assert.Equal(6, lst.Count);
	}

	[Fact]
	public void WindowFunction_Filter()
	{
		var text = @"
with
v (id, name, value) as (
    values
    (1, 'a', 10)
    , (2, 'a', 20)
    , (3, 'b', 50)
    , (4, 'c', 70)
)
select  
    sum(value) filter (where v.name = 'a') as value_a
    , sum(value) filter (where v.name = 'b') as value_b
    , sum(value) filter (where v.name = 'c') as value_b
from
    v
";

		var item = QueryParser.Parse(text) as SelectQuery;
		if (item == null) throw new Exception();
		Monitor.Log(item);

		var lst = item.GetTokens().ToList();
		Assert.Equal(94, lst.Count);
	}

	[Fact]
	public void WindowFunction_Filter_Over()
	{
		var text = @"
with
v (id, name, value) as (
    values
    (1, 'a', 10)
    , (2, 'a', 20)
    , (3, 'b', 50)
    , (4, 'c', 70)
)
select  
    id
    , name
    , value
    , string_agg(id::text, ',') filter (where v.name = 'a') over (partition by name order by value) as text
from
    v
";

		var item = QueryParser.Parse(text) as SelectQuery;
		if (item == null) throw new Exception();
		Monitor.Log(item);

		var lst = item.GetTokens().ToList();
		Assert.Equal(79, lst.Count);
	}

	[Fact]
	public void WindowClause()
	{
		var text = @"
SELECT sum(salary) OVER w as v1, avg(salary) OVER w as v2
  FROM empsalary
  WINDOW w AS (PARTITION BY depname ORDER BY salary DESC)
";

		var item = QueryParser.Parse(text) as SelectQuery;
		if (item == null) throw new Exception();
		Monitor.Log(item);

		var lst = item.GetTokens().ToList();
		Assert.Equal(30, lst.Count);
	}

	[Fact]
	public void IsDistinctFrom()
	{
		var text = @"
select 
    null is not distinct from null
    , 1 is not distinct from null
    , null is not distinct from 1
    , 1 is not distinct from 1
    , null is distinct from null
    , 1 is distinct from null
    , null is distinct from 1
    , 1 is distinct from 1
";

		var item = QueryParser.Parse(text) as SelectQuery;
		if (item == null) throw new Exception();
		Monitor.Log(item);

		var lst = item.GetTokens().ToList();
		Assert.Equal(32, lst.Count);
	}

	[Fact]
	public void UnionIssue197()
	{
		var text = @"
select 1
union all
select 1";

		var item = QueryParser.Parse(text) as SelectQuery;
		if (item == null) throw new Exception();
		Monitor.Log(item);

		var lst = item.GetTokens().ToList();
		Assert.Equal(5, lst.Count);
	}

	[Fact]
	public void JsonFunction()
	{
		var text = @"
select
'[{""a"":""foo""},{""b"":""bar""},{""c"":""baz""}]'::json->2 as test1
, '{""a"": {""b"":""foo""}}'::json->'a' as test2
, '[1,2,3]'::json->>2 as test3
, '{""a"":1,""b"":2}'::json->>'b' as test4
, '{""a"": {""b"":{""c"": ""foo""}}}'::json#>'{a,b}' as test5
, '{""a"":[1,2,3],""b"":[4,5,6]}'::json#>>'{a,2}' as test6
, '{""id"":1, ""value"":""test"", ""nest"":{""id"":2, ""value"":""data""}}'::json->'nest' as test7
, '{""id"":1, ""value"":""test"", ""nest"":{""id"":2, ""value"":""data""}}'::json->'nest'->'value' as test8";

		var item = QueryParser.Parse(text) as SelectQuery;
		if (item == null) throw new Exception();
		Monitor.Log(item);

		var lst = item.GetTokens().ToList();
		Assert.Equal(66, lst.Count);
	}

	[Fact]
	public void Issue270()
	{
		var text = @"
select *
from TableA
join TableB
  on TableA.Id = TableB.FK";

		var item = QueryParser.Parse(text) as SelectQuery;
		if (item == null) throw new Exception();
		Monitor.Log(item);

		var lst = item.GetTokens().ToList();
		Assert.Equal(14, lst.Count);
	}

	[Fact]
	public void ParserError()
	{
		var e = Assert.Throws<NotSupportedException>(() =>
		{
			var text = "SELECT a b c";
			var item = QueryParser.Parse(text) as SelectQuery;
		});

		Assert.Equal("Parsing terminated despite the presence of unparsed tokens.(token:'c')", e.Message);
	}

	[Fact]
	public void TimeZone_Postgres()
	{
		var text = @"
SELECT
	--Postgers
	now()::timestamp without time zone as t1
";
		var item = QueryParser.Parse(text);
	}

	[Fact]
	public void TimeZone()
	{
		var text = @"
SELECT
	--Postgers
	now()::timestamp without time zone as t1
	, now() at time zone 'Asia/Tokyo' as t2
	--SQLServer
	, SYSDATETIMEOFFSET() AT TIME ZONE 'UTC' AS t3
	, GETDATE() AT TIME ZONE 'Tokyo Standard Time' AS t4
	--MySQL
	--, CONVERT_TZ(NOW(), @@session.time_zone, '+00:00') AS t5
	, CONVERT_TZ(NOW(), 'UTC', 'Asia/Tokyo') as t6
";

		var item = QueryParser.Parse(text);
		if (item == null) throw new Exception();
		Monitor.Log(item);

		var lst = item.GetTokens().ToList();
		Assert.Equal(46, lst.Count);

		var sql = @"SELECT
    NOW()::timestamp WITHOUT TIME ZONE AS t1,
    NOW() AT TIME ZONE 'Asia/Tokyo' AS t2,
    SYSDATETIMEOFFSET() AT TIME ZONE 'UTC' AS t3,
    GETDATE() AT TIME ZONE 'Tokyo Standard Time' AS t4,
    CONVERT_TZ(NOW(), 'UTC', 'Asia/Tokyo') AS t6";

		Assert.Equal(sql, item.ToText(), true, true, true);
	}

	[Fact]
	public void EmptyBracket()
	{
		var text = @"
select 
    v1
from
    ((select 1 as v1)) d
";

		var item = QueryParser.Parse(text);
		if (item == null) throw new Exception();
		Monitor.Log(item);

		var sql = @"SELECT
    v1
FROM
    (
        (
            SELECT
                1 AS v1
        )
    ) AS d";

		Assert.Equal(sql, item.ToText(), true, true, true);
	}


	[Fact]
	public void PostgresOperator()
	{
		var text = @"select
	'((0,0),1)'::circle <-> '((5,0),1)'::circle";

		var sq = QueryParser.Parse(text);
		Monitor.Log(sq);

		var lst = sq.GetTokens().ToList();
		Assert.Equal(8, lst.Count);

		Assert.Equal(text, sq.ToText(), true, true, true);
	}

	[Fact]
	public void JoinLateral()
	{
		var text = @"SELECT
    m.name
FROM
    manufacturers AS m
    LEFT JOIN LATERAL GET_PRODUCT_NAMES(m.id) AS pname ON true
WHERE
    pname IS NULL";

		var sq = QueryParser.Parse(text);
		Monitor.Log(sq);

		var lst = sq.GetTokens().ToList();
		Assert.Equal(24, lst.Count);

		Assert.Equal(text, sq.ToText(), true, true, true);
	}

	[Fact]
	public void CommaLateral()
	{
		var text = @"SELECT
    *
FROM
    foo,
    LATERAL (
        SELECT
            *
        FROM
            bar
        WHERE
            bar.id = foo.bar_id
    ) AS ss";

		var sq = QueryParser.Parse(text);
		Monitor.Log(sq);

		var lst = sq.GetTokens().ToList();
		Assert.Equal(22, lst.Count);

		Assert.Equal(text, sq.ToText(), true, true, true);
	}

	[Fact]
	public void DistinctOn()
	{
		var text = @"SELECT DISTINCT ON (location)
    location,
    time,
    report
FROM
    weather_reports
ORDER BY
    location,
    time DESC";

		var sq = QueryParser.Parse(text);
		Monitor.Log(sq);

		var lst = sq.GetTokens().ToList();
		Assert.Equal(18, lst.Count);

		Assert.Equal(text, sq.ToText(), true, true, true);
	}

	[Fact]
	public void Top()
	{
		var text = @"SELECT TOP 5
    id,
    value
FROM
    Table
ORDER BY
    id";

		var sq = QueryParser.Parse(text);
		Monitor.Log(sq);

		var lst = sq.GetTokens().ToList();
		Assert.Equal(10, lst.Count);

		Assert.Equal(text, sq.ToText(), true, true, true);
	}

	[Fact]
	public void TopSelectAll()
	{
		var text = @"SELECT TOP 5
    *
FROM
    Table
ORDER BY
    id";

		var sq = QueryParser.Parse(text);
		Monitor.Log(sq);

		var lst = sq.GetTokens().ToList();
		Assert.Equal(8, lst.Count);

		Assert.Equal(text, sq.ToText(), true, true, true);
	}

	[Fact]
	public void ExceptLexemeError()
	{
		//outer
		var text = @"select * from a left outeraaaa join b on 1 = 1";

		var e = Assert.Throws<SyntaxException>(() =>
		{
			var sq = QueryParser.Parse(text);
		});

		Assert.Equal("expect : 'outer', actual : 'outeraaaa'", e.Message);
	}

	[Fact]
	public void LineCommentBreak()
	{
		var text = @"select 1+--
2";
		var expect = @"SELECT
    1 + 2";

		var sq = QueryParser.Parse(text);
		Monitor.Log(sq);

		var lst = sq.GetTokens().ToList();
		Assert.Equal(4, lst.Count);

		Assert.Equal(expect, sq.ToText(), true, true, true);
	}

	[Fact]
	public void BlockCommentBreak()
	{
		var text = @"select 1+/**/
2";
		var expect = @"SELECT
    1 + 2";

		var sq = QueryParser.Parse(text);
		Monitor.Log(sq);

		var lst = sq.GetTokens().ToList();
		Assert.Equal(4, lst.Count);

		Assert.Equal(expect, sq.ToText(), true, true, true);
	}
}