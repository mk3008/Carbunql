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
        var sources = query.GetQuerySources().ToList();
        Monitor.Log(sources);

        Assert.Single(sources);

        var ds = sources[0];
        Assert.Equal(1, ds.Sequence);
        Assert.Equal(1, ds.Branch);
        Assert.Equal(1, ds.Level);
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
        var sources = query.GetQuerySources().ToList();
        Monitor.Log(sources);

        Assert.Single(sources);

        var ds = sources[0];
        Assert.Equal(1, ds.Sequence);
        Assert.Equal(1, ds.Branch);
        Assert.Equal(1, ds.Level);
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
        var sources = query.GetQuerySources().ToList();
        Monitor.Log(sources);

        Assert.Single(sources);

        var ds = sources[0];
        Assert.Equal(1, ds.Sequence);
        Assert.Equal(1, ds.Branch);
        Assert.Equal(1, ds.Level);
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
        var sources = query.GetQuerySources().ToList();
        Monitor.Log(sources);

        Assert.Single(sources);

        var ds = sources[0];
        Assert.Equal(1, ds.Sequence);
        Assert.Equal(1, ds.Branch);
        Assert.Equal(1, ds.Level);
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
        var sources = query.GetQuerySources().ToList();
        Monitor.Log(sources);

        Assert.Single(sources);

        var ds = sources[0];
        Assert.Equal(1, ds.Sequence);
        Assert.Equal(1, ds.Branch);
        Assert.Equal(1, ds.Level);
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
        var sources = query.GetQuerySources().ToList();
        Monitor.Log(sources);

        Assert.Single(sources);

        var ds = sources[0];
        Assert.Equal(1, ds.Sequence);
        Assert.Equal(1, ds.Branch);
        Assert.Equal(1, ds.Level);
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
select
    d.sale_id
from
    --Seq:1, Branch:1, Lv:1
    (
        select 
            s.sale_id
            , s.store_id
            , s.quantity * q.amount as price
        from
            --Seq:1, Branch:1, Lv:2
            sale as s
    ) as d";

        var query = new SelectQuery(sql);
        var sources = query.GetQuerySources().ToList();
        Monitor.Log(sources);

        Assert.Equal(2, sources.Count);

        //index:0
        var ds = sources[0];
        Assert.Equal(1, ds.Sequence);
        Assert.Equal(1, ds.Branch);
        Assert.Equal(2, ds.Level);
        Assert.Equal("s", ds.Alias);

        var columns = ds.ColumnNames.ToList();
        Assert.Equal(4, columns.Count());
        Assert.Equal("sale_id", columns[0]);
        Assert.Equal("store_id", columns[1]);
        Assert.Equal("quantity", columns[2]);
        Assert.Equal("amount", columns[3]);

        //index:1
        ds = sources[1];
        Assert.Equal(1, ds.Sequence);
        Assert.Equal(1, ds.Branch);
        Assert.Equal(1, ds.Level);
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
with
cte as (
    select 
        s.sale_id
        , s.store_id
        , s.quantity * q.amount as price
    from
        --Seq:1, Branch:1, Lv:2
        sale as s
)
select
    d.sale_id
