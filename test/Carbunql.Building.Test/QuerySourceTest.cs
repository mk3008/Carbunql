using Xunit.Abstractions;

namespace Carbunql.Building.Test;

public class QuerySourceTest
{
    private readonly QueryCommandMonitor Monitor;

    public QuerySourceTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void Simple()
    {
        //When referencing a physical table, the column names referenced from the select clause, etc. are identified.
        //The DBMS does not reference it, so it cannot recognize unused columns.
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
            x.AddSourceComment($"Index:{x.Index}, MaxLv:{x.MaxLevel}, Columns:[{string.Join(", ", x.ColumnNames)}]");
            x.ToTreePaths().ForEach(path => x.AddSourceComment($"Path:{string.Join("-", path)}"));
        });
        var sources = query.GetQuerySources().ToList();
        Monitor.Log(sources);
        Monitor.Log(query);

        Assert.Single(sources);

        var ds = sources[0];
        Assert.Equal(1, ds.Index);
        Assert.Equal(1, ds.MaxLevel);
        Assert.Equal("s", ds.Alias);

        var columns = ds.ColumnNames.ToList();
        Assert.Equal(3, columns.Count());
        Assert.Equal("sale_id", columns[0]);
        Assert.Equal("store_id", columns[1]);
        Assert.Equal("price", columns[2]);
    }

    [Fact]
    public void WhereTest()
    {
        var sql = @"
select 
    s.store_id
from
    sale as s
where
    s.store_id = 1";

        var query = new SelectQuery(sql);
        query.GetQuerySources().ForEach(x =>
        {
            x.AddSourceComment($"Index:{x.Index}, MaxLv:{x.MaxLevel}, Columns:[{string.Join(", ", x.ColumnNames)}]");
            x.ToTreePaths().ForEach(path => x.AddSourceComment($"Path:{string.Join("-", path)}"));
        });
        var sources = query.GetQuerySources().ToList();
        Monitor.Log(sources);
        Monitor.Log(query);

        Assert.Single(sources);

        var ds = sources[0];
        Assert.Equal(1, ds.Index);
        Assert.Equal(1, ds.MaxLevel);
        Assert.Equal("s", ds.Alias);

        var columns = ds.ColumnNames.ToList();
        Assert.Single(columns);
        Assert.Equal("store_id", columns[0]);
    }

    [Fact]
    public void OrderTest()
    {
        var sql = @"
select 
    s.store_id
from
    sale as s
order by
    s.store_id = 1";

        var query = new SelectQuery(sql);
        query.GetQuerySources().ForEach(x =>
        {
            x.AddSourceComment($"Index:{x.Index}, MaxLv:{x.MaxLevel}, Columns:[{string.Join(", ", x.ColumnNames)}]");
            x.ToTreePaths().ForEach(path => x.AddSourceComment($"Path:{string.Join("-", path)}"));
        });
        var sources = query.GetQuerySources().ToList();
        Monitor.Log(sources);
        Monitor.Log(query);

        Assert.Single(sources);

        var ds = sources[0];
        Assert.Equal(1, ds.Index);
        Assert.Equal(1, ds.MaxLevel);
        Assert.Equal("s", ds.Alias);

        var columns = ds.ColumnNames.ToList();
        Assert.Single(columns);
        Assert.Equal("store_id", columns[0]);
    }


    [Fact]
    public void GroupTest()
    {
        var sql = @"
select 
    1
from
    sale as s
group by
    s.store_id";

        var query = new SelectQuery(sql);
        query.GetQuerySources().ForEach(x =>
        {
            x.AddSourceComment($"Index:{x.Index}, MaxLv:{x.MaxLevel}, Columns:[{string.Join(", ", x.ColumnNames)}]");
            x.ToTreePaths().ForEach(path => x.AddSourceComment($"Path:{string.Join("-", path)}"));
        });
        var sources = query.GetQuerySources().ToList();
        Monitor.Log(sources);
        Monitor.Log(query);

        Assert.Single(sources);

        var ds = sources[0];
        Assert.Equal(1, ds.Index);
        Assert.Equal(1, ds.MaxLevel);
        Assert.Equal("s", ds.Alias);

        var columns = ds.ColumnNames.ToList();
        Assert.Single(columns);
        Assert.Equal("store_id", columns[0]);
    }

    [Fact]
    public void HavingTest()
    {
        var sql = @"
select 
    1
from
    sale as s
group by
    s.store_id
having
    sum(s.price) = 0";

        var query = new SelectQuery(sql);
        query.GetQuerySources().ForEach(x =>
        {
            x.AddSourceComment($"Index:{x.Index}, MaxLv:{x.MaxLevel}, Columns:[{string.Join(", ", x.ColumnNames)}]");
            x.ToTreePaths().ForEach(path => x.AddSourceComment($"Path:{string.Join("-", path)}"));
        });
        var sources = query.GetQuerySources().ToList();
        Monitor.Log(sources);
        Monitor.Log(query);

        Assert.Single(sources);

        var ds = sources[0];
        Assert.Equal(1, ds.Index);
        Assert.Equal(1, ds.MaxLevel);
        Assert.Equal("s", ds.Alias);

        var columns = ds.ColumnNames.ToList();
        Assert.Equal(2, columns.Count());
        Assert.Equal("store_id", columns[0]);
        Assert.Equal("price", columns[1]);
    }

    [Fact]
    public void OmitAliasTest()
    {
        // If the table alias is omitted,
        // analysis can only be performed if there is no table join.
        var sql = @"
select 
    s.store_id
from
    sale as s
where
    --Table aliases are omitted, 
    --but they can be identified because there is no table join.
    store_id = 1";

        var query = new SelectQuery(sql);
        query.GetQuerySources().ForEach(x =>
        {
            x.AddSourceComment($"Index:{x.Index}, MaxLv:{x.MaxLevel}, Columns:[{string.Join(", ", x.ColumnNames)}]");
            x.ToTreePaths().ForEach(path => x.AddSourceComment($"Path:{string.Join("-", path)}"));
        });
        var sources = query.GetQuerySources().ToList();
        Monitor.Log(sources);
        Monitor.Log(query);

        Assert.Single(sources);

        var ds = sources[0];
        Assert.Equal(1, ds.Index);
        Assert.Equal(1, ds.MaxLevel);
        Assert.Equal("s", ds.Alias);

        var columns = ds.ColumnNames.ToList();
        Assert.Single(columns);
        Assert.Equal("store_id", columns[0]);
    }

    [Fact]
    public void JoinTest_OmitAlias()
    {
        //If you are using table joins, do not omit the table alias
        var sql = @"
select 
    s.sale_id
from
    sale as s
    inner join store as st on s.store_id = st.store_id
where
    --An example of a statement that omits the table alias name.
    --Although the SQL is correct, it cannot be parsed by the library.
    sale_id = 1";

        var query = new SelectQuery(sql);

        var exception = Assert.Throws<InvalidProgramException>(() =>
        {
            var sources = query.GetQuerySources().ToList();
            Monitor.Log(sources);
        });

        Assert.Contains("There are columns whose table alias names cannot be parsed: sale_id.", exception.Message);
    }

    [Fact]
    public void SubQueryTest()
    {
        //If you are using a subquery,
        //you get the participating columns from the query's select clause.
        var sql = @"
SELECT
    d.sale_id
FROM
    /* Lv:1, Seq:1, Refs:0-1, Columns:[sale_id, store_id, price] */
    (
        SELECT
            s.sale_id,
            s.store_id,
            s.quantity * s.amount AS price
        FROM
            /* Lv:2, Seq:1, Refs:0-1-2, Columns:[sale_id, store_id, quantity, amount] */
            sale AS s
    ) AS d";

        var query = new SelectQuery(sql);
        query.GetQuerySources().ForEach(x =>
        {
            x.AddSourceComment($"Index:{x.Index}, MaxLv:{x.MaxLevel}, Columns:[{string.Join(", ", x.ColumnNames)}]");
            x.ToTreePaths().ForEach(path => x.AddSourceComment($"Path:{string.Join("-", path)}"));
        });
        var sources = query.GetQuerySources().ToList();
        Monitor.Log(sources);
        Monitor.Log(query);

        Assert.Equal(2, sources.Count);

        //index:0
        var ds = sources[0];
        Assert.Equal(2, ds.Index);
        Assert.Equal(2, ds.MaxLevel);
        Assert.Equal("s", ds.Alias);

        var columns = ds.ColumnNames.ToList();
        Assert.Equal(4, columns.Count());
        Assert.Equal("sale_id", columns[0]);
        Assert.Equal("store_id", columns[1]);
        Assert.Equal("quantity", columns[2]);
        Assert.Equal("amount", columns[3]);

        //index:1
        ds = sources[1];
        Assert.Equal(1, ds.Index);
        Assert.Equal(1, ds.MaxLevel);
        Assert.Equal("d", ds.Alias);

        columns = ds.ColumnNames.ToList();
        Assert.Equal(3, columns.Count());
        Assert.Equal("sale_id", columns[0]);
        Assert.Equal("store_id", columns[1]);
        Assert.Equal("price", columns[2]);// The column "price" is recognizable.
    }

    [Fact]
    public void CTETest()
    {
        //If you are using a CTE,
        //you get the participating columns from the query's select clause.
        var sql = @"
WITH
monthly_sales AS (
    SELECT
        store_id,
        product_id,
        DATE_TRUNC('month', sales_date) AS month,
        SUM(sales_amount) AS total_sales
    FROM
        sales
    GROUP BY
        store_id,
        product_id,
        DATE_TRUNC('month', sales_date)
),
total_monthly_sales AS (
    SELECT
        store_id,
        month,
        SUM(total_sales) AS total_sales
    FROM
        monthly_sales
    GROUP BY
        store_id,
        month
)
SELECT
    ms.store_id,
    ms.product_id,
    ms.month,
    ms.total_sales,
    tms.total_sales AS total_monthly_sales,
    (ms.total_sales::FLOAT / tms.total_sales) * 100 AS sales_percentage
FROM
    monthly_sales ms
INNER JOIN
    total_monthly_sales tms ON ms.store_id = tms.store_id AND ms.month = tms.month
ORDER BY
    ms.month,
    ms.product_id";

        var expect = @"WITH
    monthly_sales AS (
        SELECT
            store_id,
            product_id,
            DATE_TRUNC('month', sales_date) AS month,
            SUM(sales_amount) AS total_sales
        FROM
            /* Index:2, MaxLv:3, Columns:[store_id, product_id, sales_date, sales_amount] */
            /* Path:2-1-0 */
            /* Path:2-4-3-0 */
            sales
        GROUP BY
            store_id,
            product_id,
            DATE_TRUNC('month', sales_date)
    ),
    total_monthly_sales AS (
        SELECT
            store_id,
            month,
            SUM(total_sales) AS total_sales
        FROM
            /* Index:4, MaxLv:2, Columns:[store_id, product_id, month, total_sales] */
            /* Path:4-3-0 */
            monthly_sales
        GROUP BY
            store_id,
            month
    )
SELECT
    ms.store_id,
    ms.product_id,
    ms.month,
    ms.total_sales,
    tms.total_sales AS total_monthly_sales,
    (ms.total_sales::FLOAT / tms.total_sales) * 100 AS sales_percentage
FROM
    /* Index:1, MaxLv:1, Columns:[store_id, product_id, month, total_sales] */
    /* Path:1-0 */
    monthly_sales AS ms
    INNER JOIN
    /* Index:3, MaxLv:1, Columns:[store_id, month, total_sales] */
    /* Path:3-0 */
    total_monthly_sales AS tms ON ms.store_id = tms.store_id AND ms.month = tms.month
ORDER BY
    ms.month,
    ms.product_id";

        var query = new SelectQuery(sql);
        query.GetQuerySources().ForEach(x =>
        {
            x.AddSourceComment($"Index:{x.Index}, MaxLv:{x.MaxLevel}, Columns:[{string.Join(", ", x.ColumnNames)}]");
            x.ToTreePaths().ForEach(path => x.AddSourceComment($"Path:{string.Join("-", path)}"));
        });
        var sources = query.GetQuerySources().ToList();
        Monitor.Log(sources);
        Monitor.Log(query);

        Assert.Equal(expect, query.ToText());

        Assert.Equal(4, sources.Count);
    }

    [Fact]
    public void UnionTest()
    {
        //If you are using a CTE,
        //you get the participating columns from the query's select clause.
        var sql = @"
SELECT
    s.sale_id,
    s.store_id,
    s.price
FROM
    /* Lv:1, Seq:1, Refs:0-1, Columns:[sale_id, store_id, price] */
    sale AS s
WHERE
    s.sale_id = 1
UNION ALL
SELECT
    s.sale_id,
    s.store_id,
    s.price
FROM
    /* Lv:1, Seq:2, Refs:0-2, Columns:[sale_id, store_id, price] */
    sale AS s
WHERE
    s.sale_id = 2
UNION ALL
SELECT
    s.sale_id,
    s.store_id,
    s.price
FROM
    /* Lv:1, Seq:3, Refs:0-3, Columns:[sale_id, store_id, price] */
    sale AS s
WHERE
    s.sale_id = 3";

        var query = new SelectQuery(sql);
        query.GetQuerySources().ForEach(x =>
        {
            x.AddSourceComment($"Index:{x.Index}, MaxLv:{x.MaxLevel}, Columns:[{string.Join(", ", x.ColumnNames)}]");
            x.ToTreePaths().ForEach(path => x.AddSourceComment($"Path:{string.Join("-", path)}"));
        });
        var sources = query.GetQuerySources().ToList();
        Monitor.Log(sources);
        Monitor.Log(query);

        Assert.Equal(3, sources.Count);

        //index:0
        var ds = sources[0];
        Assert.Equal(1, ds.Index);
        Assert.Equal(1, ds.MaxLevel);
        Assert.Equal("s", ds.Alias);

        var columns = ds.ColumnNames.ToList();
        Assert.Equal(3, columns.Count());
        Assert.Equal("sale_id", columns[0]);
        Assert.Equal("store_id", columns[1]);
        Assert.Equal("price", columns[2]);

        //index:1
        ds = sources[1];
        Assert.Equal(2, ds.Index);
        Assert.Equal(1, ds.MaxLevel);
        Assert.Equal("s", ds.Alias);

        columns = ds.ColumnNames.ToList();
        Assert.Equal(3, columns.Count());
        Assert.Equal("sale_id", columns[0]);
        Assert.Equal("store_id", columns[1]);
        Assert.Equal("price", columns[2]);

        //index:2
        ds = sources[2];
        Assert.Equal(3, ds.Index);
        Assert.Equal(1, ds.MaxLevel);
        Assert.Equal("s", ds.Alias);

        columns = ds.ColumnNames.ToList();
        Assert.Equal(3, columns.Count());
        Assert.Equal("sale_id", columns[0]);
        Assert.Equal("store_id", columns[1]);
        Assert.Equal("price", columns[2]);
    }

    [Fact]
    public void WildCardTest()
    {

        var sql = @"
SELECT
    *
FROM
    /* Lv:1, Seq:1, Refs:0-1, Columns:[sale_id, store_id, price] */
    (
        SELECT
            *
        FROM
            /* Lv:2, Seq:1, Refs:0-1-2, Columns:[sale_id, store_id, price] */
            (
                SELECT
                    s.sale_id,
                    s.store_id,
                    s.quantity * s.amount AS price
                FROM
                    /* Lv:3, Seq:1, Refs:0-1-2-3, Columns:[sale_id, store_id, quantity, amount] */
                    sale AS s
            ) AS d
    ) AS q";

        var query = new SelectQuery(sql);
        query.GetQuerySources().ForEach(x =>
        {
            x.AddSourceComment($"Index:{x.Index}, MaxLv:{x.MaxLevel}, Columns:[{string.Join(", ", x.ColumnNames)}]");
            x.ToTreePaths().ForEach(path => x.AddSourceComment($"Path:{string.Join("-", path)}"));
        });
        var sources = query.GetQuerySources().ToList();
        Monitor.Log(sources);
        Monitor.Log(query);

        Assert.Equal(3, sources.Count);

        //index:0
        var ds = sources[0];
        Assert.Equal(3, ds.Index);
        Assert.Equal(3, ds.MaxLevel);
        Assert.Equal("s", ds.Alias);

        var columns = ds.ColumnNames.ToList();
        Assert.Equal(4, columns.Count());
        Assert.Equal("sale_id", columns[0]);
        Assert.Equal("store_id", columns[1]);
        Assert.Equal("quantity", columns[2]);
        Assert.Equal("amount", columns[3]);

        //index:1
        ds = sources[1];
        Assert.Equal(2, ds.Index);
        Assert.Equal(2, ds.MaxLevel);
        Assert.Equal("d", ds.Alias);

        columns = ds.ColumnNames.ToList();
        Assert.Equal(3, columns.Count());
        Assert.Equal("sale_id", columns[0]);
        Assert.Equal("store_id", columns[1]);
        Assert.Equal("price", columns[2]);

        //index:2
        ds = sources[2];
        Assert.Equal(1, ds.Index);
        Assert.Equal(1, ds.MaxLevel);
        Assert.Equal("q", ds.Alias);

        columns = ds.ColumnNames.ToList();
        Assert.Equal(3, columns.Count());
        Assert.Equal("sale_id", columns[0]);
        Assert.Equal("store_id", columns[1]);
        Assert.Equal("price", columns[2]);
    }

    [Fact]
    public void AliasWildCardTest()
    {

        var sql = @"
SELECT
    q.*
FROM
    /* Lv:1, Seq:1, Refs:0-1, Columns:[sale_id, store_id, price] */
    (
        SELECT
            d.*
        FROM
            /* Lv:2, Seq:1, Refs:0-1-2, Columns:[sale_id, store_id, price] */
            (
                SELECT
                    s.sale_id,
                    s.store_id,
                    s.quantity * s.amount AS price
                FROM
                    /* Lv:3, Seq:1, Refs:0-1-2-3, Columns:[sale_id, store_id, quantity, amount] */
                    sale AS s
            ) AS d
    ) AS q
    CROSS JOIN
    /* Lv:1, Seq:2, Refs:0-4, Columns:[store_id] */
    store AS st
WHERE
    st.store_id = 1
";

        var query = new SelectQuery(sql);
        query.GetQuerySources().ForEach(x =>
        {
            x.AddSourceComment($"Index:{x.Index}, MaxLv:{x.MaxLevel}, Columns:[{string.Join(", ", x.ColumnNames)}]");
            x.ToTreePaths().ForEach(path => x.AddSourceComment($"Path:{string.Join("-", path)}"));
        });
        var sources = query.GetQuerySources().ToList();
        Monitor.Log(sources);
        Monitor.Log(query);

        Assert.Equal(4, sources.Count);

        //index:0
        var ds = sources[0];
        Assert.Equal(3, ds.Index);
        Assert.Equal(3, ds.MaxLevel);
        Assert.Equal("s", ds.Alias);

        var columns = ds.ColumnNames.ToList();
        Assert.Equal(4, columns.Count());
        Assert.Equal("sale_id", columns[0]);
        Assert.Equal("store_id", columns[1]);
        Assert.Equal("quantity", columns[2]);
        Assert.Equal("amount", columns[3]);

        //index:1
        ds = sources[1];
        Assert.Equal(2, ds.Index);
        Assert.Equal(2, ds.MaxLevel);
        Assert.Equal("d", ds.Alias);

        columns = ds.ColumnNames.ToList();
        Assert.Equal(3, columns.Count());
        Assert.Equal("sale_id", columns[0]);
        Assert.Equal("store_id", columns[1]);
        Assert.Equal("price", columns[2]);

        //index:2
        ds = sources[2];
        Assert.Equal(1, ds.Index);
        Assert.Equal(1, ds.MaxLevel);
        Assert.Equal("q", ds.Alias);

        columns = ds.ColumnNames.ToList();
        Assert.Equal(3, columns.Count());
        Assert.Equal("sale_id", columns[0]);
        Assert.Equal("store_id", columns[1]);
        Assert.Equal("price", columns[2]);

        //index:3
        ds = sources[3];
        Assert.Equal(4, ds.Index);
        Assert.Equal(1, ds.MaxLevel);
        Assert.Equal("st", ds.Alias);

        columns = ds.ColumnNames.ToList();
        Assert.Single(columns);
        Assert.Equal("store_id", columns[0]);
    }

    [Fact]
    public void CTEJoinTest()
    {
        var sql = @"
WITH
    monthly_sales AS (
        SELECT
            customer_id,
            DATE_TRUNC('month', sale_date) AS sale_month,
            SUM(sale_amount) AS total_sales
        FROM
            /* Lv:3, Seq:1, Refs:0-1-3-4, Columns:[customer_id, sale_date, sale_amount] */
            sales
        GROUP BY
            customer_id,
            DATE_TRUNC('month', sale_date)
    ),
    report AS (
        SELECT
            c.customer_id,
            c.customer_name,
            ms.sale_month,
            COALESCE(ms.total_sales, 0) AS total_sales
        FROM
            /* Lv:2, Seq:1, Refs:0-1-2, Columns:[customer_id, customer_name] */
            customers AS c
            LEFT JOIN
            /* Lv:2, Seq:2, Refs:0-1-3, Columns:[customer_id, sale_month, total_sales] */
            monthly_sales AS ms ON c.customer_id = ms.customer_id
    )
SELECT
    *
FROM
    /* Lv:1, Seq:1, Refs:0-1, Columns:[customer_id, customer_name, sale_month, total_sales] */
    report AS r
ORDER BY
    r.customer_id,
    r.sale_month";

        var query = new SelectQuery(sql);
        query.GetQuerySources().ForEach(x =>
        {
            x.AddSourceComment($"Index:{x.Index}, MaxLv:{x.MaxLevel}, Columns:[{string.Join(", ", x.ColumnNames)}]");
            x.ToTreePaths().ForEach(path => x.AddSourceComment($"Path:{string.Join("-", path)}"));
        });
        var sources = query.GetQuerySources().ToList();
        Monitor.Log(sources);
        Monitor.Log(query);

        Assert.Equal(4, sources.Count);

        //index:0
        var ds = sources[0];
        Assert.Equal(2, ds.Index);
        Assert.Equal(2, ds.MaxLevel);
        Assert.Equal("c", ds.Alias);

        var columns = ds.ColumnNames.ToList();
        Assert.Equal(2, columns.Count());
        Assert.Equal("customer_id", columns[0]);
        Assert.Equal("customer_name", columns[1]);

        //index:1
        ds = sources[1];
        Assert.Equal(4, ds.Index);
        Assert.Equal(3, ds.MaxLevel);
        Assert.Equal("sales", ds.Alias);

        columns = ds.ColumnNames.ToList();
        Assert.Equal(3, columns.Count());
        Assert.Equal("customer_id", columns[0]);
        Assert.Equal("sale_date", columns[1]);
        Assert.Equal("sale_amount", columns[2]);

        //index:2
        ds = sources[2];
        Assert.Equal(3, ds.Index);
        Assert.Equal(2, ds.MaxLevel);
        Assert.Equal("ms", ds.Alias);

        columns = ds.ColumnNames.ToList();
        Assert.Equal(3, columns.Count());
        Assert.Equal("customer_id", columns[0]);
        Assert.Equal("sale_month", columns[1]);
        Assert.Equal("total_sales", columns[2]);

        //index:3
        ds = sources[3];
        Assert.Equal(1, ds.Index);
        Assert.Equal(1, ds.MaxLevel);
        Assert.Equal("r", ds.Alias);

        columns = ds.ColumnNames.ToList();
        Assert.Equal(4, columns.Count());
        Assert.Equal("customer_id", columns[0]);
        Assert.Equal("customer_name", columns[1]);
        Assert.Equal("sale_month", columns[2]);
        Assert.Equal("total_sales", columns[3]);
    }

    [Fact]
    public void Composite()
    {
        var sql = @"
WITH
    dat (
        line_id, name, unit_price, quantity, tax_rate
    ) AS (
        VALUES
            (1, 'apple', 105, 5, 0.07),
            (2, 'orange', 203, 3, 0.07),
            (3, 'banana', 233, 9, 0.07),
            (4, 'tea', 309, 7, 0.08),
            (5, 'coffee', 555, 9, 0.08),
            (6, 'cola', 456, 2, 0.08)
    ),
    detail AS (
        SELECT
            q.*,
            TRUNC(q.price * (1 + q.tax_rate)) - q.price AS tax,
            q.price * (1 + q.tax_rate) - q.price AS raw_tax
        FROM
            /* Lv:5, Seq:1, Refs:0-1-2-3-4-5, Columns:[price, line_id, name, unit_price, quantity, tax_rate] */
            /* Lv:6, Seq:1, Refs:0-1-2-3-6-7-8, Columns:[price, line_id, name, unit_price, quantity, tax_rate] */
            (
                SELECT
                    dat.*,
                    (dat.unit_price * dat.quantity) AS price
                FROM
                    /* Lv:6, Seq:1, Refs:0-1-2-3-4-5, Columns:[line_id, name, unit_price, quantity, tax_rate] */
                    /* Lv:7, Seq:1, Refs:0-1-2-3-6-7-8, Columns:[line_id, name, unit_price, quantity, tax_rate] */
                    dat
            ) AS q
    ),
    tax_summary AS (
        SELECT
            d.tax_rate,
            TRUNC(SUM(raw_tax)) AS total_tax
        FROM
            /* Lv:5, Seq:1, Refs:0-1-2-3-6-7, Columns:[tax, raw_tax, price, line_id, name, unit_price, quantity, tax_rate] */
            detail AS d
        GROUP BY
            d.tax_rate
    )
SELECT
    line_id,
    name,
    unit_price,
    quantity,
    tax_rate,
    price,
    price + tax AS tax_included_price,
    tax
FROM
    /* Lv:1, Seq:1, Refs:0-1, Columns:[line_id, name, unit_price, quantity, tax_rate, price, tax] */
    (
        SELECT
            line_id,
            name,
            unit_price,
            quantity,
            tax_rate,
            price,
            tax + adjust_tax AS tax
        FROM
            /* Lv:2, Seq:1, Refs:0-1-2, Columns:[adjust_tax, total_tax, cumulative, priority, tax, raw_tax, price, line_id, name, unit_price, quantity, tax_rate] */
            (
                SELECT
                    q.*,
                    CASE
                        WHEN q.total_tax - q.cumulative >= q.priority THEN 1
                        ELSE 0
                    END AS adjust_tax
                FROM
                    /* Lv:3, Seq:1, Refs:0-1-2-3, Columns:[total_tax, cumulative, priority, tax, raw_tax, price, line_id, name, unit_price, quantity, tax_rate] */
                    (
                        SELECT
                            d.*,
                            s.total_tax,
                            SUM(d.tax) OVER(
                                PARTITION BY
                                    d.tax_rate
                            ) AS cumulative,
                            ROW_NUMBER() OVER(
                                PARTITION BY
                                    d.tax_rate
                                ORDER BY
                                    d.raw_tax % 1 DESC,
                                    d.line_id
                            ) AS priority
                        FROM
                            /* Lv:4, Seq:1, Refs:0-1-2-3-4, Columns:[tax, raw_tax, price, line_id, name, unit_price, quantity, tax_rate] */
                            detail AS d
                            INNER JOIN
                            /* Lv:4, Seq:2, Refs:0-1-2-3-6, Columns:[tax_rate, total_tax] */
                            tax_summary AS s ON d.tax_rate = s.tax_rate
                    ) AS q
            ) AS q
    ) AS q
ORDER BY
    line_id";

        var query = new SelectQuery(sql);
        query.GetQuerySources().ForEach(x =>
        {
            x.AddSourceComment($"Index:{x.Index}, Alias:{x.Alias}, MaxLv:{x.MaxLevel}, SourceType:{x.SourceType}, Columns:[{string.Join(", ", x.ColumnNames)}]");
            x.ToTreePaths().ForEach(path => x.AddSourceComment($"Path:{string.Join("-", path)}"));
        });
        var sources = query.GetQuerySources().ToList();
        Monitor.Log(sources);
        Monitor.Log(query);

        var expect = @"WITH
    dat (
        line_id, name, unit_price, quantity, tax_rate
    ) AS (
        VALUES
            (1, 'apple', 105, 5, 0.07),
            (2, 'orange', 203, 3, 0.07),
            (3, 'banana', 233, 9, 0.07),
            (4, 'tea', 309, 7, 0.08),
            (5, 'coffee', 555, 9, 0.08),
            (6, 'cola', 456, 2, 0.08)
    ),
    detail AS (
        SELECT
            q.*,
            TRUNC(q.price * (1 + q.tax_rate)) - q.price AS tax,
            q.price * (1 + q.tax_rate) - q.price AS raw_tax
        FROM
            /* Index:5, Alias:q, MaxLv:6, SourceType:SubQuery, Columns:[price, line_id, name, unit_price, quantity, tax_rate] */
            /* Path:5-4-3-2-1-0 */
            /* Path:5-8-7-3-2-1-0 */
            (
                SELECT
                    dat.*,
                    (dat.unit_price * dat.quantity) AS price
                FROM
                    /* Index:6, Alias:dat, MaxLv:7, SourceType:ValuesQuery, Columns:[line_id, name, unit_price, quantity, tax_rate] */
                    /* Path:6-5-4-3-2-1-0 */
                    /* Path:6-5-8-7-3-2-1-0 */
                    dat
            ) AS q
    ),
    tax_summary AS (
        SELECT
            d.tax_rate,
            TRUNC(SUM(raw_tax)) AS total_tax
        FROM
            /* Index:8, Alias:d, MaxLv:5, SourceType:CommonTableExtension, Columns:[tax, raw_tax, price, line_id, name, unit_price, quantity, tax_rate] */
            /* Path:8-7-3-2-1-0 */
            detail AS d
        GROUP BY
            d.tax_rate
    )
SELECT
    line_id,
    name,
    unit_price,
    quantity,
    tax_rate,
    price,
    price + tax AS tax_included_price,
    tax
FROM
    /* Index:1, Alias:q, MaxLv:1, SourceType:SubQuery, Columns:[line_id, name, unit_price, quantity, tax_rate, price, tax] */
    /* Path:1-0 */
    (
        SELECT
            line_id,
            name,
            unit_price,
            quantity,
            tax_rate,
            price,
            tax + adjust_tax AS tax
        FROM
            /* Index:2, Alias:q, MaxLv:2, SourceType:SubQuery, Columns:[adjust_tax, total_tax, cumulative, priority, tax, raw_tax, price, line_id, name, unit_price, quantity, tax_rate] */
            /* Path:2-1-0 */
            (
                SELECT
                    q.*,
                    CASE
                        WHEN q.total_tax - q.cumulative >= q.priority THEN 1
                        ELSE 0
                    END AS adjust_tax
                FROM
                    /* Index:3, Alias:q, MaxLv:3, SourceType:SubQuery, Columns:[total_tax, cumulative, priority, tax, raw_tax, price, line_id, name, unit_price, quantity, tax_rate] */
                    /* Path:3-2-1-0 */
                    (
                        SELECT
                            d.*,
                            s.total_tax,
                            SUM(d.tax) OVER(
                                PARTITION BY
                                    d.tax_rate
                            ) AS cumulative,
                            ROW_NUMBER() OVER(
                                PARTITION BY
                                    d.tax_rate
                                ORDER BY
                                    d.raw_tax % 1 DESC,
                                    d.line_id
                            ) AS priority
                        FROM
                            /* Index:4, Alias:d, MaxLv:4, SourceType:CommonTableExtension, Columns:[tax, raw_tax, price, line_id, name, unit_price, quantity, tax_rate] */
                            /* Path:4-3-2-1-0 */
                            detail AS d
                            INNER JOIN
                            /* Index:7, Alias:s, MaxLv:4, SourceType:CommonTableExtension, Columns:[tax_rate, total_tax] */
                            /* Path:7-3-2-1-0 */
                            tax_summary AS s ON d.tax_rate = s.tax_rate
                    ) AS q
            ) AS q
    ) AS q
ORDER BY
    line_id";

        Assert.Equal(8, sources.Count);
        Assert.Equal(expect, query.ToText());
    }

    [Fact]
    public void TypeCastTest()
    {
        var sql = @"
WITH
    monthly_sales AS (
        SELECT
            product_id,
            DATE_TRUNC('month', sales_date) AS month,
            SUM(sales_amount) AS total_sales
        FROM
            /* Lv:2, Seq:1, Tree:0-1-2, Columns:[product_id, sales_date, sales_amount] */
            /* Lv:3, Seq:1, Tree:0-3-4-5, Columns:[product_id, sales_date, sales_amount] */
            sales
        GROUP BY
            product_id,
            DATE_TRUNC('month', sales_date)
    ),
    total_monthly_sales AS (
        SELECT
            month,
            SUM(total_sales) AS total_sales
        FROM
            /* Lv:2, Seq:1, Tree:0-3-4, Columns:[product_id, month, total_sales] */
            monthly_sales
        GROUP BY
            month
    )
SELECT
    ms.product_id,
    ms.month,
    ms.total_sales,
    tms.total_sales AS total_monthly_sales,
    (ms.total_sales::FLOAT / tms.total_sales) * 100 AS sales_percentage
FROM
    /* Lv:1, Seq:1, Tree:0-1, Columns:[product_id, month, total_sales] */
    monthly_sales AS ms
    INNER JOIN
    /* Lv:1, Seq:2, Tree:0-3, Columns:[month, total_sales] */
    total_monthly_sales AS tms ON ms.month = tms.month
ORDER BY
    ms.month,
    ms.product_id";

        var expect = @"WITH
    monthly_sales AS (
        SELECT
            product_id,
            DATE_TRUNC('month', sales_date) AS month,
            SUM(sales_amount) AS total_sales
        FROM
            /* Index:2, MaxLv:3, Columns:[product_id, sales_date, sales_amount] */
            /* Path:2-1-0 */
            /* Path:2-4-3-0 */
            sales
        GROUP BY
            product_id,
            DATE_TRUNC('month', sales_date)
    ),
    total_monthly_sales AS (
        SELECT
            month,
            SUM(total_sales) AS total_sales
        FROM
            /* Index:4, MaxLv:2, Columns:[product_id, month, total_sales] */
            /* Path:4-3-0 */
            monthly_sales
        GROUP BY
            month
    )
SELECT
    ms.product_id,
    ms.month,
    ms.total_sales,
    tms.total_sales AS total_monthly_sales,
    (ms.total_sales::FLOAT / tms.total_sales) * 100 AS sales_percentage
FROM
    /* Index:1, MaxLv:1, Columns:[product_id, month, total_sales] */
    /* Path:1-0 */
    monthly_sales AS ms
    INNER JOIN
    /* Index:3, MaxLv:1, Columns:[month, total_sales] */
    /* Path:3-0 */
    total_monthly_sales AS tms ON ms.month = tms.month
ORDER BY
    ms.month,
    ms.product_id";

        var query = new SelectQuery(sql);
        query.GetQuerySources().ForEach(x =>
        {
            x.AddSourceComment($"Index:{x.Index}, MaxLv:{x.MaxLevel}, Columns:[{string.Join(", ", x.ColumnNames)}]");
            x.ToTreePaths().ForEach(path => x.AddSourceComment($"Path:{string.Join("-", path)}"));
        });
        var sources = query.GetQuerySources().ToList();
        Monitor.Log(sources);
        Monitor.Log(query);

        Assert.Equal(expect, query.ToText());
        Assert.Equal(4, sources.Count);
    }

    [Fact]
    public void NameConflict()
    {
        var sql = @"
with
sales as (
    select * from sales
)
select
    *
from
    sales";

        var query = new SelectQuery(sql);
        query.GetQuerySources().ForEach(x =>
        {
            x.AddSourceComment($"Index:{x.Index}, Alias:{x.Alias}, MaxLv:{x.MaxLevel}, SourceType:{x.SourceType}, Columns:[{string.Join(", ", x.ColumnNames)}]");
            x.ToTreePaths().ForEach(path => x.AddSourceComment($"Path:{string.Join("-", path)}"));
        });
        Monitor.Log(query);

        var expect = @"WITH
    sales AS (
        SELECT
            *
        FROM
            /* Index:2, Alias:sales, MaxLv:2, SourceType:PhysicalTable, Columns:[] */
            /* Path:2-1-0 */
            sales
    )
SELECT
    *
FROM
    /* Index:1, Alias:sales, MaxLv:1, SourceType:CommonTableExtension, Columns:[] */
    /* Path:1-0 */
    sales";

        Assert.Equal(expect, query.ToText());
    }

    /// <summary>
    /// ISSUE-482
    /// </summary>
    [Fact]
    public void UseRelationshipUseWildcardExists()
    {
        var sql = @"
select
1
from
sales s
inner join target t on s.sale_id = t.sale_id --use relation ship
where
exists (select * from dat x where x.sale_id = s.sale_id) --use wildcard";

        var query = new SelectQuery(sql);
        query.GetQuerySources().ForEach(x =>
        {
            x.AddSourceComment($"Index:{x.Index}, Alias:{x.Alias}, MaxLv:{x.MaxLevel}, SourceType:{x.SourceType}, Columns:[{string.Join(", ", x.ColumnNames)}]");
            x.ToTreePaths().ForEach(path => x.AddSourceComment($"Path:{string.Join("-", path)}"));
        });
        Monitor.Log(query);

        var expect = @"SELECT
    1
FROM
    /* Index:1, Alias:s, MaxLv:1, SourceType:PhysicalTable, Columns:[sale_id] */
    /* Path:1-0 */
    sales AS s
    INNER JOIN
    /* Index:2, Alias:t, MaxLv:1, SourceType:PhysicalTable, Columns:[sale_id] */
    /* Path:2-0 */
    target AS t ON s.sale_id = t.sale_id
WHERE
    EXISTS (
        SELECT
            *
        FROM
            /* Index:3, Alias:x, MaxLv:1, SourceType:PhysicalTable, Columns:[sale_id] */
            /* Path:3-0 */
            dat AS x
        WHERE
            x.sale_id = s.sale_id
    )";

        Assert.Equal(expect, query.ToText());
    }

    /// <summary>
    /// ISSUE-482
    /// </summary>
    [Fact]
    public void SelectableColumnTest()
    {
        var sql = @"
select
1
from
sales s --has not relationship
where
not exists (select dat.v1 from dat where x.sale_id = s.sale_id) --use wildcard";

        var query = new SelectQuery(sql);
        query.GetQuerySources().ForEach(x =>
        {
            x.AddSourceComment($"Index:{x.Index}, Alias:{x.Alias}, MaxLv:{x.MaxLevel}, SourceType:{x.SourceType}, Columns:[{string.Join(", ", x.ColumnNames)}]");
            x.ToTreePaths().ForEach(path => x.AddSourceComment($"Path:{string.Join("-", path)}"));
        });
        Monitor.Log(query);

        var expect = @"SELECT
    1
FROM
    /* Index:1, Alias:s, MaxLv:1, SourceType:PhysicalTable, Columns:[sale_id] */
    /* Path:1-0 */
    sales AS s
WHERE
    NOT EXISTS (
        SELECT
            dat.v1
        FROM
            /* Index:2, Alias:dat, MaxLv:1, SourceType:PhysicalTable, Columns:[v1] */
            /* Path:2-0 */
            dat
        WHERE
            x.sale_id = s.sale_id
    )";

        Assert.Equal(expect, query.ToText());
    }

    [Fact]
    public void NoAlias()
    {
        var sql = @"
SELECT
    customer_id,
    DATE_TRUNC('month', sale_date) AS sale_month,
    SUM(sale_amount) AS total_sales
FROM
    sales
GROUP BY
    customer_id,
    DATE_TRUNC('month', sale_date)";

        var query = new SelectQuery(sql);
        query.GetQuerySources().ForEach(x =>
        {
            x.AddSourceComment($"Index:{x.Index}, Alias:{x.Alias}, MaxLv:{x.MaxLevel}, SourceType:{x.SourceType}, Columns:[{string.Join(", ", x.ColumnNames)}]");
            x.ToTreePaths().ForEach(path => x.AddSourceComment($"Path:{string.Join("-", path)}"));
        });
        Monitor.Log(query);

        var expect = @"SELECT
    customer_id,
    DATE_TRUNC('month', sale_date) AS sale_month,
    SUM(sale_amount) AS total_sales
FROM
    /* Index:1, Alias:sales, MaxLv:1, SourceType:PhysicalTable, Columns:[customer_id, sale_date, sale_amount] */
    /* Path:1-0 */
    sales
GROUP BY
    customer_id,
    DATE_TRUNC('month', sale_date)";

        Assert.Equal(expect, query.ToText());
    }

    [Fact]
    public void ExistsTest()
    {
        var sql = @"
with
target as (
select * from dat
)
select
*
from
sale s
where
exists (select * from target x where x.sale_id = s.sale_id)";

        var query = new SelectQuery(sql);
        query.GetQuerySources().ForEach(x =>
        {
            x.AddSourceComment($"Index:{x.Index}, Alias:{x.Alias}, MaxLv:{x.MaxLevel}, SourceType:{x.SourceType}, Columns:[{string.Join(", ", x.ColumnNames)}]");
            x.ToTreePaths().ForEach(path => x.AddSourceComment($"Path:{string.Join("-", path)}"));
        });
        Monitor.Log(query);

        var expect = @"WITH
    target AS (
        SELECT
            *
        FROM
            /* Index:3, Alias:dat, MaxLv:2, SourceType:PhysicalTable, Columns:[] */
            /* Path:3-2-0 */
            dat
    )
SELECT
    *
FROM
    /* Index:1, Alias:s, MaxLv:1, SourceType:PhysicalTable, Columns:[sale_id] */
    /* Path:1-0 */
    sale AS s
WHERE
    EXISTS (
        SELECT
            *
        FROM
            /* Index:2, Alias:x, MaxLv:1, SourceType:CommonTableExtension, Columns:[sale_id] */
            /* Path:2-0 */
            target AS x
        WHERE
            x.sale_id = s.sale_id
    )";

        Assert.Equal(expect, query.ToText());
    }

    [Fact]
    public void CTEExistsTest()
    {
        var sql = @"
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

        var query = new SelectQuery(sql);
        query.GetQuerySources().ForEach(x =>
        {
            x.AddSourceComment($"Index:{x.Index}, Alias:{x.Alias}, MaxLv:{x.MaxLevel}, SourceType:{x.SourceType}, Columns:[{string.Join(", ", x.ColumnNames)}]");
            x.ToTreePaths().ForEach(path => x.AddSourceComment($"Path:{string.Join("-", path)}"));
        });
        Monitor.Log(query);

        var expect = @"WITH
    final AS (
        SELECT
            s.sale_id,
            s.store_id,
            GREATEST(s.sale_date, :lower_limit) AS journal_date,
            s.price AS journal_price,
            s.request_timestamp
        FROM
            /* Index:2, Alias:s, MaxLv:2, SourceType:PhysicalTable, Columns:[sale_id, store_id, sale_date, price, request_timestamp] */
            /* Path:2-1-0 */
            sales AS s
        WHERE
            NOT EXISTS (
                SELECT
                    *
                FROM
                    /* Index:3, Alias:x, MaxLv:2, SourceType:PhysicalTable, Columns:[sale_id] */
                    /* Path:3-1-0 */
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
    /* Index:1, Alias:f, MaxLv:1, SourceType:CommonTableExtension, Columns:[sale_id, store_id, journal_date, journal_price, request_timestamp] */
    /* Path:1-0 */
    final AS f";

        Assert.Equal(expect, query.ToText());
    }
}
