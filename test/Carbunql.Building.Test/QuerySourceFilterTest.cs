﻿using System.Xml.Linq;
using Xunit.Abstractions;

namespace Carbunql.Building.Test;

public class QuerySourceFilterTest
{
    private readonly QueryCommandMonitor Monitor;

    public QuerySourceFilterTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void ShowDataSet()
    {
        var sql = @"
select 
    s.sale_id
    , s.store_id
    , s.price
from
    sale as s";

        var query = new SelectQuery(sql);
        query.GetQuerySources().ForEach(x =>
        {
            x.AddSourceComment($"Lv:{x.MaxLevel}, Columns:[{string.Join(", ", x.ColumnNames)}]");
        });
        Monitor.Log(query, exportTokens: false);

        var datasets = query.GetQuerySources().ToList();
        Monitor.Log(datasets);

        Assert.Single(datasets);
    }

    [Fact]
    public void EqualTest()
    {
        var sql = @"
SELECT
    s.sale_id,
    s.store_id,
    s.price
FROM
    /* Lv:1, Seq:1, Refs:0-1, Columns:[sale_id, store_id, price] */
    sale AS s";

        var query = new SelectQuery(sql);
        query.GetQuerySources().ForEach(x =>
        {
            x.AddSourceComment($"Lv:{x.MaxLevel}, Columns:[{string.Join(", ", x.ColumnNames)}]");
        });
        Monitor.Log(query, exportTokens: false);

        var column = "sale_id";
        var value = 1;

        query.GetQuerySources()
            .Where(ds => ds.ColumnNames.Contains(column))
            .GetRootsBySource()
            .ForEach(ds => ds.Query.Where(ds.Alias, column).Equal(value));

        Monitor.Log(query);

        var expect = @"SELECT
    s.sale_id,
    s.store_id,
    s.price
FROM
    /* Lv:1, Columns:[sale_id, store_id, price] */
    sale AS s
WHERE
    s.sale_id = 1";


        Assert.Equal(expect, query.ToText(), true, true, true);
    }

    [Fact]
    public void SubQueryTest()
    {
        var sql = @"
SELECT
    s2.sale_id,
    s2.store_id,
    s2.price
FROM
    /* Lv:1, Seq:1, Refs:0-1, Columns:[sale_id, store_id, price] */
    (
        SELECT
            s1.sale_id,
            s1.store_id,
            s1.price
        FROM
            /* Lv:2, Seq:1, Refs:0-1-2, Columns:[sale_id, store_id, price] */
            sale AS s1
    ) AS s2";

        var query = new SelectQuery(sql);
        query.GetQuerySources().ForEach(x =>
        {
            x.AddSourceComment($"Lv:{x.MaxLevel}, Columns:[{string.Join(", ", x.ColumnNames)}]");
        });
        Monitor.Log(query, exportTokens: false);

        var column = "sale_id";
        var value = 1;

        query.GetQuerySources()
            .Where(ds => ds.ColumnNames.Contains(column))
            .GetRootsBySource()
            .ForEach(ds => ds.Query.Where(ds.Alias, column).Equal(value));

        Monitor.Log(query);

        var expect = @"SELECT
    s2.sale_id,
    s2.store_id,
    s2.price
FROM
    /* Lv:1, Columns:[sale_id, store_id, price] */
    (
        SELECT
            s1.sale_id,
            s1.store_id,
            s1.price
        FROM
            /* Lv:2, Columns:[sale_id, store_id, price] */
            sale AS s1
        WHERE
            s1.sale_id = 1
    ) AS s2";

        Assert.Equal(expect, query.ToText(), true, true, true);
    }