from
    --Seq:1, Branch:1, Lv:1
    cte as d";

        var query = new SelectQuery(sql);
        var sources = query.GetQuerySources().ToList();
        Monitor.Log(sources);

        Assert.Equal(2, sources.Count);

        //index:0
        var ds = sources[0];
        Assert.Equal(1, ds.Sequence);
        Assert.Equal(1, ds.Branch);
        Assert.Equal(2, ds.Level);
        Assert.Equal("s", ds.Alias);

        var columns = ds.ColumnNames.ToList();
        Assert.Equal(4, columns.Count());
        Assert.Equal("sale_id", columns[0]);
        Assert.Equal("store_id", columns[1]);
        Assert.Equal("quantity", columns[2]);
        Assert.Equal("amount", columns[3]);

        //index:1
        ds = sources[1];
        Assert.Equal(1, ds.Sequence);
        Assert.Equal(1, ds.Branch);
        Assert.Equal(1, ds.Level);
        Assert.Equal("d", ds.Alias);

        columns = ds.ColumnNames.ToList();
        Assert.Equal(3, columns.Count());
        Assert.Equal("sale_id", columns[0]);
        Assert.Equal("store_id", columns[1]);
        Assert.Equal("price", columns[2]); // The column "price" is recognizable.

    }

    [Fact]
    public void UnionTest()
    {
        //If you are using a CTE,
        //you get the participating columns from the query's select clause.
        var sql = @"
select 
    s.sale_id
    , s.store_id
    , s.price
from
    --Seq:1, Branch:1, Lv:1
    sale as s
where
    s.sale_id = 1
union all
select 
    s.sale_id
    , s.store_id
    , s.price
from
    --Seq:2, Branch:2, Lv:1
    sale as s
where
    s.sale_id = 2
union all
select 
    s.sale_id
    , s.store_id
    , s.price
from
    --Seq:3, Branch:3, Lv:1
    sale as s
where
    s.sale_id = 3";

        var query = new SelectQuery(sql);
        var sources = query.GetQuerySources().ToList();
        Monitor.Log(sources);

        Assert.Equal(3, sources.Count);

        //index:0
        var ds = sources[0];
        Assert.Equal(1, ds.Sequence);
        Assert.Equal(1, ds.Branch);
        Assert.Equal(1, ds.Level);
        Assert.Equal("s", ds.Alias);

        var columns = ds.ColumnNames.ToList();
        Assert.Equal(3, columns.Count());
        Assert.Equal("sale_id", columns[0]);
        Assert.Equal("store_id", columns[1]);
        Assert.Equal("price", columns[2]);

        //index:1
        ds = sources[1];
        Assert.Equal(2, ds.Sequence);
        Assert.Equal(2, ds.Branch);
        Assert.Equal(1, ds.Level);
        Assert.Equal("s", ds.Alias);

        columns = ds.ColumnNames.ToList();
        Assert.Equal(3, columns.Count());
        Assert.Equal("sale_id", columns[0]);
        Assert.Equal("store_id", columns[1]);
        Assert.Equal("price", columns[2]);

        //index:2
        ds = sources[2];
        Assert.Equal(3, ds.Sequence);
        Assert.Equal(3, ds.Branch);
        Assert.Equal(1, ds.Level);
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
select
    *
from
    --Seq:1, Branch:1, Lv:1
    (
        select
            *
        from
            --Seq:1, Branch:1, Lv:2
            (
                select 
                    s.sale_id
                    , s.store_id
                    , s.quantity * q.amount as price
                from
                    --Seq:1, Branch:1, Lv:3
                    sale as s
            ) as d
    ) as q";

        var query = new SelectQuery(sql);
        var sources = query.GetQuerySources().ToList();
        Monitor.Log(sources);

        Assert.Equal(3, sources.Count);

        //index:0
        var ds = sources[0];
        Assert.Equal(1, ds.Sequence);
        Assert.Equal(1, ds.Branch);
        Assert.Equal(3, ds.Level);
        Assert.Equal("s", ds.Alias);

        var columns = ds.ColumnNames.ToList();
        Assert.Equal(4, columns.Count());
        Assert.Equal("sale_id", columns[0]);
        Assert.Equal("store_id", columns[1]);
        Assert.Equal("quantity", columns[2]);
        Assert.Equal("amount", columns[3]);

        //index:1
        ds = sources[1];
        Assert.Equal(1, ds.Sequence);
        Assert.Equal(1, ds.Branch);
        Assert.Equal(2, ds.Level);
        Assert.Equal("d", ds.Alias);

        columns = ds.ColumnNames.ToList();
        Assert.Equal(3, columns.Count());
        Assert.Equal("sale_id", columns[0]);
        Assert.Equal("store_id", columns[1]);
        Assert.Equal("price", columns[2]);

        //index:2
        ds = sources[2];
        Assert.Equal(1, ds.Sequence);
        Assert.Equal(1, ds.Branch);
        Assert.Equal(1, ds.Level);
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
select
    q.*
from
    --Index:2, Seq:1, Branch:1, Lv:1
    (
        select
            d.*
        from
            --Index:1, Seq:1, Branch:1, Lv:2
            (
                select 
                    s.sale_id
                    , s.store_id
                    , s.quantity * q.amount as price
                from
                    --Index:0, Seq:1, Branch:1, Lv:3
                    sale as s
            ) as d
    ) as q
    --Index:3, Seq:2, Branch:2, Lv:1
    cross join store as st
where
    st.store_id = 1
";

        var query = new SelectQuery(sql);
        var sources = query.GetQuerySources().ToList();
        Monitor.Log(sources);

        Assert.Equal(4, sources.Count);

        //index:0
        var ds = sources[0];
        Assert.Equal(1, ds.Sequence);
        Assert.Equal(1, ds.Branch);
        Assert.Equal(3, ds.Level);
        Assert.Equal("s", ds.Alias);

        var columns = ds.ColumnNames.ToList();
        Assert.Equal(4, columns.Count());
        Assert.Equal("sale_id", columns[0]);
        Assert.Equal("store_id", columns[1]);
        Assert.Equal("quantity", columns[2]);
        Assert.Equal("amount", columns[3]);

        //index:1
        ds = sources[1];
        Assert.Equal(1, ds.Sequence);
        Assert.Equal(1, ds.Branch);
        Assert.Equal(2, ds.Level);
        Assert.Equal("d", ds.Alias);

        columns = ds.ColumnNames.ToList();
        Assert.Equal(3, columns.Count());
        Assert.Equal("sale_id", columns[0]);
        Assert.Equal("store_id", columns[1]);
        Assert.Equal("price", columns[2]);

        //index:2
        ds = sources[2];
        Assert.Equal(1, ds.Sequence);
        Assert.Equal(1, ds.Branch);
        Assert.Equal(1, ds.Level);
        Assert.Equal("q", ds.Alias);

        columns = ds.ColumnNames.ToList();
        Assert.Equal(3, columns.Count());
        Assert.Equal("sale_id", columns[0]);
        Assert.Equal("store_id", columns[1]);
        Assert.Equal("price", columns[2]);

        //index:3
        ds = sources[3];
        Assert.Equal(2, ds.Sequence);
        Assert.Equal(2, ds.Branch);
        Assert.Equal(1, ds.Level);
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
            --Index:1, Seq:1, Branch:2, Lv:3 
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
            --Index:0, Seq:1, Branch:1, Lv:2
            customers AS c
            --Index:2, Seq:1, Branch:2, Lv:2
            LEFT JOIN monthly_sales AS ms ON c.customer_id = ms.customer_id
    )
