using Carbunql.Analysis;
using Carbunql.Extensions;
using Xunit.Abstractions;


namespace Carbunql.Building.Test;

public class InsertTest
{
    private readonly QueryCommandMonitor Monitor;

    public InsertTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void InsertQuery()
    {
        var sql = "select a.id, a.value as v from table as a";
        var q = QueryParser.Parse(sql);

        var iq = q.ToInsertQuery("new_table");
        Monitor.Log(iq);

        var lst = iq.GetTokens().ToList();

        Assert.Equal(21, lst.Count());

        var expect = @"INSERT INTO
    new_table(id, v)
SELECT
    a.id,
    a.value AS v
FROM
    table AS a";

        Assert.Equal(expect, iq.ToText(), true, true, true);
    }

    [Fact]
    public void InsertQuery_Values()
    {
        var sql = "values (1, 'a'), (2, 'b')";
        var q = QueryParser.Parse(sql);

        var iq = q.ToInsertQuery("new_table");
        Monitor.Log(iq);

        var lst = iq.GetTokens().ToList();

        Assert.Equal(14, lst.Count());

        var expect = @"INSERT INTO
    new_table
VALUES
    (1, 'a'),
    (2, 'b')";

        Assert.Equal(expect, iq.ToText(), true, true, true);
    }

    [Fact]
    public void InsertQuery_ColumnFilter()
    {
        var sql = "select a.id, a.value as v from table as a";
        var tmp = QueryParser.Parse(sql);

        var sq = new SelectQuery();
        var (f, q) = sq.From(tmp).As("q");
        q.GetColumnNames().Where(x => x.IsEqualNoCase("id")).ToList().ForEach(x => sq.Select(q, x));

        var iq = sq.ToInsertQuery("new_table");
        Monitor.Log(iq);

        var lst = iq.GetTokens().ToList();

        Assert.Equal(28, lst.Count());

        var expect = @"INSERT INTO
    new_table(id)
SELECT
    q.id
FROM
    (
        SELECT
            a.id,
            a.value AS v
        FROM
            table AS a
    ) AS q";

        Assert.Equal(expect, iq.ToText(), true, true, true);
    }

    [Fact]
    public void InsertQuery_Returning()
    {
        var sql = "select a.id, a.value as v from table as a";
        var tmp = QueryParser.Parse(sql);

        var sq = new SelectQuery();
        var (f, q) = sq.From(tmp).As("q");
        q.GetColumnNames().Where(x => x.IsEqualNoCase("id")).ToList().ForEach(x => sq.Select(q, x));

        var iq = sq.ToInsertQuery("new_table");
        iq.Returning("seq");
        Monitor.Log(iq);

        var lst = iq.GetTokens().ToList();

        Assert.Equal(30, lst.Count());

        var expect = @"INSERT INTO
    new_table(id)
SELECT
    q.id
FROM
    (
        SELECT
            a.id,
            a.value AS v
        FROM
            table AS a
    ) AS q
RETURNING
    seq";

        Assert.Equal(expect, iq.ToText(), true, true, true);
    }
}