    [Fact]
    public void InnerJoinTest()
    {
        var sql = @"
SELECT
    s.sale_id,
    s.store_id,
    s.price
FROM
    /* Lv:1, Seq:1, Refs:0-1, Columns:[sale_id, store_id, price] */
    sale AS s
    INNER JOIN
    /* Lv:1, Seq:2, Refs:0-2, Columns:[store_id] */
    store AS st ON s.store_id = st.store_id";

        var query = new SelectQuery(sql);
        query.GetQuerySources().ForEach(x =>
        {
            x.AddSourceComment($"Lv:{x.MaxLevel}, Columns:[{string.Join(", ", x.ColumnNames)}]");
        });
        Monitor.Log(query, exportTokens: false);

        var column = "store_id";
        var value = 1;

        query.GetQuerySources()
            .Where(ds => ds.ColumnNames.Contains(column))
            .GetRootsBySource()
            .ForEach(ds => ds.Query.Where(ds.Alias, column).Equal(value));

        Monitor.Log(query);

        //Other query sources within the same query may also be included in the search.
        var expect = @"SELECT
    s.sale_id,
    s.store_id,
    s.price
FROM
    /* Lv:1, Columns:[sale_id, store_id, price] */
    sale AS s
    INNER JOIN
    /* Lv:1, Columns:[store_id] */
    store AS st ON s.store_id = st.store_id
WHERE
    s.store_id = 1
    AND st.store_id = 1";

        Assert.Equal(expect, query.ToText(), true, true, true);
    }

    [Fact]
    public void InnerJoinTest_GetRootDataSetsByBranchGetRootDataSetsByQuery()
    {
        var sql = @"
SELECT
    s.sale_id,
    s.store_id,
    st.store_name,
    s.price
FROM
    /* Lv:1, Seq:1, Refs:0-1, Columns:[sale_id, store_id, price] */
    sale AS s
    INNER JOIN
    /* Lv:1, Seq:2, Refs:0-2, Columns:[store_name, store_id] */
    store AS st ON s.store_id = st.store_id";

        var query = new SelectQuery(sql);
        query.GetQuerySources().ForEach(x =>
        {
            x.AddSourceComment($"Lv:{x.MaxLevel}, Columns:[{string.Join(", ", x.ColumnNames)}]");
        });
        Monitor.Log(query, exportTokens: false);

        var datasets = query.GetQuerySources().ToList();
        Monitor.Log(datasets);

        var column = "store_id";
        var value = 1;

        //If you want to apply a single search condition to a query, write it like this.
        //However, when considering outer joins, it may be best to refrain from modifying it to this extent.
        query.GetQuerySources()
            .Where(ds => ds.ColumnNames.Contains(column))
            .GetRootsBySource()
            .GetRootsByQuery()
            .ForEach(ds => ds.Query.Where(ds.Alias, column).Equal(value));

        Monitor.Log(query);

        var expect = @"SELECT
    s.sale_id,
    s.store_id,
    st.store_name,
    s.price
FROM
    /* Lv:1, Columns:[sale_id, store_id, price] */
    sale AS s
    INNER JOIN
    /* Lv:1, Columns:[store_name, store_id] */
    store AS st ON s.store_id = st.store_id
WHERE
    s.store_id = 1";

        Assert.Equal(expect, query.ToText(), true, true, true);
    }

    [Fact]
    public void CTETest()
    {
        var sql = @"
WITH
    sx AS (
        SELECT
            s1.sale_id,
            s1.store_id,
            s1.price
        FROM
            /* Lv:2, Seq:1, Refs:0-1-2, Columns:[sale_id, store_id, price] */
            sale AS s1
    )
SELECT
    s2.sale_id,
    s2.store_id,
    s2.price
FROM
    /* Lv:1, Seq:1, Refs:0-1, Columns:[sale_id, store_id, price] */
    sx AS s2";

        var query = new SelectQuery(sql);
        query.GetQuerySources().ForEach(x =>
        {
            x.AddSourceComment($"Lv:{x.MaxLevel}, Columns:[{string.Join(", ", x.ColumnNames)}]");
        });
        Monitor.Log(query, exportTokens: false);

        var column = "store_id";
        var value = 1;

        query.GetQuerySources()
            .Where(ds => ds.ColumnNames.Contains(column))
            .GetRootsBySource()
            .ForEach(ds => ds.Query.Where(ds.Alias, column).Equal(value));

        Monitor.Log(query, exportTokens: false);

        var expect = @"WITH
    sx AS (
        SELECT
            s1.sale_id,
            s1.store_id,
            s1.price
        FROM
            /* Lv:2, Columns:[sale_id, store_id, price] */
            sale AS s1
        WHERE
            s1.store_id = 1
    )
SELECT
    s2.sale_id,
    s2.store_id,
    s2.price
FROM
    /* Lv:1, Columns:[sale_id, store_id, price] */
    sx AS s2";

        Assert.Equal(expect, query.ToText(), true, true, true);
    }

