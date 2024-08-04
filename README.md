# Carbunql
![GitHub](https://img.shields.io/github/license/mk3008/Carbunql)
![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/mk3008/Carbunql)
![Github Last commit](https://img.shields.io/github/last-commit/mk3008/Carbunql)  
[![SqModel](https://img.shields.io/nuget/v/Carbunql.svg)](https://www.nuget.org/packages/Carbunql/) 
[![SqModel](https://img.shields.io/nuget/dt/Carbunql.svg)](https://www.nuget.org/packages/Carbunql/) 

This C# library can convert select queries into object models, and also convert object models back into SQL select queries.

This library allows you to dynamically modify columns, search conditions, and even CTEs (Common Table Expressions) for select queries, dramatically increasing the reusability of your select queries.

## Demo 1: Dynamic Filtering

```cs
using Carbunql;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Enter minimum price (or leave blank to omit):");
        string? minPriceInput = Console.ReadLine();
        decimal? minPrice = string.IsNullOrEmpty(minPriceInput) ? null : Convert.ToDecimal(minPriceInput);

        Console.WriteLine("Enter maximum price (or leave blank to omit):");
        string? maxPriceInput = Console.ReadLine();
        decimal? maxPrice = string.IsNullOrEmpty(maxPriceInput) ? null : Convert.ToDecimal(maxPriceInput);

        Console.WriteLine("Enter category (or leave blank to omit):");
        string? category = Console.ReadLine();

        Console.WriteLine("Enter in-stock status (true/false) (or leave blank to omit):");
        string? inStockInput = Console.ReadLine();
        bool? inStock = string.IsNullOrEmpty(inStockInput) ? null : Convert.ToBoolean(inStockInput);

        var query = GenerateProductQuery(minPrice, maxPrice, category, inStock);
        Console.WriteLine("Generated SQL Query:");
        Console.WriteLine(query);
    }

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

        // Convert the selection query to an object
        var sq = new SelectQuery(sql);

        // Dynamically add search conditions
        if (minPrice != null)
        {
            sq.AddWhere("price", (source, column) => $"{source.Alias}.{column} >= {minPrice.Value}");
        }
        if (maxPrice != null)
        {
            sq.AddWhere("price", (source, column) => $"{source.Alias}.{column} <= {maxPrice.Value}");
        }
        if (!string.IsNullOrEmpty(category))
        {
            // Parameterize string values before adding them to search conditions
            var pname = ":category";
            sq.AddParameter(new QueryParameter(pname, category))
                .AddWhere("category", (source, column) => $"{source.Alias}.{column} = {pname}");
        }
        if (inStock != null)
        {
            sq.AddWhere("in_stock", (source, column) => $"{source.Alias}.{column} = {inStock.Value}");
        }
        return sq.ToText();
    }
}
```


### Example

```
Enter minimum price (or leave blank to omit):

Enter maximum price (or leave blank to omit):
100
Enter category (or leave blank to omit):
tea
Enter in-stock status (true/false) (or leave blank to omit):
true
Generated SQL Query:
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

## Demo 2: Dynamic column selection
```cs
using Carbunql;

public class Program
{
    public static void Main()
    {
        // Define available columns
        var availableColumns = new Dictionary<string, string>
        {
            { "1", "customer_name" },
            { "2", "email" },
            { "3", "purchase_history" }
        };

        Console.WriteLine("Available columns to select:");
        foreach (var column in availableColumns)
        {
            Console.WriteLine($"{column.Key}: {column.Value}");
        }

        Console.WriteLine("Enter the numbers of the columns you want to include, separated by commas (e.g., 1,2):");
        string? input = Console.ReadLine();
        var selectedColumnNumbers = input?.Split(',', StringSplitOptions.RemoveEmptyEntries);

        var selectedColumns = new List<string>();
        if (selectedColumnNumbers != null)
        {
            foreach (var number in selectedColumnNumbers)
            {
                if (availableColumns.TryGetValue(number.Trim(), out var column))
                {
                    selectedColumns.Add(column);
                }
            }
        }

        var query = GenerateCustomReportQuery(selectedColumns);
        Console.WriteLine("Generated SQL Query:");
        Console.WriteLine(query);
    }

    public static string GenerateCustomReportQuery(List<string> columns)
    {
        var sql = """
            SELECT
                customer_name,
                email,
                purchase_history
            FROM
                customer
            """;

        // Convert the query to an object
        var sq = new SelectQuery(sql);

        // Restrict the selected columns
        sq.FilterInColumns(columns);

        // Convert the query to text format
        return sq.ToText();
    }
}
```

### Example
```
Available columns to select:
1: customer_name
2: email
3: purchase_history
Enter the numbers of the columns you want to include, separated by commas (e.g., 1,2):
1,3
Generated SQL Query:
SELECT
    customer_name,
    purchase_history
FROM
    customer
```

## Demo 3: Dynamic CTE creation


```cs
using Carbunql;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Which month to summarize? (yyyy-mm-dd)");
        DateTime summaryMonth = Convert.ToDateTime(Console.ReadLine());

        Console.WriteLine("Include monthly summary rows? (true/false)");
        bool includeMonthly = Convert.ToBoolean(Console.ReadLine());

        var query = GenerateReportQuery(includeMonthly, summaryMonth);
        Console.WriteLine("Generated SQL Query:");
        Console.WriteLine(query);
    }

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
        var sq = new SelectQuery();
        sq.AddCTEQuery(dailySummaryQuery, "daily_summary");
        sq.AddFrom("daily_summary", "d");
        sq.AddSelectAll("d");

        if (includeSummary)
        {
            // Add monthly summary query with UNION ALL
            sq.AddSelectQuery("union all", _ =>
            {
                var xsq = new SelectQuery();
                xsq.AddCTEQuery(monthlySummaryQuery, "monthly_summary");
                xsq.AddFrom("monthly_summary", "m");
                xsq.AddSelectAll("m");
                return xsq;
            });
        }

        // Add date filter condition
        var pname = ":sale_date";
        sq.AddParameter(new QueryParameter(pname, summaryMonth))
            .AddWhere("sale_date", (source, column) => $"{pname} <= {source.Alias}.{column} and {source.Alias}.{column} < {pname}::timestamp + '1 month'");

        // Convert the entire query to a CTE
        sq = sq.ToCTEQuery("final", "f");

        // Add sorting conditions
        sq.RemoveSelect("sort_number")
            .AddOrder("sale_date", (source, column) => $"{source.Alias}.{column}")
            .AddOrder("sort_number", (source, column) => $"{source.Alias}.{column}");

        return sq.ToText();
    }
}
```

### Example

```
Which month to summarize? (yyyy-mm-dd)
2024-08-01
Include monthly summary rows? (true/false)
true
Generated SQL Query:
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

- You can model select queries and perform advanced editing.
- The model can be written back to select, create table, add, update, merge, and delete queries.
- No DBMS reference.
- No configuration or entity classes required.

You can try out some of the processing on the online demo site.

https://mk3008.github.io/Carbunql/

![image](https://github.com/user-attachments/assets/21ddb1af-ae13-4405-a2c9-40b15f372e84)

## Constraints

- Syntax checking is modest.
- Comments are removed when modeling.
- No mapper function.

If you want to execute queries or perform mapping, use "Carbunql.Dapper" and use it together with Dapper.

https://www.nuget.org/packages/Carbunql.Dapper

## Getting started
Install the package from NuGet.

```
PM> Install-Package Carbunql
```

https://www.nuget.org/packages/Carbunql/

### Model a select query

Just pass the select query string to the constructor of the SelectQuery class.

```cs
using Carbunql;

var text = "select s.sale_id, s.store_id, date_trunc('month', s.sale_date) as allocate_ym, s.sale_price from sales as s";
var sq = new SelectQuery(text);
```

### Return the model to a select query

Use the `ToText` or `ToOneLineText` method.

The `ToText` method will return a formatted select query. Parameter information will also be added as a comment.

The `ToOneLineText` method will output a single line without formatting. Use the ToOneLineText method if performance is important.

```cs
using Carbunql;

var text = "select s.sale_id, s.store_id, date_trunc('month', s.sale_date) as allocate_ym, s.sale_price from sales as s";
var sq = new SelectQuery(text);
var query = sq.ToOneLineText();
```

### Create an empty select query

If you do not specify arguments in the constructor, a model without SELECT and FROM clauses will be created. Please add SELECT and FROM clauses manually.

```cs
using Carbunql;

var sq = new SelectQuery();
```

### Add a FROM clause

If you added an empty select query, use the `AddFrom` function to manually add a FROM clause. The first argument is the table name, and the second argument is the alias name.

```cs
using Carbunql;

var sq = new SelectQuery();
sq.AddFrom("customer", "c");
```

### Add a column to select

You can add a column to select by using the `AddSelect` function. The first argument is the column name, and the second argument is the column alias name. The column alias name is optional.

```cs
using Carbunql;

var sq = new SelectQuery();
sq.AddFrom("customer", "c");
sq.AddSelect("c.customer_id")
  .AddSelect("c.first_name || c.last_name", "customer_name");
```

### Add search conditions

You can add search conditions by using the `AddWhere` method.

The first argument is the name of the column to which you want to add a condition.

The second argument is a delegate (or lambda expression) that takes the column source and column name as input and generates the condition part of the SQL.

```cs
using Carbunql;

var text = "select s.sale_id, s.store_id, date_trunc('month', s.sale_date) as allocate_ym, s.sale_price from sales as s";
var sq = new SelectQuery(text);
sq.AddWhere("sale_id", (source, column) => $"{source.Alias}.{column} = 1");
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
