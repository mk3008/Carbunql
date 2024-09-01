using Carbunql;
using Carbunql.Building;
using Carbunql.Fluent;

var sql = """
    select
        s.sale_id
        , s.store_id
        , s.sale_date as journal_date
        , s.price as journal_price
        , s.request_timestamp
    from
        sales as s
    """;

var lower_limit = new DateTime(2024, 7, 20);

var query = SelectQuery.Parse(sql)
    .OverrideSelect("journal_date", (source, item) => $"greatest({item}, {source.Query.AddParameter(":lower_limit", lower_limit)})")
    .NotExists(["sale_id"], "sale_journals")
    .GreaterThanOrEqual("request_timestamp", ":lower_limit")
    .ToCteQuery("final", "f", out var final)
    .SelectAll(final)
    .ToInsertQuery("sale_journals");

Console.WriteLine(query.ToText());
Console.WriteLine(";");

/*
INSERT INTO
    sale_journals(sale_id, store_id, journal_date, journal_price, request_timestamp)
WITH
    final AS (
        SELECT
            s.sale_id,
            s.store_id,
            GREATEST(s.sale_date, Carbunql.SelectQuery) AS journal_date,
            s.price AS journal_price,
            s.request_timestamp
        FROM
            sales AS s
        WHERE
            NOT EXISTS (
                SELECT
                    *
                FROM
                    sale_journals AS x
                WHERE
                    x.sale_id = s.sale_id
            )
            AND :lower_limit <= s.request_timestamp
    )
SELECT
    f.sale_id,
    f.store_id,
    f.journal_date,
    f.journal_price,
    f.request_timestamp
FROM
    final AS f
;
*/