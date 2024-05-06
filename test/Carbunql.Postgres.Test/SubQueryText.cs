using Carbunql.Building;
using Carbunql.Postgres;
using Xunit.Abstractions;

namespace Carbunql.Postgres.Test;

public class SubQueryText
{
    private readonly QueryCommandMonitor Monitor;

    public SubQueryText(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
        Output = output;
    }

    private ITestOutputHelper Output { get; set; }

    [Fact]
    public void FromAs()
    {
        var sq = new SelectQuery();
        var (from, a) = sq.FromAs<RecordA>("a");

        sq.Select("a", "a_id");
        sq.Select(nameof(a), "a_id");
        sq.Select(() => a.a_id);
        sq.Select(() => a.a_id).As("id");

        Monitor.Log(sq);

        var sql = @"SELECT
    a.a_id,
    a.a_id,
    a.a_id,
    a.a_id AS id
FROM
    RecordA AS a";

        Assert.Equal(22, sq.GetTokens().ToList().Count);
        Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
    }

    [Fact]
    public void AliasAs()
    {
        var sq = new SelectQuery();
        var (from, a) = sq.From("table_a").As<RecordA>("a"); ;

        sq.Select("a", "a_id");
        sq.Select(nameof(a), "a_id");
        sq.Select(() => a.a_id);
        sq.Select(() => a.a_id).As("id");

        Monitor.Log(sq);

        Assert.Equal(22, sq.GetTokens().ToList().Count);

        var sql = @"SELECT
    a.a_id,
    a.a_id,
    a.a_id,
    a.a_id AS id
FROM
    table_a AS a";
        Assert.Equal(sql, sq.ToText(), true, true, true);

    }
    [Fact]
    public void SubQueryTest()
    {
        var sq = new SelectQuery();
        var (from, a) = sq.From(new SelectQuery("select * from table_a")).As<RecordA>("a"); ;

        sq.Select("a", "a_id");
        sq.Select(nameof(a), "a_id");
        sq.Select(() => a.a_id);
        sq.Select(() => a.a_id).As("id");

        Monitor.Log(sq);

        Assert.Equal(27, sq.GetTokens().ToList().Count);

        var sql = @"SELECT
    a.a_id,
    a.a_id,
    a.a_id,
    a.a_id AS id
FROM
    (
        SELECT
            *
        FROM
            table_a
    ) AS a";
        Assert.Equal(sql, sq.ToText(), true, true, true);
    }

    [Fact]
    public void SubroutineTest()
    {
        var sq = new SelectQuery();
        var (from, a) = sq.FromAs(GetSelectQuery(), "a");

        sq.Select("a", "a_id");
        sq.Select(nameof(a), "a_id");
        sq.Select(() => a.a_id);
        sq.Select(() => a.a_id).As("id");

        Monitor.Log(sq);

        Assert.Equal(59, sq.GetTokens().ToList().Count);

        var sql = @"SELECT
    a.a_id,
    a.a_id,
    a.a_id,
    a.a_id AS id
FROM
    (
        SELECT
            a.a_id,
            a.text,
            a.value
        FROM
            RecordA AS a
            INNER JOIN RecordB AS b ON (a.a_id = b.a_id AND b.text = 'test')
    ) AS a";

        Assert.Equal(sql, sq.ToText(), true, true, true);
    }

    public SelectQuery<SubQueryRow> GetSelectQuery()
    {
        var sq = new SelectQuery<SubQueryRow>();
        var (from, a) = sq.FromAs<RecordA>("a");
        var b = from.InnerJoinAs<RecordB>(b => a.a_id == b.a_id && b.text == "test");

        sq.Select(() => a.a_id);
        sq.Select(() => a.text);
        sq.Select(() => a.value);

        /*
SELECT
    a.a_id,
    a.text,
    a.value
FROM
    RecordA AS a
    INNER JOIN RecordB AS b ON (a.a_id = b.a_id AND b.text = 'test')
         */

        return sq;
    }


    [Fact]
    public void QueryClassTest()
    {
        var sq = new SelectQuery();
        var (from, a) = sq.FromAs(TestQuery.Query, "a");

        sq.Select(() => a.a_id);
        sq.Select(() => a.value * 2).As("value");

        Monitor.Log(sq);

        Assert.Equal(45, sq.GetTokens().ToList().Count);

        var sql = @"SELECT
    a.a_id,
    a.value * 2 AS value
FROM
    (
        SELECT
            a.a_id,
            a.text,
            b.value
        FROM
            table_a AS a
            INNER JOIN table_b AS b ON a.a_id = b.a_id
    ) AS a";

        Assert.Equal(sql, sq.ToText(), true, true, true);
    }

    public record struct RecordA(int a_id, string text, int value, bool is_enabled, double rate, DateTime timestamp, Gender gender);

    public record struct RecordB(int a_id, int b_id, string text, int value);

    public record struct SubQueryRow(int a_id, string text, int value);

    public enum Gender
    {
        Male,
        Female,
        Other,
        Unknown
    }
}