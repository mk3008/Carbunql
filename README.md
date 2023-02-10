# Carbunql
A lightweight library for parsing and building select queries. SQL can be rebuilt dynamically.

## Demo
You can experience parsing results online on the demo site.

https://mk3008.github.io/Carbunql

<img width="845" alt="demo_screenshot" src="https://user-images.githubusercontent.com/7686540/218080149-27085450-563a-4706-8ae4-5fb365c090f1.png">

## Features
- Supports parsing select queries without using a DBMS
- DBMS agnostic
- Supports processing select queries

## Constraints
- Minimum grammar check
- Only select queries can be parsed

# Sample
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
sq.WhereColumn(a, "id").Equal(":id");
sq.WhereColumn(b, "is_visible").True();
sq.WhereColumn(c, "table_b_id").IsNull();

// parameter
sq.Parameters.Add(":id", 1);

var cmd = sq.ToCommand();
DebugPrint(cmd);
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

var cmd = sq.ToCommand();
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
    c1.And(() => new ColumnValue(a, "value").Equal(2));

    // a.value = 3 and a.value = 4
    var c2 = new ColumnValue(a, "id").Equal(3);
    c2.And(() => new ColumnValue(a, "value").Equal(4));

    // (
    //     (a.id = 1 and a.value = 2)
    //     or
    //     (a.value = 3 and a.value = 4)
    // )
    return c1.ToGroup().Or(c2.ToGroup()).ToGroup();
});

var cmd = sq.ToCommand();
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
    x.WhereColumn(b, "id").Equal(a, "id");
    return x.ToExists();
});
sq.Where(() =>
{
    var x = new SelectQuery();
    var (_, b) = x.From("table_b").As("b");
    x.SelectAll();
    x.WhereColumn(b, "id").Equal(a, "id");
    return x.ToNotExists();
});

var cmd = sq.ToCommand();
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

var cq = new CTEQuery();

// a as (select * from table_a)
var ct_a = cq.With(() =>
{
    var sq = new SelectQuery();
    sq.From("table_a");
    sq.SelectAll();
    return sq;
}).As("a");

// b as (select * from table_b)
var ct_b = cq.With(() =>
{
    var sq = new SelectQuery();
    sq.From("table_b");
    sq.SelectAll();
    return sq;
}).As("b");

// get select query
var sq = cq.GetOrNewSelectQuery();

// select * from a iner join b a.id = b.id
var (from, a) = sq.From(ct_a).As("a");
from.InnerJoin(ct_b).On(a, "id");

sq.SelectAll();

var cmd = cq.ToCommand();
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
