using Carbunql;
using Carbunql.Fluent;

var actual = """
    select
        s.sale_id
        , s.sale_date  as journal_date
        , s.shop_id
        , s.price
    from
        sale as s
    """;

var expect = """
    select
        m.sale_id
        , j.journal_date
        , j.shop_id
        , j.price
    from
        journal as j
        inner join journal__map_sale as m on j.journal_id = m.journal_id
    """;

var keycolumns = new[] { "sale_id" };
var validationColumns = new[] { "shop_id", "price" };
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
CREATE TEMPORARY TABLE missing
AS
WITH
    _actual AS (
        SELECT
            s.sale_id,
            s.sale_date AS journal_date,
            s.shop_id,
            s.price
        FROM
            sale AS s
    ),
    _expect AS (
        SELECT
            m.sale_id,
            j.journal_date,
            j.shop_id,
            j.price
        FROM
            journal AS j
            INNER JOIN journal__map_sale AS m ON j.journal_id = m.journal_id
    )
-- Missing
SELECT
    a.sale_id,
    GREATEST(a.journal_date, '2024/08/28 0:00:00') AS journal_date,
    a.shop_id,
    a.price,
    NEXTVAL('journal_id_journal_id_seq') AS journal_id
FROM
    _actual AS a
    LEFT JOIN _expect AS e ON a.sale_id = e.sale_id
WHERE
    e.sale_id IS null
;
CREATE TEMPORARY TABLE excess
AS
WITH
    _actual AS (
        SELECT
            s.sale_id,
            s.sale_date AS journal_date,
            s.shop_id,
            s.price
        FROM
            sale AS s
    ),
    _expect AS (
        SELECT
            m.sale_id,
            j.journal_date,
            j.shop_id,
            j.price
        FROM
            journal AS j
            INNER JOIN journal__map_sale AS m ON j.journal_id = m.journal_id
    )
-- Excess
SELECT
    e.sale_id,
    GREATEST(e.journal_date, '2024/08/28 0:00:00') AS journal_date,
    e.shop_id,
    (e.price) * -1 AS price,
    NEXTVAL('journal_id_journal_id_seq') AS journal_id
FROM
    _expect AS e
    LEFT JOIN _actual AS a ON e.sale_id = a.sale_id
WHERE
    e.sale_id IS null
;
CREATE TEMPORARY TABLE different
AS
WITH
    _actual AS (
        SELECT
            s.sale_id,
            s.sale_date AS journal_date,
            s.shop_id,
            s.price
        FROM
            sale AS s
    ),
    _expect AS (
        SELECT
            m.sale_id,
            j.journal_date,
            j.shop_id,
            j.price
        FROM
            journal AS j
            INNER JOIN journal__map_sale AS m ON j.journal_id = m.journal_id
    ),
    _different AS (
        SELECT
            e.sale_id,
            NEXTVAL('journal_different_id_journal_different_id_seq') AS journal_different_id
        FROM
            _expect AS e
            LEFT JOIN _actual AS a ON e.sale_id = a.sale_id
        WHERE
            CASE
                WHEN a.shop_id IS NULL AND e.shop_id IS NULL THEN 0
                WHEN a.shop_id IS NOT NULL AND e.shop_id IS NULL THEN 1
                WHEN a.shop_id IS NULL AND e.shop_id IS NOT NULL THEN 1
                WHEN a.shop_id <> e.shop_id THEN 1
                ELSE 0
            END + CASE
                WHEN a.price IS NULL AND e.price IS NULL THEN 0
                WHEN a.price IS NOT NULL AND e.price IS NULL THEN 1
                WHEN a.price IS NULL AND e.price IS NOT NULL THEN 1
                WHEN a.price <> e.price THEN 1
                ELSE 0
            END > 0
    )
-- Reverse
SELECT
    e.sale_id,
    GREATEST(e.journal_date, '2024/08/28 0:00:00') AS journal_date,
    e.shop_id,
    (e.price) * -1 AS price,
    NEXTVAL('journal_id_journal_id_seq') AS journal_id,
    d.journal_different_id
FROM
    _different AS d
    INNER JOIN _expect AS e ON d.sale_id = e.sale_id
UNION ALL
-- Current
SELECT
    a.sale_id,
    GREATEST(a.journal_date, '2024/08/28 0:00:00') AS journal_date,
    a.shop_id,
    a.price,
    NEXTVAL('journal_id_journal_id_seq') AS journal_id,
    d.journal_different_id
FROM
    _different AS d
    INNER JOIN _actual AS a ON d.sale_id = a.sale_id
;
 */