SELECT
    *
FROM
    --Index:3,  Seq:1, Branch:1, Lv:1
    report AS r
ORDER BY
    r.customer_id,
    r.sale_month";

        var query = new SelectQuery(sql);
        var sources = query.GetQuerySources().ToList();
        Monitor.Log(sources);

        Assert.Equal(4, sources.Count);

        //index:0
        var ds = sources[0];
        Assert.Equal(1, ds.Sequence);
        Assert.Equal(1, ds.Branch);
        Assert.Equal(2, ds.Level);
        Assert.Equal("c", ds.Alias);

        var columns = ds.ColumnNames.ToList();
        Assert.Equal(2, columns.Count());
        Assert.Equal("customer_id", columns[0]);
        Assert.Equal("customer_name", columns[1]);

        //index:1
        ds = sources[1];
        Assert.Equal(1, ds.Sequence);
        Assert.Equal(2, ds.Branch);
        Assert.Equal(3, ds.Level);
        Assert.Equal("sales", ds.Alias);

        columns = ds.ColumnNames.ToList();
        Assert.Equal(3, columns.Count());
        Assert.Equal("customer_id", columns[0]);
        Assert.Equal("sale_date", columns[1]);
        Assert.Equal("sale_amount", columns[2]);

        //index:2
        ds = sources[2];
        Assert.Equal(2, ds.Sequence);
        Assert.Equal(2, ds.Branch);
        Assert.Equal(2, ds.Level);
        Assert.Equal("ms", ds.Alias);

        columns = ds.ColumnNames.ToList();
        Assert.Equal(3, columns.Count());
        Assert.Equal("customer_id", columns[0]);
        Assert.Equal("sale_month", columns[1]);
        Assert.Equal("total_sales", columns[2]);

        //index:3
        ds = sources[3];
        Assert.Equal(1, ds.Sequence);
        Assert.Equal(1, ds.Branch);
        Assert.Equal(1, ds.Level);
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
    --not dataset
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
            --Seq:1, Branch:1, Lv:5
            --Seq:1, Branch:2, Lv:6
            (
                SELECT
                    dat.*,
                    (dat.unit_price * dat.quantity) AS price
                FROM
                    --Seq:1, Branch:1, Lv:6
                    --Seq:1, Branch:2, Lv:7
                    dat
            ) AS q
    ),
    tax_summary AS (
        --Seq:1, Branch:2, Lv:5
        SELECT
            d.tax_rate,
            TRUNC(SUM(raw_tax)) AS total_tax
        FROM
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
    --Seq1, Branch:1, Lv:1
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
            --Seq:1, Branch:1, Lv:2
            (
                SELECT
                    q.*,
                    CASE
                        WHEN q.total_tax - q.cumulative >= q.priority THEN 1
                        ELSE 0
                    END AS adjust_tax
                FROM
                    --Seq:1, Branch:1, Lv:3
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
                            --Seq:1, Branch:1, Lv:4
                            detail AS d
                            --Seq:2, Branch:2, Lv:4
                            INNER JOIN tax_summary AS s ON d.tax_rate = s.tax_rate
                    ) AS q
            ) AS q
    ) AS q
