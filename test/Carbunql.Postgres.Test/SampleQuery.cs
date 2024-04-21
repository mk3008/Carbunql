namespace Carbunql.Postgres.Test;

public static class SampleQuery
{
    public static string CommandText =>
"""
SELECT
    a.a_id,
    a.text,
    b.value
FROM
    table_a AS a
    INNER JOIN table_b AS b ON a.a_id = b.a_id
""";

    public static SelectQuery<Row> Query => GetQuery();

    private static SelectQuery<Row>? QueryCache;

    private static SelectQuery<Row> GetQuery()
    {
        if (QueryCache == null) QueryCache = new SelectQuery<Row>(CommandText);
        return QueryCache;
    }

    public readonly record struct Row(
        int a_id,
        string text,
        int value
    );
}
