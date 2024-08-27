# Carbunql

![image](https://github.com/user-attachments/assets/3c364944-8de3-4200-8293-3492f7680e06)

![GitHub](https://img.shields.io/github/license/mk3008/Carbunql)
![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/mk3008/Carbunql)
![Github Last commit](https://img.shields.io/github/last-commit/mk3008/Carbunql)  
[![SqModel](https://img.shields.io/nuget/v/Carbunql.svg)](https://www.nuget.org/packages/Carbunql/) 
[![SqModel](https://img.shields.io/nuget/dt/Carbunql.svg)](https://www.nuget.org/packages/Carbunql/) 

This C# library allows you to flexibly edit Raw SQL as object models. It supports easy addition and modification of columns and search conditions, as well as transformations to CTEs (Common Table Expressions), subqueries, and even insert and update queries.

By using Carbunql, you can maximize the reusability of Raw SQL and efficiently manage your queries.

## Demo

This is a sample that accepts any search conditions and adds them as search conditions. If no conditions are entered, they will be excluded from the search conditions.

### Source code

I will only extract the parts that process SQL. Please refer to the link below for the complete source.
https://github.com/mk3008/Carbunql/blob/main/demo/DynamicFiltering/Program.cs

```cs
private static string GenerateProductQuery(decimal? minPrice, decimal? maxPrice, string? category, bool? inStock)
{
    var sql = """
SELECT
    p.product_id,
    p.product_name,
    p.price,
    p.category,
    p.in_stock
FROM
    product as p
""";

    var pname = ":category";

    // Convert the selection query to an object
    var sq = SelectQuery.Parse(sql)
        .GreaterThanOrEqualIfNotNullOrEmpty("price", minPrice)
        .LessThanOrEqualIfNotNullOrEmpty("price", maxPrice)
        .AddParameter(pname, category)
        .EqualIfNotNullOrEmpty("category", pname)
        .EqualIfNotNullOrEmpty("in_stock", inStock);

    return sq.ToText();
}
```

### Parameter

```
Enter minimum price (or leave blank to omit):

Enter maximum price (or leave blank to omit):
100
Enter category (or leave blank to omit):
tea
Enter in-stock status (true/false) (or leave blank to omit):
true
```

### Generated SQL

You can see that only the specified search criteria has been inserted.

```sql
/*
  :category = 'tea'
*/
SELECT
    p.product_id,
    p.product_name,
    p.price,
    p.category,
    p.in_stock
FROM
    product AS p
WHERE
    p.price <= 100
    AND p.category = :category
    AND p.in_stock = True
```

## Advanced Demo

This is a sample that displays an aggregate report for a specified year and month.

You can see that even complex processing using CTE and UNION can be written simply.

### Source code

I will only extract the parts that process SQL. Please refer to the link below for the complete source.
https://github.com/mk3008/Carbunql/blob/main/demo/DynamicCTE/Program.cs

```cs
public static string GenerateReportQuery(bool includeSummary, DateTime summaryMonth)
{
    string dailySummaryQuery = """
        SELECT
            sale_date
            , sum(amount) AS amount_total
            , '' as caption 
            , 1 as sort_number
        FROM
            salse
        GROUP BY
            sale_date
        """;

    string monthlySummaryQuery = """
        SELECT
            date_trunc('month', sale_date) + '1 month -1 day' as sale_date
            , sum(amount) AS amount_total
            , 'monthly total' as caption 
            , 2 as sort_number
        FROM
            salse
        GROUP BY
            date_trunc('month', sale_date) + '1 month -1 day'
        """;

    // Create daily summary query
    var sq = new SelectQuery()
        .With(dailySummaryQuery, "daily_summary")
        .From("daily_summary", "d")
        .SelectAll("d");

    if (includeSummary)
    {
        // Add monthly summary query with UNION ALL
        sq.UnionAll(() =>
        {
            var xsq = new SelectQuery()
                .With(monthlySummaryQuery, "monthly_summary")
                .From("monthly_summary", "m")
                .SelectAll("m");
            return xsq;
        });
    }

    // Add date filter condition
    var saleDate = ":sale_date";
    sq.AddParameter(saleDate, summaryMonth)
        .BetweenInclusiveStart("sale_date", saleDate, $"{saleDate}::timestamp + '1 month'");

    // Convert the entire query to a CTE
    sq = sq.ToCTEQuery("final", "f");

    // Add sorting conditions
    sq.RemoveSelect("sort_number")
        .OrderBy("sale_date")
        .OrderBy("sort_number");

    return sq.ToText();
}
```

### Parameter

```
Which month to summarize? (yyyy-mm-dd)
2024-08-01
Include monthly summary rows? (true/false)
true
```

### Generated SQL

A query that combines a daily aggregation query and a monthly aggregation query will be output.

Please also note that the search conditions are inserted in two places. Carbunql will insert the search conditions in the appropriate positions without you having to specify them in detail.

```sql
/*
  :sale_date = '2024/08/01 0:00:00'
*/
WITH
    daily_summary AS (
        SELECT
            sale_date,
            SUM(amount) AS amount_total,
            '' AS caption,
            1 AS sort_number
        FROM
            salse
        WHERE
            :sale_date <= salse.sale_date
            AND salse.sale_date < :sale_date::timestamp + '1 month'
        GROUP BY
            sale_date
    ),
    monthly_summary AS (
        SELECT
            DATE_TRUNC('month', sale_date) + '1 month -1 day' AS sale_date,
            SUM(amount) AS amount_total,
            'monthly total' AS caption,
            2 AS sort_number
        FROM
            salse
        WHERE
            :sale_date <= salse.sale_date
            AND salse.sale_date < :sale_date::timestamp + '1 month'
        GROUP BY
            DATE_TRUNC('month', sale_date) + '1 month -1 day'
    ),
    final AS (
        SELECT
            d.sale_date,
            d.amount_total,
            d.caption,
            d.sort_number
        FROM
            daily_summary AS d
        UNION ALL
        SELECT
            m.sale_date,
            m.amount_total,
            m.caption,
            m.sort_number
        FROM
            monthly_summary AS m
    )
SELECT
    f.sale_date,
    f.amount_total,
    f.caption
FROM
    final AS f
ORDER BY
    f.sale_date,
    f.sort_number
```

## Features

- Convert Raw SQL to object models.
- Convert object models back to Raw SQL.
- Format Raw SQL.
- Convert select queries to various query types:
  - Insert queries
  - Update queries
  - Delete queries
  - Merge queries
  - Table creation queries
- Edit columns and search conditions in select queries.
- No DBMS environment or entity classes required.
- Perform basic syntax checks.
- The [Carbunql.Dapper](https://www.nuget.org/packages/Carbunql.Dapper) library allows you to run queries with Dapper.

You can try out some of the processing on the online demo site.

https://mk3008.github.io/Carbunql/

![image](https://github.com/user-attachments/assets/21ddb1af-ae13-4405-a2c9-40b15f372e84)

## Constraints

- Comments are removed when modeling.

## Getting started
Install the package from NuGet.

```
PM> Install-Package Carbunql
```

https://www.nuget.org/packages/Carbunql/

## Model a select query

Just pass the select query string to the constructor of the SelectQuery class.

```cs
using Carbunql;

var text = "select s.sale_id, s.store_id, date_trunc('month', s.sale_date) as allocate_ym, s.sale_price from sales as s";
var sq = SelectQuery.Parse(text);
```

## Return the model to a select query

Use the `ToText` or `ToOneLineText` method.

The `ToText` method will return a formatted select query. Parameter information will also be added as a comment.

The `ToOneLineText` method will output a single line without formatting. Use the ToOneLineText method if performance is important.

```cs
using Carbunql;

var text = "select s.sale_id, s.store_id, date_trunc('month', s.sale_date) as allocate_ym, s.sale_price from sales as s";
var sq = SelectQuery.Parse(text);
var query = sq.ToOneLineText();
```

## Execute the query

If you use [Carbunql.Dapper](https://www.nuget.org/packages/Carbunql.Dapper), you can execute it directly without writing it back to RawSQL.

This is the recommended method because parameter information is also passed.

```
PM> Install-Package Carbunql.Dapper
```

The full source code for the demo can be found below.
https://github.com/mk3008/Carbunql/blob/main/test/Carbunql.Dapper.Test/DapperTest.cs

```cs
using var cn = PostgresDB.ConnectionOpenAsNew(Logger);

var sql = @"with
data_ds(c1, c2) as (
    values
    (1,1)
    , (1,2)
    , (2,1)
    , (2,2)
)
select
    *
from
    data_ds
where
    c1 = :val
";

var sq = SelectQuery.Parse(sql);
sq.AddParameter(":val", 1);

using var r = cn.ExecuteReader(sq);
var cnt = 0;
while (r.Read())
{
    cnt++;
}
Assert.Equal(2, cnt);
```

## Create an empty select query

If you do not specify arguments in the constructor, a model without SELECT and FROM clauses will be created. Please add SELECT and FROM clauses manually.

```cs
using Carbunql;

var sq = new SelectQuery();
```

## Add a FROM clause

If you added an empty select query, use the `From` function to manually add a FROM clause. The first argument is the table name, and the second argument is the alias name.

> [!NOTE]
> Don't forget to import the namespace Carbunql.Fluent.

```cs
using Carbunql;
using Carbunql.Fluent;

var sq = new SelectQuery()
  .From("customer", "c");
```

## Add a column to select

You can add a column to select by using the `Select` function. The first argument is the column name, and the second argument is the column alias name. The column alias name is optional.

```cs
using Carbunql;
using Carbunql.Fluent;

var sq = new SelectQuery()
  .From("customer", "c")
  .Select("c.customer_id")
  .Select("c.first_name || c.last_name", "customer_name");
```

## Add search conditions

You can add search conditions by using the `AddWhere` method.

The first argument is the name of the column to which you want to add a condition.

The second argument is a delegate (or lambda expression) that takes the column source and column name as input and generates the condition part of the SQL.

```cs
using Carbunql;

var text = "select s.sale_id, s.store_id, date_trunc('month', s.sale_date) as allocate_ym, s.sale_price from sales as s";
var sq = SelectQuery.Parse(text)
  .AddWhere("sale_id", (source, column) => $"{source.Alias}.{column} = 1");
```

The above code can be written even more succinctly if we import the namespace Carbunql.Fluent:

```cs
using Carbunql;
using Carbunql.Fluent;

var text = "select s.sale_id, s.store_id, date_trunc('month', s.sale_date) as allocate_ym, s.sale_price from sales as s";
var sq = SelectQuery.Parse(text)
  .Equal("sale_id", 1);
```

## About the AddWhere function

Please note the following specifications.

- It is added with AND conditions.
- If the column name to be searched does not exist, an error will occur.
- The search target is the entire query. The query reference order is analyzed, and the conditions are inserted only for the query source at the deepest position.

For example, the search condition for the column `sale_id` is inserted only for `s` in the subquery.
It is not inserted for the query source `subq`. This is because it references a query source to which the search condition has already been applied.

```sql
select
    subq.sale_id
from
    (
        select
            s.sale_id
        from
            sale as s --the deepest query source to which sale_id belongs
    ) subq
```

In this way, since the reference order of the query source is analyzed, the search condition is inserted in the optimal position without any consideration of the insertion location.

However, since this library does not reference the DBMS (table definition), it cannot detect column names that are not explicitly stated in the select query.

If you try to insert a search condition for the column `price` into the select query above, an error will occur even if the column is defined in the DBMS.

In this case, make it clear that there is a column name in the select query. If you write it like this, the column `price` will be detected.

```sql
select
    subq.sale_id
from
    (
        select
            s.sale_id
            , s.price
        from
            sale as s
    ) subq
```

Also, when searching for a column, it checks which query source it is attributed to. For this reason, if the alias name of the query source is omitted, it may not be detected correctly.

For example, in the case of a single query source like the one below, the query source is only `s`, so it is possible to identify the column `price`.

```
select
    sale_id
    , price
from
    sale as s
```

However, if a table is joined, parsing will fail even if the SQL is executable because it does not refer to the DBMS (table definition). In the following query, it is not possible to determine whether the column "price" belongs to `s` or `c` based on the information in the select query alone.

```
select
    sale_id
    , price
from
    sale as s
    inner join customer as c on s.customer_id = c.customer_id
```

## Referenced Libraries
### ZString / MIT License
https://github.com/Cysharp/ZString

https://github.com/Cysharp/ZString/blob/master/LICENSE

Copyright (c) 2020 Cysharp, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

### Dapper / Apache License 2.0
https://github.com/DapperLib/Dapper

https://github.com/DapperLib/Dapper/blob/main/License.txt

The Dapper library and tools are licenced under Apache 2.0: http://www.apache.org/licenses/LICENSE-2.0

The Dapper logo is copyright Marc Gravell 2021 onwards; it is fine to use the Dapper logo when referencing the Dapper library and utilities, but
the Dapper logo (including derivatives) must not be used in a way that misrepresents an external product or library as being affiliated or endorsed
with Dapper. For example, you must not use the Dapper logo as the package icon on your own external tool (even if it uses Dapper internally),
without written permission. If in doubt: ask.
