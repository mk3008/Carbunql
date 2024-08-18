using Carbunql.Postgres;
using Carbunql.Postgres.Linq;
using Xunit.Abstractions;
using static Carbunql.Postgres.Linq.Sql;

namespace Carbunql.Postgres.Test.Linq;

public class SelectColumnInfoParserTest
{
    private readonly QueryCommandMonitor Monitor;

    public SelectColumnInfoParserTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
        Output = output;
    }

    private ITestOutputHelper Output { get; set; }

    [Fact]
    public void SelectAll()
    {
        var query = from a in FromTable<table_a>()
                    select a;

        Monitor.Log(query);

        var sq = query.ToSelectQuery();
        Monitor.Log(sq);

        var items = SelectableItemParser.Parse(query.Expression);
        Assert.Equal(3, items.Count);
        Assert.Equal("a_id", items[0].Alias);
        Assert.Equal("a.a_id", items[0].Value.ToText());
        Assert.Equal("text", items[1].Alias);
        Assert.Equal("a.text", items[1].Value.ToText());
        Assert.Equal("value", items[2].Alias);
        Assert.Equal("a.value", items[2].Value.ToText());

        var sql = @"
SELECT
    a.a_id,
    a.text,
    a.value
FROM
    table_a AS a
";
        Assert.Equal(sql.RemoveControlChar(), sq.ToText().RemoveControlChar());
    }

    [Fact]
    public void SelectColumn()
    {
        var query = from a in FromTable<table_a>()
                    select new { a.a_id };

        Monitor.Log(query);

        var sq = query.ToSelectQuery();
        Monitor.Log(sq);

        var items = SelectableItemParser.Parse(query.Expression);
        Assert.Single(items);
        Assert.Equal("a_id", items[0].Alias);
        Assert.Equal("a.a_id", items[0].Value.ToText());

        var sql = @"
SELECT
    a.a_id
FROM
    table_a AS a";
        Assert.Equal(sql.RemoveControlChar(), sq.ToText().RemoveControlChar());
    }

    [Fact]
    public void SelectColumns()
    {
        var query = from a in FromTable<table_a>()
                    select new
                    {
                        a.a_id,
                        a.text,
                        a.value
                    };

        Monitor.Log(query);

        var sq = query.ToSelectQuery();
        Monitor.Log(sq);

        var items = SelectableItemParser.Parse(query.Expression);
        Assert.Equal(3, items.Count);
        Assert.Equal("a_id", items[0].Alias);
        Assert.Equal("a.a_id", items[0].Value.ToText());
        Assert.Equal("text", items[1].Alias);
        Assert.Equal("a.text", items[1].Value.ToText());
        Assert.Equal("value", items[2].Alias);
        Assert.Equal("a.value", items[2].Value.ToText());

        var sql = @"
SELECT
    a.a_id,
    a.text,
    a.value
FROM
    table_a AS a
";
        Assert.Equal(sql.RemoveControlChar(), sq.ToText().RemoveControlChar());
    }

    [Fact]
    public void Alias()
    {
        var pi = 3.14;

        var query = from a in FromTable<table_a>()
                    select new
                    {
                        ID = a.a_id,
                        TEXT = a.text,
                        VALUE = a.value,
                        V1 = 1,
                        V2 = 1 + 1,
                        V3 = pi
                    };

        Monitor.Log(query);

        var sq = query.ToSelectQuery();
        Monitor.Log(sq);

        var items = SelectableItemParser.Parse(query.Expression);
        Assert.Equal(6, items.Count);
        Assert.Equal("ID", items[0].Alias);
        Assert.Equal("a.a_id", items[0].Value.ToText());
        Assert.Equal("TEXT", items[1].Alias);
        Assert.Equal("a.text", items[1].Value.ToText());
        Assert.Equal("VALUE", items[2].Alias);
        Assert.Equal("a.value", items[2].Value.ToText());
        Assert.Equal("V1", items[3].Alias);
        Assert.Equal("1", items[3].Value.ToText());
        Assert.Equal("V2", items[4].Alias);
        Assert.Equal("2", items[4].Value.ToText());
        Assert.Equal("V3", items[5].Alias);
        Assert.Equal(":member_pi", items[5].Value.ToText());

        var sql = @"SELECT
    a.a_id AS ID,
    a.text AS TEXT,
    a.value AS VALUE,
    1 AS V1,
    2 AS V2,
    :member_pi AS V3
FROM
    table_a AS a";
        Assert.Equal(sql.RemoveControlChar(), sq.ToText().RemoveControlChar());
    }

    [Fact]
    public void JoinSelectColumns()
    {
        var query = from a in FromTable<table_a>()
                    from b in CrossJoinTable<table_a>()
                    where a.a_id == 1 && b.text == "test"
                    select new
                    {
                        a.a_id,
                        b.text
                    };

        Monitor.Log(query);

        var sq = query.ToSelectQuery();
        Monitor.Log(sq);

        var items = SelectableItemParser.Parse(query.Expression);
        Assert.Equal(2, items.Count);
        Assert.Equal("a_id", items[0].Alias);
        Assert.Equal("a.a_id", items[0].Value.ToText());
        Assert.Equal("text", items[1].Alias);
        Assert.Equal("b.text", items[1].Value.ToText());

        var sql = @"
SELECT
    a.a_id,
    b.text
FROM
    table_a AS a
    CROSS JOIN table_a as b
WHERE
    (a.a_id = 1 AND b.text = 'test')";
        Assert.Equal(sql.RemoveControlChar(), sq.ToText().RemoveControlChar());
    }

    [Fact]
    public void JoinSelectAll()
    {
        var query = from a in FromTable<table_a>()
                    from b in CrossJoinTable<table_a>()
                    where a.a_id == 1 && b.text == "test"
                    select a;

        Monitor.Log(query);

        var sq = query.ToSelectQuery();
        Monitor.Log(sq);

        var items = SelectableItemParser.Parse(query.Expression);
        Assert.Equal(3, items.Count);
        Assert.Equal("a_id", items[0].Alias);
        Assert.Equal("a.a_id", items[0].Value.ToText());
        Assert.Equal("text", items[1].Alias);
        Assert.Equal("a.text", items[1].Value.ToText());
        Assert.Equal("value", items[2].Alias);
        Assert.Equal("a.value", items[2].Value.ToText());

        var sql = @"
SELECT
    a.a_id,
    a.text,
    a.value
FROM
    table_a AS a
    CROSS JOIN table_a as b
WHERE
    (a.a_id = 1 AND b.text = 'test')";
        Assert.Equal(sql.RemoveControlChar(), sq.ToText().RemoveControlChar());
    }

    [Fact]
    public void JoinSelectAll_2()
    {
        var query = from a in FromTable<table_a>()
                    from b in CrossJoinTable<table_a>()
                    where a.a_id == 1 && b.text == "test"
                    select b;

        Monitor.Log(query);

        var sq = query.ToSelectQuery();
        Monitor.Log(sq);

        var items = SelectableItemParser.Parse(query.Expression);
        Assert.Equal(3, items.Count);
        Assert.Equal("a_id", items[0].Alias);
        Assert.Equal("b.a_id", items[0].Value.ToText());
        Assert.Equal("text", items[1].Alias);
        Assert.Equal("b.text", items[1].Value.ToText());
        Assert.Equal("value", items[2].Alias);
        Assert.Equal("b.value", items[2].Value.ToText());

        var sql = @"
SELECT
    b.a_id,
    b.text,
    b.value
FROM
    table_a AS a
    CROSS JOIN table_a as b
WHERE
    (a.a_id = 1 AND b.text = 'test')";
        Assert.Equal(sql.RemoveControlChar(), sq.ToText().RemoveControlChar());
    }

    [Fact]
    public void MemberInit()
    {
        var query = from a in FromTable<table_a>()
                    select new table_b
                    (
                        a.a_id,
                        a.text,
                        a.a_id * 10
                    );

        Monitor.Log(query);

        var sq = query.ToSelectQuery();
        Monitor.Log(sq);

        var sql = @"
SELECT
    a.a_id AS b_id,
    a.text,
    a.a_id * 10 AS value
FROM
    table_a AS a";
        Assert.Equal(sql.RemoveControlChar(), sq.ToText().RemoveControlChar());
    }

    public record struct table_a(int a_id, string text, int value);

    public record class table_b(int b_id, string text, int value);
}