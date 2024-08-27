using Carbunql.Postgres;
using Carbunql.Postgres.Linq;
using Xunit.Abstractions;
using static Carbunql.Postgres.Linq.Sql;

namespace Carbunql.Postgres.Test.Linq;

public class TableInfoParserTest
{
    private readonly QueryCommandMonitor Monitor;

    public TableInfoParserTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
        Output = output;
    }

    private ITestOutputHelper Output { get; set; }

    private string TruncateControlString(string text)
    {
        return text.Replace("\r", "").Replace("\n", "").Replace(" ", "").Replace("\t", "").ToLower();
    }

    [Fact]
    public void DualTableTest()
    {
        var query = from a in Dual()
                    select new
                    {
                        v1 = 1,
                    };

        Monitor.Log(query);

        var sq = query.ToSelectQuery();
        Monitor.Log(sq);

        SelectableTableParser.TryParse(query.Expression, out var from);

        Assert.Null(from);

        var sql = @"
SELECT
    1 AS v1
";
        Assert.Equal(TruncateControlString(sql), TruncateControlString(sq.ToText()));
    }

    [Fact]
    public void TypeTableTest()
    {
        var query = from a in FromTable<table_a>()
                    select a;

        Monitor.Log(query);

        var sq = query.ToSelectQuery();
        Monitor.Log(sq);

        var from = SelectableTableParser.Parse(query.Expression);

        Assert.Equal("table_a as a", from?.ToOneLineText());
        Assert.Equal("a", from?.Alias);

        var sql = @"
SELECT
    a.a_id,
    a.text,
    a.value
FROM
    table_a AS a
";
        Assert.Equal(TruncateControlString(sql), TruncateControlString(sq.ToText()));
    }

    [Fact]
    public void StringTableTest()
    {
        var query = from a in FromTable<table_a>("TABLE_A")
                    select a;

        Monitor.Log(query);

        var sq = query.ToSelectQuery();
        Monitor.Log(sq);

        var from = SelectableTableParser.Parse(query.Expression);

        Assert.Equal("TABLE_A as a", from?.ToOneLineText());
        Assert.Equal("a", from?.Alias);

        var sql = @"
SELECT
    a.a_id,
    a.text,
    a.value
FROM
    table_a AS a
";
        Assert.Equal(TruncateControlString(sql), TruncateControlString(sq.ToText()));
    }

    [Fact]
    public void SubQueryTest()
    {
        var subquery = from a in FromTable<table_a>() select new { a.a_id };

        var query = from x in FromTable(subquery)
                    select x;

        Monitor.Log(query);

        var sq = query.ToSelectQuery();
        Monitor.Log(sq);

        var from = SelectableTableParser.Parse(query.Expression);

        Assert.Equal("(select a.a_id from table_a as a) as x", from?.ToOneLineText());
        Assert.Equal("x", from?.Alias);

        var sql = @"
select
	x.a_id
from
	(
		select
			a.a_id
		from 
			table_a as a
	) as x
";
        Assert.Equal(TruncateControlString(sql), TruncateControlString(sq.ToText()));
    }

    [Fact]
    public void SelectQueryClassTest()
    {
        var preset = SelectQuery.Parse("select x.a_id, x.value, x.text from table_x as x where x.a_id = 1");

        var query = from a in FromTable<table_a>(preset)
                    select a;

        Monitor.Log(query);

        var sq = query.ToSelectQuery();
        Monitor.Log(sq);

        var from = SelectableTableParser.Parse(query.Expression);

        Assert.Equal("(select x.a_id, x.value, x.text from table_x as x where x.a_id = 1) as a", from?.ToOneLineText());
        Assert.Equal("a", from?.Alias);

        var sql = @"
SELECT
    a.a_id,
    a.text,
    a.value
FROM
    (
        SELECT
            x.a_id,
            x.value,
            x.text
        FROM
            table_x AS x
        WHERE
            x.a_id = 1
    ) AS a
";
        Assert.Equal(TruncateControlString(sql), TruncateControlString(sq.ToText()));
    }


    [Fact]
    public void CommonTableTest()
    {
        var subquery = from a in FromTable<table_a>() select new { a.a_id };

        var query = from cte in CommonTable(subquery)
                    from x in FromTable(cte)
                    select x;

        Monitor.Log(query);

        var sq = query.ToSelectQuery();
        Monitor.Log(sq);

        var from = SelectableTableParser.Parse(query.Expression);

        Assert.Equal("cte as x", from?.ToOneLineText());
        Assert.Equal("x", from?.Alias);

        var sql = @"
WITH
    cte AS (
        SELECT
            a.a_id
        FROM
            table_a AS a
    )
SELECT
    x.a_id
FROM
    cte AS x
";
        Assert.Equal(TruncateControlString(sql), TruncateControlString(sq.ToText()));
    }

    [Fact]
    public void CommonTableNest2Test()
    {
        var subquery = from a in FromTable<table_a>() select new { a.a_id };

        var query = from cte1 in CommonTable(subquery)
                    from cte2 in CommonTable(subquery)
                    from x in FromTable(cte1)
                    select x;

        Monitor.Log(query);

        var sq = query.ToSelectQuery();
        Monitor.Log(sq);

        var from = SelectableTableParser.Parse(query.Expression);

        Assert.Equal("cte1 as x", from?.ToOneLineText());
        Assert.Equal("x", from?.Alias);

        var sql = @"
WITH
    cte1 AS (
        SELECT
            a.a_id
        FROM
            table_a AS a
    ),
    cte2 AS (
        SELECT
            a.a_id
        FROM
            table_a AS a
    )
SELECT
    x.a_id
FROM
    cte1 AS x
";
        Assert.Equal(TruncateControlString(sql), TruncateControlString(sq.ToText()));
    }

    [Fact]
    public void CommonTableNestManyTest()
    {
        var subquery = from a in FromTable<table_a>() select new { a.a_id };

        var query = from cte1 in CommonTable(subquery)
                    from cte2 in CommonTable(subquery)
                    from cte3 in CommonTable(subquery)
                    from cte4 in CommonTable(subquery)
                    from cte5 in CommonTable(subquery)
                    from x in FromTable(cte5)
                    select x;

        Monitor.Log(query);

        var sq = query.ToSelectQuery();
        Monitor.Log(sq);

        var from = SelectableTableParser.Parse(query.Expression);

        Assert.Equal("cte5 as x", from?.ToOneLineText());
        Assert.Equal("x", from?.Alias);

        var sql = @"
WITH
    cte1 AS (
        SELECT
            a.a_id
        FROM
            table_a AS a
    ),
    cte2 AS (
        SELECT
            a.a_id
        FROM
            table_a AS a
    ),
    cte3 AS (
        SELECT
            a.a_id
        FROM
            table_a AS a
    ),
    cte4 AS (
        SELECT
            a.a_id
        FROM
            table_a AS a
    ),
    cte5 AS (
        SELECT
            a.a_id
        FROM
            table_a AS a
    )
SELECT
    x.a_id
FROM
    cte5 AS x
";
        Assert.Equal(TruncateControlString(sql), TruncateControlString(sq.ToText()));
    }

    [Fact]
    public void CteAndDual()
    {
        var subquery = from a in FromTable<table_a>() select a.a_id;

        var query = from cte1 in CommonTable(subquery)
                    from cte2 in CommonTable(subquery)
                    from x in Dual()
                    select x;

        Monitor.Log(query);

        SelectableTableParser.TryParse(query.Expression, out var from);

        Assert.Null(from);
    }

    [Fact]
    public void CteAndTypeTable()
    {
        var subquery = from a in FromTable<table_a>() select a.a_id;

        var query = from cte1 in CommonTable(subquery)
                    from cte2 in CommonTable(subquery)
                    from x in FromTable<table_a>()
                    select x;

        Monitor.Log(query);

        var sq = query.ToSelectQuery();
        Monitor.Log(sq);

        var from = SelectableTableParser.Parse(query.Expression);

        Assert.Equal("table_a as x", from?.ToOneLineText());
        Assert.Equal("x", from?.Alias);
    }

    [Fact]
    public void CteAndStringTable()
    {
        var subquery = from a in FromTable<table_a>() select new { a.a_id };

        var query = from cte1 in CommonTable(subquery)
                    from cte2 in CommonTable(subquery)
                    from x in FromTable<table_a>("sales")
                    select x;

        Monitor.Log(query);

        var sq = query.ToSelectQuery();
        Monitor.Log(sq);

        var from = SelectableTableParser.Parse(query.Expression);

        Assert.Equal("sales as x", from?.ToOneLineText());
        Assert.Equal("x", from?.Alias);

        var sql = @"
WITH
    cte1 AS (
        SELECT
            a.a_id
        FROM
            table_a AS a
    ),
    cte2 AS (
        SELECT
            a.a_id
        FROM
            table_a AS a
    )
SELECT
    x.a_id,
    x.text,
    x.value
FROM
    sales AS x
";
        Assert.Equal(TruncateControlString(sql), TruncateControlString(sq.ToText()));
    }

    [Fact]
    public void CteAndSubQuery()
    {
        var subquery = from a in FromTable<table_a>() select a.a_id;

        var query = from cte1 in CommonTable(subquery)
                    from x in FromTable(subquery)
                    select x;

        Monitor.Log(query);

        var sq = query.ToSelectQuery();
        Monitor.Log(sq);

        var from = SelectableTableParser.Parse(query.Expression);

        Assert.Equal("(select a.a_id from table_a as a) as x", from?.ToOneLineText());
        Assert.Equal("x", from?.Alias);
    }

    [Fact]
    public void JoinAndTypeTableTest()
    {
        var query = from s in FromTable<sale>()
                    from a in InnerJoinTable<article>(x => s.article_id == x.article_id)
                    select a;

        Monitor.Log(query);

        var sq = query.ToSelectQuery();
        Monitor.Log(sq);

        var from = SelectableTableParser.Parse(query.Expression);
        Assert.Equal("sale as s", from?.ToOneLineText());
        Assert.Equal("s", from?.Alias);

        var sql = @"
SELECT
    a.article_id,
    a.article_name,
    a.price
FROM
    sale AS s
    INNER JOIN article AS a ON s.article_id = a.article_id
";
        Assert.Equal(TruncateControlString(sql), TruncateControlString(sq.ToText()));
    }

    [Fact]
    public void JoinAndStringTableTest()
    {
        var query = from s in FromTable<sale>("sales")
                    from a in InnerJoinTable<article>("articles", x => s.article_id == x.article_id)
                    select a;

        Monitor.Log(query);

        var sq = query.ToSelectQuery();
        Monitor.Log(sq);

        var from = SelectableTableParser.Parse(query.Expression);
        Assert.Equal("s", from?.Alias);
        Assert.Equal("sales as s", from?.ToOneLineText());

        var sql = @"
SELECT
    a.article_id,
    a.article_name,
    a.price
FROM
    sales AS s
    INNER JOIN articles AS a ON s.article_id = a.article_id
";
        Assert.Equal(TruncateControlString(sql), TruncateControlString(sq.ToText()));
    }

    [Fact]
    public void JoinAndSubQueryTest()
    {
        var subquery = from sales in FromTable<sale>() select sales;

        var query = from s in FromTable(subquery)
                    from a in InnerJoinTable<article>(x => s.article_id == x.article_id)
                    select s;

        Monitor.Log(query);

        var sq = query.ToSelectQuery();
        Monitor.Log(sq);

        var from = SelectableTableParser.Parse(query.Expression);

        Assert.Equal("(select sales.sales_id, sales.article_id, sales.quantity from sale as sales) as s", from?.ToOneLineText());
        Assert.Equal("s", from?.Alias);

        var sql = @"
SELECT
    s.sales_id,
    s.article_id,
    s.quantity
FROM
    (
        SELECT
            sales.sales_id,
            sales.article_id,
            sales.quantity
        FROM
            sale AS sales
    ) AS s
    INNER JOIN article AS a ON s.article_id = a.article_id
 
";
        Assert.Equal(TruncateControlString(sql), TruncateControlString(sq.ToText()));
    }

    public record struct table_a(int a_id, string text, int value);

    public record struct sale(int sales_id, int article_id, int quantity);

    public record struct article(int article_id, string article_name, int price);
}