ORDER BY
    line_id";

        var query = new SelectQuery(sql);
        var sources = query.GetQuerySources().ToList();
        Monitor.Log(sources);

        query.GetQuerySources().ForEach(x =>
        {
            x.AddSourceComment($"Seq:{x.Sequence}, Branch:{x.Branch}, Lv:{x.Level}");
        })
        .GetRootsByBranch().ForEach(x =>
        {
            x.AddQueryComment($"Root of Branch:{x.Branch}");
        });

        Monitor.Log(query);

        Assert.Equal(10, sources.Count);
    }

    [Fact]
    public void TypeCastTest()
    {
        var sql = @"
WITH monthly_sales AS (
    SELECT
        product_id,
        DATE_TRUNC('month', sales_date) AS month,
        SUM(sales_amount) AS total_sales
    FROM
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
    monthly_sales ms
INNER JOIN
    total_monthly_sales tms ON ms.month = tms.month
ORDER BY
    ms.month,
    ms.product_id";

        var query = new SelectQuery(sql);
        var sources = query.GetQuerySources().ToList();
        Monitor.Log(sources);

        query.GetQuerySources().ForEach(x =>
        {
            x.AddSourceComment($"Seq:{x.Sequence}, Branch:{x.Branch}, Lv:{x.Level}");
        })
        .GetRootsByBranch().ForEach(x =>
        {
            x.AddQueryComment($"Root of Branch:{x.Branch}");
        });

        Monitor.Log(query);

        Assert.Equal(5, sources.Count);
    }
}
