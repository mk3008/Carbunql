# Carbunql
![GitHub](https://img.shields.io/github/license/mk3008/Carbunql)
![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/mk3008/Carbunql)
![Github Last commit](https://img.shields.io/github/last-commit/mk3008/Carbunql)  
[![SqModel](https://img.shields.io/nuget/v/Carbunql.svg)](https://www.nuget.org/packages/Carbunql/) 
[![SqModel](https://img.shields.io/nuget/dt/Carbunql.svg)](https://www.nuget.org/packages/Carbunql/) 

This C# library provides a feature to convert a selection query into an object. By objectifying, it becomes easier to modify the selection query and perform more complex manipulations such as adding join expressions. 

Using this library allows for a more versatile use of existing selection queries.

## Demo

This code adds an inner join expression to an existing select query and also adds a where condition.

Note that no DBMS is required to run this demo.

```cs
using Carbunql;
using Carbunql.Building;
using Carbunql.Clauses;

// Convert select query to SelectQuery class.
SelectQuery sq = new SelectQuery("select s.sale_id, s.shop_id, s.sale_price from sales s");

/*
 Use the "ToCommand" method to convert the SelectQuery class to a SQL statement.

    SELECT
        s.sale_id,
        s.sale_price
    FROM
        sales AS s
 */
Console.WriteLine(sq.ToCommand().CommandText);

//Getting the From clause.
FromClause from = sq.FromClause!;

//Get the root table defined in the From clause.
SelectableTable s = from.Root;

//Inner join with "shops" master.
//The column used in the join expression is "shop_id".
SelectableTable sh = from.InnerJoin("shops").As("sh").On(s, "shop_id");

//Add column "shop_name" in "shops" master to select columns.
sq.Select(sh, "shop_name");

//Added extraction condition to where clause.
string parameterName = sq.AddParameter(":shop_id", 1);
sq.Where(sh, "shop_id").Equal(parameterName);

/*
 The result written back to the select query.

	SELECT
	    s.sale_id,
	    s.shop_id,
	    s.sale_price,
	    sh.shop_name
	FROM
	    sales AS s
	    INNER JOIN shops AS sh ON s.shop_id = sh.shop_id
	WHERE
	    sh.shop_id = :shop_id
*/
Console.WriteLine(sq.ToCommand().CommandText);

/*
 You can also get parameters from the ToCommand method.
 
 　　:shop_id = 1
 */
foreach (KeyValuePair<string, object?> prm in sq.ToCommand().Parameters)
{
	Console.WriteLine($"{prm.Key} = {prm.Value}");
}

/*
 If you use "Carbunql.Dapper", you can execute SQL as SelectQuery class.
 https://www.nuget.org/packages/Carbunql.Dapper
 */
//var cn = IDbConnection;
//cn.Execute(sq);
```

It is also possible to convert existing queries into subqueries and CTEs.
Additionally, you can convert them to add, update, delete, and merge queries.

[Please refer to the online site for the above conversion demo](https://mk3008.github.io/Carbunql).

![demosite screenshot](https://user-images.githubusercontent.com/7686540/218080149-27085450-563a-4706-8ae4-5fb365c090f1.png)

## Features
- Supports parsing select queries without using a DBMS
- DBMS agnostic
- Supports processing select queries

## Constraints
- Minimum grammar check
- Only select queries can be parsed
- Comment is removed

If you want to execute modified queries, please use the [Dapper](https://github.com/DapperLib/Dapper) library "[Carbunql.Dapper](https://www.nuget.org/packages/Carbunql.Dapper)".

# Getting started

> PM> Install-Package [Carbunql](https://www.nuget.org/packages/Carbunql/)

## Parse
Just pass the select query string to the constructor of the SelectQuery class.
```cs
using Carbunql;

var text = @"
select a.column_1 as col1, a.column_2 as col2
from table_a as a
left join table_b as b on a.id = b.table_a_id
where b.table_a_id is null
";

var sq = new SelectQuery(text);
string sql = sq.ToCommand().CommandText;
```

```sql
SELECT
    a.column_1 AS col1,
    a.column_2 AS col2
FROM
    table_a AS a
    LEFT JOIN table_b AS b ON a.id = b.table_a_id
WHERE
    b.table_a_id IS null
```

## Building
You can build using the SelectQuery class.
```cs
using Carbunql;
using Carbunql.Building;

var sq = new SelectQuery();

// from clause
var (from, a) = sq.From("table_a").As("a");
var b = from.InnerJoin("table_b").As("b").On(a, "table_a_id");
var c = from.LeftJoin("table_c").As("c").On(b, "table_b_id");

// select clause
sq.Select(a, "id").As("a_id");
sq.Select(b, "table_a_id").As("b_id");

// where clause
sq.Where(a, "id").Equal(":id").And(b, "is_visible").True().And(c, "table_b_id").IsNull();

// parameter
sq.Parameters.Add(":id", 1);

string sql = sq.ToCommand().CommandText;
```

```sql
/*
    :id = 1
*/
SELECT
    a.id AS a_id,
    b.table_a_id AS b_id
FROM
    table_a AS a
    INNER JOIN table_b AS b ON a.table_a_id = b.table_a_id
    LEFT JOIN table_c AS c ON b.table_b_id = c.table_b_id
WHERE
    a.id = :id
    AND b.is_visible = true
    AND c.table_b_id IS null
```

## Build subquery
```cs
using Carbunql;
using Carbunql.Building;

var sq = new SelectQuery();
sq.From(() =>
{
    var x = new SelectQuery();
    x.From("table_a").As("a");
    x.SelectAll();
    return x;
}).As("b");
sq.SelectAll();

string sql = sq.ToCommand().CommandText;
```

```sql
SELECT
    *
FROM
    (
        SELECT
            *
        FROM
            table_a AS a
    ) AS b
```

## Build condition
```cs
using Carbunql;
using Carbunql.Building;
using Carbunql.Values;

var sq = new SelectQuery();
var (from, a) = sq.From("table_a").As("a");
sq.SelectAll();

sq.Where(() =>
{
    // a.id = 1 and a.value = 2
    var c1 = new ColumnValue(a, "id").Equal(1);
    c1.And(a, "value").Equal(2));

    // a.value = 3 and a.value = 4
    var c2 = new ColumnValue(a, "id").Equal(3);
    c2.And(a, "value").Equal(4);

    // (
    //     (a.id = 1 and a.value = 2)
    //     or
    //     (a.value = 3 and a.value = 4)
    // )
    return c1.ToGroup().Or(c2.ToGroup()).ToGroup();
});

string sql = sq.ToCommand().CommandText;
```

```sql
SELECT
    *
FROM
    table_a AS a
WHERE
    ((a.id = 1 AND a.value = 2) OR (a.id = 3 AND a.value = 4))
```

## Build exists
```cs
using Carbunql;
using Carbunql.Building;

var sq = new SelectQuery();
var (from, a) = sq.From("table_a").As("a");
sq.SelectAll();
sq.Where(() =>
{
    var x = new SelectQuery();
    var (_, b) = x.From("table_b").As("b");
    x.SelectAll();
    x.Where(b, "id").Equal(a, "id");
    return x.ToExists();
});
sq.Where(() =>
{
    var x = new SelectQuery();
    var (_, b) = x.From("table_b").As("b");
    x.SelectAll();
    x.Where(b, "id").Equal(a, "id");
    return x.ToNotExists();
});

string sql = sq.ToCommand().CommandText;
```

```sql
SELECT
    *
FROM
    table_a AS a
WHERE
    EXISTS (
        SELECT
            *
        FROM
            table_b AS b
        WHERE
            b.id = a.id
    )
    AND NOT EXISTS (
        SELECT
            *
        FROM
            table_b AS b
        WHERE
            b.id = a.id
    )
```

## Build CTE
```cs
using Carbunql;
using Carbunql.Building;

var sq = new SelectQuery();

// a as (select * from table_a)
var ct_a = sq.With(() =>
{
    var q = new SelectQuery();
    q.From("table_a");
    q.SelectAll();
    return q;
}).As("a");

// b as (select * from table_b)
var ct_b = sq.With(() =>
{
    var q = new SelectQuery();
    q.From("table_b");
    q.SelectAll();
    return q;
}).As("b");

// select * from a iner join b a.id = b.id
var (from, a) = sq.From(ct_a).As("a");
from.InnerJoin(ct_b).On(a, "id");

sq.SelectAll();

string sql = sq.ToCommand().CommandText;
```

```sql
WITH
    a AS (
        SELECT
            *
        FROM
            table_a
    ),
    b AS (
        SELECT
            *
        FROM
            table_b
    )
SELECT
    *
FROM
    a
    INNER JOIN b ON a.id = b.id
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
