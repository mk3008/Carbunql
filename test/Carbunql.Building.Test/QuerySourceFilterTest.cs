using System.Xml.Linq;
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
            x.AddSourceComment($"Lv:{x.Level}, Seq:{x.Sequence}, Refs:{string.Join("-", x.ReferencedIndexes)}, Columns:[{string.Join(", ", x.ColumnNames)}]");
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
            x.AddSourceComment($"Lv:{x.Level}, Seq:{x.Sequence}, Refs:{string.Join("-", x.ReferencedIndexes)}, Columns:[{string.Join(", ", x.ColumnNames)}]");
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
    /* Lv:1, Seq:1, Refs:0-1, Columns:[sale_id, store_id, price] */
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
            x.AddSourceComment($"Lv:{x.Level}, Seq:{x.Sequence}, Refs:{string.Join("-", x.ReferencedIndexes)}, Columns:[{string.Join(", ", x.ColumnNames)}]");
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
    /* Lv:1, Seq:1, Refs:0-1, Columns:[sale_id, store_id, price] */
    (
        SELECT
            s1.sale_id,
            s1.store_id,
            s1.price
        FROM
            /* Lv:2, Seq:1, Refs:0-1-2, Columns:[sale_id, store_id, price] */
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
            x.AddSourceComment($"Lv:{x.Level}, Seq:{x.Sequence}, Refs:{string.Join("-", x.ReferencedIndexes)}, Columns:[{string.Join(", ", x.ColumnNames)}]");
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
    /* Lv:1, Seq:1, Refs:0-1, Columns:[sale_id, store_id, price] */
    sale AS s
    INNER JOIN
    /* Lv:1, Seq:2, Refs:0-2, Columns:[store_id] */
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
            x.AddSourceComment($"Lv:{x.Level}, Seq:{x.Sequence}, Refs:{string.Join("-", x.ReferencedIndexes)}, Columns:[{string.Join(", ", x.ColumnNames)}]");
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
    /* Lv:1, Seq:1, Refs:0-1, Columns:[sale_id, store_id, price] */
    sale AS s
    INNER JOIN
    /* Lv:1, Seq:2, Refs:0-2, Columns:[store_name, store_id] */
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
            x.AddSourceComment($"Lv:{x.Level}, Seq:{x.Sequence}, Refs:{string.Join("-", x.ReferencedIndexes)}, Columns:[{string.Join(", ", x.ColumnNames)}]");
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
            /* Lv:2, Seq:1, Refs:0-1-2, Columns:[sale_id, store_id, price] */
            sale AS s1
        WHERE
            s1.store_id = 1
    )
SELECT
    s2.sale_id,
    s2.store_id,
    s2.price
FROM
    /* Lv:1, Seq:1, Refs:0-1, Columns:[sale_id, store_id, price] */
    sx AS s2";

        Assert.Equal(expect, query.ToText(), true, true, true);
    }

    [Fact]
    public void PracticalTest()
    {
        var sql = @"
WITH
    monthly_sales AS (
        SELECT
            customer_id,
            DATE_TRUNC('month', sale_date) AS sale_month,
            SUM(sale_amount) AS total_sales
        FROM
            /* Lv:2, Seq:1, Refs:0-2-3, Columns:[customer_id, sale_date, sale_amount] */
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
    /* Lv:1, Seq:1, Refs:0-1, Columns:[customer_id, customer_name] */
    customers AS c
    LEFT JOIN
    /* Lv:1, Seq:2, Refs:0-2, Columns:[customer_id, sale_month, total_sales] */
    monthly_sales AS ms ON c.customer_id = ms.customer_id
ORDER BY
    c.customer_id,
    ms.sale_month";

        var query = new SelectQuery(sql);
        query.GetQuerySources().ForEach(x =>
        {
            x.AddSourceComment($"Lv:{x.Level}, Seq:{x.Sequence}, Refs:{string.Join("-", x.ReferencedIndexes)}, Columns:[{string.Join(", ", x.ColumnNames)}]");
        });
        Monitor.Log(query, exportTokens: false);

        var column = "customer_id";
        var value = 1;

        query.GetQuerySources()
            .Where(ds => ds.ColumnNames.Contains(column))
            .GetRootsBySource()
            .ForEach(ds => ds.Query.Where(ds.Alias, column).Equal(value));

        Monitor.Log(query, exportTokens: false);

        var expect = @"WITH
    monthly_sales AS (
        SELECT
            customer_id,
            DATE_TRUNC('month', sale_date) AS sale_month,
            SUM(sale_amount) AS total_sales
        FROM
            /* Lv:2, Seq:1, Refs:0-2-3, Columns:[customer_id, sale_date, sale_amount] */
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
    /* Lv:1, Seq:1, Refs:0-1, Columns:[customer_id, customer_name] */
    customers AS c
    LEFT JOIN
    /* Lv:1, Seq:2, Refs:0-2, Columns:[customer_id, sale_month, total_sales] */
    monthly_sales AS ms ON c.customer_id = ms.customer_id
WHERE
    c.customer_id = 1
ORDER BY
    c.customer_id,
    ms.sale_month";

        Assert.Equal(expect, query.ToText(), true, true, true);
    }
}
