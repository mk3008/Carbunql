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
        Monitor.Log(query);

        var datasets = query.GetQuerySources().ToList();
        Monitor.Log(datasets);

        Assert.Single(datasets);
    }

    [Fact]
    public void EqualTest()
    {
        var sql = @"
select 
    s.sale_id
    , s.store_id
    , s.price
from
    sale as s";

        var query = new SelectQuery(sql);

        var column = "sale_id";
        var value = 1;

        query.GetQuerySources()
            .Where(ds => ds.ColumnNames.Contains(column))
            .GetRootsByBranch()
            .ForEach(ds => ds.Query.Where(ds.Alias, column).Equal(value));

        Monitor.Log(query);

        var expect = @"SELECT
    s.sale_id,
    s.store_id,
    s.price
FROM
    sale AS s
WHERE
    s.sale_id = 1";


        Assert.Equal(expect, query.ToText(), true, true, true);
    }

    [Fact]
    public void SubQueryTest()
    {
        var sql = @"
select 
    s2.sale_id
    , s2.store_id
    , s2.price
from
    (
        SELECT
            s1.sale_id,
            s1.store_id,
            s1.price
        FROM
            sale AS s1
    ) as s2";

        var query = new SelectQuery(sql);
        var datasets = query.GetQuerySources().ToList();
        Monitor.Log(datasets);


        var column = "sale_id";
        var value = 1;

        query.GetQuerySources()
            .Where(ds => ds.ColumnNames.Contains(column))
            .GetRootsByBranch()
            .ForEach(ds => ds.Query.Where(ds.Alias, column).Equal(value));

        Monitor.Log(query);

        var expect = @"SELECT
    s2.sale_id,
    s2.store_id,
    s2.price
FROM
    (
        SELECT
            s1.sale_id,
            s1.store_id,
            s1.price
        FROM
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
select 
    s.sale_id
    , s.store_id
    , s.price
from
    sale as s
    inner join store as st on s.store_id = st.store_id";

        var query = new SelectQuery(sql);

        var column = "store_id";
        var value = 1;

        query.GetQuerySources()
            .Where(ds => ds.ColumnNames.Contains(column))
            .GetRootsByBranch()
            .ForEach(ds => ds.Query.Where(ds.Alias, column).Equal(value));

        Monitor.Log(query);

        var expect = @"SELECT
    s.sale_id,
    s.store_id,
    s.price
FROM
    sale AS s
    INNER JOIN store AS st ON s.store_id = st.store_id
WHERE
    s.store_id = 1
    AND st.store_id = 1";

        Assert.Equal(expect, query.ToText(), true, true, true);
    }

    [Fact]
    public void InnerJoinTest_GetRootDataSetsByBranchGetRootDataSetsByQuery()
    {
        var sql = @"
select 
    s.sale_id
    , s.store_id
    , st.store_name
    , s.price
from
    sale as s
    inner join store as st on s.store_id = st.store_id";

        var query = new SelectQuery(sql);
        var datasets = query.GetQuerySources().ToList();
        Monitor.Log(datasets);

        var column = "store_id";
        var value = 1;

        query.GetQuerySources()
            .Where(ds => ds.ColumnNames.Contains(column))
            .GetRootsByBranch()
            .GetRootsByQuery()
            .ForEach(ds => ds.Query.Where(ds.Alias, column).Equal(value));

        Monitor.Log(query);

        var expect = @"SELECT
    s.sale_id,
    s.store_id,
    st.store_name,
    s.price
FROM
    sale AS s
    INNER JOIN store AS st ON s.store_id = st.store_id
WHERE
    s.store_id = 1";

        Assert.Equal(expect, query.ToText(), true, true, true);
    }

    [Fact]
    public void CTETest()
    {
        var sql = @"
with
sx as (
    SELECT
        s1.sale_id,
        s1.store_id,
        s1.price
    FROM
        sale AS s1
)
select 
    s2.sale_id
    , s2.store_id
    , s2.price
from
    sx as s2";

        var query = new SelectQuery(sql);

        var column = "store_id";
        var value = 1;

        query.GetQuerySources()
            .Where(ds => ds.ColumnNames.Contains(column))
            .GetRootsByBranch()
            .ForEach(ds => ds.Query.Where(ds.Alias, column).Equal(value));

        Monitor.Log(query);

        var expect = @"WITH
    sx AS (
        SELECT
            s1.sale_id,
            s1.store_id,
            s1.price
        FROM
            sale AS s1
        WHERE
            s1.store_id = 1
    )
SELECT
    s2.sale_id,
    s2.store_id,
    s2.price
FROM
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
            --DataSet:sales, Seq:1, Branch:2, Lv:2
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
    --DataSet:c,  Seq:1, Branch:1, Lv:1
    customers AS c
    --DataSet:ms, Seq:2, Branch:2, Lv:1  
    LEFT JOIN monthly_sales AS ms ON c.customer_id = ms.customer_id
ORDER BY
    c.customer_id,
    ms.sale_month";

        var query = new SelectQuery(sql);
        var datasets = query.GetQuerySources().ToList();
        Monitor.Log(datasets);

        var column = "customer_id";
        var value = 1;

        query.GetQuerySources()
            .Where(ds => ds.ColumnNames.Contains(column))
            .GetRootsByBranch()
            .ForEach(ds => ds.Query.Where(ds.Alias, column).Equal(value));

        Monitor.Log(query);

        var expect = @"WITH
    monthly_sales AS (
        SELECT
            customer_id,
            DATE_TRUNC('month', sale_date) AS sale_month,
            SUM(sale_amount) AS total_sales
        FROM
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
    customers AS c
    LEFT JOIN monthly_sales AS ms ON c.customer_id = ms.customer_id
WHERE
    c.customer_id = 1
ORDER BY
    c.customer_id,
    ms.sale_month";

        Assert.Equal(expect, query.ToText(), true, true, true);
    }
}