    [Fact]
    public void PracticalTest()
    {
        var sql = @"WITH
    monthly_sales AS (
        SELECT
            customer_id,
            DATE_TRUNC('month', sale_date) AS sale_month,
            SUM(sale_amount) AS total_sales
        FROM
            /* Index:3, MaxLv:2, Columns:[customer_id, sale_date, sale_amount] */
            /* Path:3-2-0 */
            sales
        GROUP BY
            customer_id,
            DATE_TRUNC('month', sale_date)
    )
SELECT
    c.customer_id,
    c.customer_name,
    ms.sale_month,
    COALESCE(ms.total_sales, 0) AS total_sales
FROM
    /* Index:1, MaxLv:1, Columns:[customer_id, customer_name] */
    /* Path:1-0 */
    customers AS c
    LEFT JOIN
    /* Index:2, MaxLv:1, Columns:[customer_id, sale_month, total_sales] */
    /* Path:2-0 */
    monthly_sales AS ms ON c.customer_id = ms.customer_id
ORDER BY
    c.customer_id,
    ms.sale_month";

        var query = new SelectQuery(sql);
        query.GetQuerySources().ForEach(x =>
        {
            x.AddSourceComment($"Index:{x.Index}, MaxLv:{x.MaxLevel}, Columns:[{string.Join(", ", x.ColumnNames)}]");
            x.ToTreePaths().ForEach(path => x.AddSourceComment($"Path:{string.Join("-", path)}"));
        });
        Monitor.Log(query, exportTokens: false);

        var column = "customer_id";
        var value = 1;

        query.GetQuerySources()
            .Where(ds => ds.ColumnNames.Contains(column))
            .GetRootsBySource()
            .ForEach(ds =>
            {
                ds.AddSourceComment("inject filter");
                ds.Query.Where(ds.Alias, column).Equal(value);
            });

        Monitor.Log(query, exportTokens: false);

        var expect = @"WITH
    monthly_sales AS (
        SELECT
            customer_id,
            DATE_TRUNC('month', sale_date) AS sale_month,
            SUM(sale_amount) AS total_sales
        FROM
            /* Index:3, MaxLv:2, Columns:[customer_id, sale_date, sale_amount] */
            /* Path:3-2-0 */
            /* inject filter */
            sales
        WHERE
            sales.customer_id = 1
        GROUP BY
            customer_id,
            DATE_TRUNC('month', sale_date)
    )
SELECT
    c.customer_id,
    c.customer_name,
    ms.sale_month,
    COALESCE(ms.total_sales, 0) AS total_sales
FROM
    /* Index:1, MaxLv:1, Columns:[customer_id, customer_name] */
    /* Path:1-0 */
    /* inject filter */
    customers AS c
    LEFT JOIN
    /* Index:2, MaxLv:1, Columns:[customer_id, sale_month, total_sales] */
    /* Path:2-0 */
    monthly_sales AS ms ON c.customer_id = ms.customer_id
WHERE
    c.customer_id = 1
ORDER BY
    c.customer_id,
    ms.sale_month";

        Assert.Equal(expect, query.ToText(), true, true, true);
    }

    [Fact]
    public void NotExistsInsertTest()
    {
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

        var query = new SelectQuery(sql)
            .OverrideSelect("journal_date", (source, item) => $"greatest({item}, {source.Query.AddParameter(":lower_limit", lower_limit)})")
            .AddNotExists(["sale_id"], "sale_journals")
            .AddWhere("request_timestamp", (source) => $"{source.Alias}.request_timestamp >= :lower_limit")
            .ToCTEQuery("final", "f")
            .ToInsertQuery("sale_journals");

        Monitor.Log(query, exportTokens: false);

        var expect = @"/*
  :lower_limit = '2024/07/20 0:00:00'
*/
INSERT INTO
    sale_journals(sale_id, store_id, journal_date, journal_price, request_timestamp)
WITH
    final AS (
        SELECT
            s.sale_id,
            s.store_id,
            GREATEST(s.sale_date, :lower_limit) AS journal_date,
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
            AND s.request_timestamp >= :lower_limit
    )
SELECT
    f.sale_id,
    f.store_id,
    f.journal_date,
    f.journal_price,
    f.request_timestamp
FROM
    final AS f";

        Assert.Equal(expect, query.ToText(), true, true, true);
    }

