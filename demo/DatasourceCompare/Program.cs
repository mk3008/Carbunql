using Carbunql;
using Carbunql.Fluent;

var actualQuery = """
    select
        s.sale_id
        , s.sale_date  as journal_date
        , s.shop_id
        , s.price
    from
        sale as s
    """;

var expectQuery = """
    select
        m.sale_id
        , j.journal_date
        , j.shop_id
        , j.price
    from
        journal as j
        inner join journal__map_sale as m on j.journal_id = m.journal_id
    """;

var actual = FluentTable.Create(actualQuery, "actual", "a");
var expect = FluentTable.Create(expectQuery, "expect", "e");

var keycolumns = new[] { "sale_id" };
var validationColumns = new[] { "journal_date", "shop_id", "price" };
var reverseColumns = new[] { "price" };

var dt = DateTime.Now;
var lowerLimit = new DateTime(dt.Year, dt.Month, dt.Day);

var ctq = GenerateMissingCreateTableQuery("missing", actual, expect, lowerLimit, keycolumns);
Console.WriteLine(ctq.ToText());
Console.WriteLine(";");

ctq = GenerateExcessCreateTableQuery("excess", actual, expect, lowerLimit, keycolumns, reverseColumns);
Console.WriteLine(ctq.ToText());
Console.WriteLine(";");

ctq = GenerateDifferentCreateTableQuery("different", actual, expect, lowerLimit, keycolumns, validationColumns, reverseColumns);
Console.WriteLine(ctq.ToText());
Console.WriteLine(";");

/*
-- GenerateMissingCreateTableQuery
CREATE TEMPORARY TABLE missing
AS
WITH
    actual AS (
        SELECT
            s.sale_id,
            s.sale_date AS journal_date,
            s.shop_id,
            s.price
        FROM
            sale AS s
    ),
    expect AS (
        SELECT
            m.sale_id,
            j.journal_date,
            j.shop_id,
            j.price
        FROM
            journal AS j
            INNER JOIN journal__map_sale AS m ON j.journal_id = m.journal_id
    )
SELECT
    a.sale_id,
    GREATEST(a.journal_date, '2024/09/01 0:00:00') AS journal_date,
    a.shop_id,
    a.price,
    NEXTVAL('journal_id_journal_id_seq') AS journal_id
FROM
    actual AS a
    LEFT JOIN expect AS e ON a.sale_id = e.sale_id
WHERE
    e.sale_id IS null
;
-- GenerateExcessCreateTableQuery
CREATE TEMPORARY TABLE excess
AS
WITH
    expect AS (
        SELECT
            m.sale_id,
            j.journal_date,
            j.shop_id,
            j.price
        FROM
            journal AS j
            INNER JOIN journal__map_sale AS m ON j.journal_id = m.journal_id
    ),
    actual AS (
        SELECT
            s.sale_id,
            s.sale_date AS journal_date,
            s.shop_id,
            s.price
        FROM
            sale AS s
    )
SELECT
    e.sale_id,
    GREATEST(e.journal_date, '2024/09/01 0:00:00') AS journal_date,
    e.shop_id,
    (e.price) * -1 AS price,
    NEXTVAL('journal_id_journal_id_seq') AS journal_id
FROM
    expect AS e
    LEFT JOIN actual AS a ON e.sale_id = a.sale_id
WHERE
    e.sale_id IS null
;
-- GenerateDifferentCreateTableQuery
CREATE TEMPORARY TABLE different
AS
WITH
    req AS (
        SELECT
            sale_id
        FROM
            request
    ),
    expect AS (
        SELECT
            m.sale_id,
            j.journal_date,
            j.shop_id,
            j.price
        FROM
            journal AS j
            INNER JOIN journal__map_sale AS m ON j.journal_id = m.journal_id
        WHERE
            EXISTS (
                SELECT
                    *
                FROM
                    req AS r
                WHERE
                    r.sale_id = m.sale_id
            )
    ),
    actual AS (
        SELECT
            s.sale_id,
            s.sale_date AS journal_date,
            s.shop_id,
            s.price
        FROM
            sale AS s
        WHERE
            EXISTS (
                SELECT
                    *
                FROM
                    req AS r
                WHERE
                    r.sale_id = s.sale_id
            )
    ),
    different AS (
        SELECT
            e.sale_id,
            NEXTVAL('journal_different_id_journal_different_id_seq') AS journal_different_id
        FROM
            expect AS e
            LEFT JOIN actual AS a ON e.sale_id = a.sale_id
        WHERE
            CASE
                WHEN e.journal_date IS NULL AND a.journal_date IS NULL THEN 0
                WHEN e.journal_date IS NOT NULL AND a.journal_date IS NULL THEN 1
                WHEN e.journal_date IS NULL AND a.journal_date IS NOT NULL THEN 1
                WHEN e.journal_date <> a.journal_date THEN 1
                ELSE 0
            END + CASE
                WHEN e.shop_id IS NULL AND a.shop_id IS NULL THEN 0
                WHEN e.shop_id IS NOT NULL AND a.shop_id IS NULL THEN 1
                WHEN e.shop_id IS NULL AND a.shop_id IS NOT NULL THEN 1
                WHEN e.shop_id <> a.shop_id THEN 1
                ELSE 0
            END + CASE
                WHEN e.price IS NULL AND a.price IS NULL THEN 0
                WHEN e.price IS NOT NULL AND a.price IS NULL THEN 1
                WHEN e.price IS NULL AND a.price IS NOT NULL THEN 1
                WHEN e.price <> a.price THEN 1
                ELSE 0
            END > 0
    )
-- Reverse
SELECT
    e.sale_id,
    GREATEST(e.journal_date, '2024/09/01 0:00:00') AS journal_date,
    e.shop_id,
    (e.price) * -1 AS price,
    NEXTVAL('journal_id_journal_id_seq') AS journal_id,
    d.journal_different_id
FROM
    different AS d
    INNER JOIN expect AS e ON d.sale_id = e.sale_id
UNION ALL
-- Current
SELECT
    a.sale_id,
    GREATEST(a.journal_date, '2024/09/01 0:00:00') AS journal_date,
    a.shop_id,
    a.price,
    NEXTVAL('journal_id_journal_id_seq') AS journal_id,
    d.journal_different_id
FROM
    different AS d
    INNER JOIN actual AS a ON d.sale_id = a.sale_id
;
*/