static CreateTableQuery GenerateMissingCreateTableQuery(
    string tableName,
    string actualQuery,
    string expectQuery,
    DateTime lowerLimit,
    IEnumerable<string> keyColumns)
{
    var actualCte = "_actual";
    var actualAlias = "a";
    var expectCte = "_expect";
    var expectAlias = "e";

    var q = new SelectQuery()
        .With(actualQuery, actualCte)
        .With(expectQuery, expectCte)
        .From(actualCte, actualAlias)
        .LeftJoin(expectCte, expectAlias, actualAlias, keyColumns)
        .Equal(expectAlias, keyColumns.First(), null)
        .SelectAll(actualAlias)
        .Greatest(actualAlias, "journal_date", lowerLimit)
        .SelectValue("nextval('journal_id_journal_id_seq')", "journal_id")
        .Comment("Missing")
        .ToCreateTableQuery(tableName, true);

    return q;
}

static CreateTableQuery GenerateExcessCreateTableQuery(
    string tableName,
    string actualQuery,
    string expectQuery,
    DateTime lowerLimit,
    IEnumerable<string> keyColumns,
    IEnumerable<string> reverseColumns)
{
    var actualCte = "_actual";
    var actualAlias = "a";
    var expectCte = "_expect";
    var expectAlias = "e";

    var q = new SelectQuery()
        .With(actualQuery, actualCte)
        .With(expectQuery, expectCte)
        .From(expectCte, expectAlias)
        .LeftJoin(actualCte, actualAlias, expectAlias, keyColumns)
        .Equal(expectAlias, keyColumns.First(), null)
        .SelectAll(expectAlias)
        .Greatest(expectAlias, "journal_date", lowerLimit)
        .ReverseSign(expectAlias, reverseColumns)
        .SelectValue("nextval('journal_id_journal_id_seq')", "journal_id")
        .Comment("Excess")
        .ToCreateTableQuery(tableName, true);

    return q;
}

static CreateTableQuery GenerateDifferentCreateTableQuery(string tableName,
    string actualQuery,
    string expectQuery,
    DateTime lowerLimit,
    IEnumerable<string> keyColumns,
    IEnumerable<string> validateColumns,
    IEnumerable<string> reverseColumns)
{
    var actualCte = "_actual";
    var actualAlias = "a";
    var expectCte = "_expect";
    var expectAlias = "e";
    var differentCte = "_different";
    var differentAlias = "d";

    var differentQuery = new SelectQuery()
        .With(actualQuery, actualCte)
        .With(expectQuery, expectCte)
        .From(expectCte, expectAlias)
        .LeftJoin(actualCte, actualAlias, expectAlias, keyColumns)
        .HasDifferent(actualAlias, expectAlias, validateColumns)
        .Select(expectAlias, keyColumns)
        .SelectValue("nextval('journal_different_id_journal_different_id_seq')", "journal_different_id");

    var reverseQuery = GenerateDifferentQueryAsReverse(
        differentQuery,
        differentCte,
        differentAlias,
        expectCte,
        expectAlias,
        lowerLimit,
        keyColumns,
        reverseColumns);

    var currentQuery = GenerateDifferentQueryAsCurrent(differentQuery,
        differentCte,
        differentAlias,
        actualCte,
        actualAlias,
        lowerLimit,
        keyColumns);

    var query = reverseQuery.UnionAll(_ => currentQuery)
        .Equal("sale_id", 1) //QuerySourceFilter
        .ToCreateTableQuery(tableName, true);

    return query;
}

static SelectQuery GenerateDifferentQueryAsReverse(
    SelectQuery differentQuery,
    string differentCte,
    string differentAlias,
    string expectCte,
    string expectAlias,
    DateTime lowerLimit,
    IEnumerable<string> keyColumns,
    IEnumerable<string> reverseColumns)
{
    var reverseQuery = new SelectQuery()
        .With(differentQuery, differentCte)
        .From(differentCte, differentAlias)
        .InnerJoin(expectCte, expectAlias, differentAlias, keyColumns)
        .SelectAll(expectAlias)
        .Greatest(expectAlias, "journal_date", lowerLimit)
        .ReverseSign(expectAlias, reverseColumns)
        .SelectValue("nextval('journal_id_journal_id_seq')", "journal_id")
        .Select(differentAlias, "journal_different_id")
        .Comment("Reverse");

    return reverseQuery;
}

static SelectQuery GenerateDifferentQueryAsCurrent(SelectQuery differentQuery,
    string differentCte,
    string differentAlias,
    string actualCte,
    string actualAlias,
    DateTime lowerLimit,
    IEnumerable<string> keyColumns)
{
    var currentQuery = new SelectQuery()
        .With(differentQuery, differentCte)
        .From(differentCte, differentAlias)
        .InnerJoin(actualCte, actualAlias, differentAlias, keyColumns)
        .SelectAll(actualAlias)
        .Greatest(actualAlias, "journal_date", lowerLimit)
        .SelectValue("nextval('journal_id_journal_id_seq')", "journal_id")
        .Select(differentAlias, "journal_different_id")
        .Comment("Current");

    return currentQuery;
}