    [Fact]
    public void MergeTest()
    {

        var sql = """
    select
        s.sale_id
        , s.store_id
        , s.sale_date as journal_date
        , s.price + s.tax as journal_price
        , s.request_timestamp
    from
        sales as s
    """;

        var d = new DateTime(2024, 7, 1);
        var query = new SelectQuery()
            // Define expected values (current values) as a CTE named 'expect'
            .AddCTEQuery("""
        select distinct on(sale_id) 
            sale_id
            , store_id
            , journal_date
            , journal_price
        from
            sale_journals
        where
            journal_price > 0
        order by
            sale_id
            , sale_journal_id desc
    """, "expect")
            // Define the correct values as a CTE named 'actual'
            .AddCTEQuery(sql, "actual")
            // Compare the expected and correct values, and format the differences with red/black formatting
            .AddFrom("expect", "exp")
            .AddJoin("inner join", "actual", "act", "exp.sale_id = act.sale_id")
            .AddWhere("exp.journal_price <> act.journal_price")
            .OverrideSelect("journal_date", (source, item) => $"greatest({item}, {source.Query.AddParameter(":mod_date", d)})")
            .AddSelectAll("exp")
            .RemoveSelect("journal_price")
            .AddSelect("exp.journal_price * -1", "reverse_price")
            .AddSelect("act.journal_price * +1", "collect_price")
            // Define the query result with red/black formatting as a CTE named 'diff' and select it
            .ToCTEQuery("diff", "r")
            // Since we want to process red entries, the collect_price value is unnecessary; use reverse_price as the journal value
            .RemoveSelect("collect_price")
            .RenameSelect("reverse_price", "journal_price")
            // To process black entries, generate a UNION ALL query
            .AddSelectQuery("union all", owner =>
            {
                return new SelectQuery()
                    // Since we want to use the CTE 'diff', import the CTE information
                    .ImportCTEQueries(owner)
                    // Define the black entry selection query, similar to the red entry
                    .AddFrom("diff", "c")
                    .AddSelectAll("c")
                    .RemoveSelect("reverse_price")
                    .RenameSelect("collect_price", "journal_price");
            })
            // Convert to an insert query
            .ToInsertQuery("sale_journals");

        Monitor.Log(query, exportTokens: false);

        var expect = @"/*
  :mod_date = '2024/07/01 0:00:00'
*/
INSERT INTO
    sale_journals(sale_id, store_id, journal_date, journal_price)
WITH
    expect AS (
        SELECT DISTINCT ON (sale_id)
            sale_id,
            store_id,
            GREATEST(journal_date, :mod_date) AS journal_date,
            journal_price
        FROM
            sale_journals
        WHERE
            journal_price > 0
        ORDER BY
            sale_id,
            sale_journal_id DESC
    ),
    actual AS (
        SELECT
            s.sale_id,
            s.store_id,
            GREATEST(s.sale_date, :mod_date) AS journal_date,
            s.price + s.tax AS journal_price,
            s.request_timestamp
        FROM
            sales AS s
    ),
    diff AS (
        SELECT
            exp.sale_id,
            exp.store_id,
            exp.journal_date,
            exp.journal_price * -1 AS reverse_price,
            act.journal_price * +1 AS collect_price
        FROM
            expect AS exp
            INNER JOIN actual AS act ON exp.sale_id = act.sale_id
        WHERE
            exp.journal_price <> act.journal_price
    )
SELECT
    r.sale_id,
    r.store_id,
    r.journal_date,
    r.reverse_price AS journal_price
FROM
    diff AS r
UNION ALL
SELECT
    c.sale_id,
    c.store_id,
    c.journal_date,
    c.collect_price AS journal_price
FROM
    diff AS c";

        Assert.Equal(expect, query.ToText(), true, true, true);
    }
}