static CreateTableQuery GenerateMissingCreateTableQuery(
    string tableName,
    FluentTable actual,
    FluentTable expect,
    DateTime lowerLimit,
    IEnumerable<string> keyColumns)
{
    var q = new SelectQuery()
        .From(actual)
        .LeftJoin(actual, expect, keyColumns)
        .Equal(expect, keyColumns.First(), null)
        .SelectAll(actual)
        .Greatest(actual, "journal_date", lowerLimit)
        .SelectValue("nextval('journal_id_journal_id_seq')", "journal_id")
        .ToCreateTableQuery(tableName, true)
        .Comment(nameof(GenerateMissingCreateTableQuery));

    return q;
}

static CreateTableQuery GenerateExcessCreateTableQuery(
    string tableName,
    FluentTable actual,
    FluentTable expect,
    DateTime lowerLimit,
    IEnumerable<string> keyColumns,
    IEnumerable<string> reverseColumns)
{
    var q = new SelectQuery()
        .From(expect)
        .LeftJoin(expect, actual, keyColumns)
        .Equal(expect, keyColumns.First(), null)
        .SelectAll(expect)
        .Greatest(expect, "journal_date", lowerLimit)
        .ReverseSign(expect, reverseColumns)
        .SelectValue("nextval('journal_id_journal_id_seq')", "journal_id")
        .ToCreateTableQuery(tableName, true)
        .Comment(nameof(GenerateExcessCreateTableQuery));

    return q;
}

static CreateTableQuery GenerateDifferentCreateTableQuery(string tableName,
    FluentTable actual,
    FluentTable expect,
    DateTime lowerLimit,
    IEnumerable<string> keyColumns,
    IEnumerable<string> validateColumns,
    IEnumerable<string> reverseColumns)
{
    var differentQuery = new SelectQuery()
        .From(expect)
        .LeftJoin(expect, actual, keyColumns)
        .HasDifferent(expect, actual, validateColumns)
        .Select(expect, keyColumns)
        .SelectValue("nextval('journal_different_id_journal_different_id_seq')", "journal_different_id");

    var different = FluentTable.Create(differentQuery, "different", "d");

    var reverseQuery = GenerateDifferentQueryAsReverse(
        different,
        expect,
        lowerLimit,
        keyColumns,
        reverseColumns);

    var currentQuery = GenerateDifferentQueryAsCurrent(differentQuery,
        different,
        actual,
        lowerLimit,
        keyColumns);

    var request = FluentTable.Create("select sale_id from request", "req", "r");

    var query = reverseQuery.UnionAll(currentQuery)
        .Exists(request, ["sale_id"])
        .ToCreateTableQuery(tableName, true)
        .Comment(nameof(GenerateDifferentCreateTableQuery));

    return query;
}

static SelectQuery GenerateDifferentQueryAsReverse(
    FluentTable different,
    FluentTable expect,
    DateTime lowerLimit,
    IEnumerable<string> keyColumns,
    IEnumerable<string> reverseColumns)
{
    var reverseQuery = new SelectQuery()
        .From(different)
        .InnerJoin(different, expect, keyColumns)
        .SelectAll(expect)
        .Greatest(expect, "journal_date", lowerLimit)
        .ReverseSign(expect, reverseColumns)
        .SelectValue("nextval('journal_id_journal_id_seq')", "journal_id")
        .Select(different, "journal_different_id")
        .Comment("Reverse");

    return reverseQuery;
}

static SelectQuery GenerateDifferentQueryAsCurrent(SelectQuery differentQuery,
    FluentTable different,
    FluentTable actual,
    DateTime lowerLimit,
    IEnumerable<string> keyColumns)
{
    var currentQuery = new SelectQuery()
        .From(different)
        .InnerJoin(different, actual, keyColumns)
        .SelectAll(actual)
        .Greatest(actual, "journal_date", lowerLimit)
        .SelectValue("nextval('journal_id_journal_id_seq')", "journal_id")
        .Select(different, "journal_different_id")
        .Comment("Current");

    return currentQuery;
}
