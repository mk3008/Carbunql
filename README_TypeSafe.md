This library is experimental.

# Carbunql.TypeSafe

This library allows for TypeSafe SQL building, enables independent definition and reuse of subqueries and CTEs, and supports unit testing without the need for tables.

## Demo

This is a demo of turning SQL into a function and reusing it.


### Defining the Model Class

First, define a table like the one below as a model class.


```cs
using Carbunql;
using Carbunql.TypeSafe;

public record Sale : IDataRow
{
    public long sale_id { get; set; }
    public string product_name { get; set; } = null!;
    public int unit_price { get; set; }
    public int quantity { get; set; }
    public double tax_rate { get; set; }

    public IDataSet DataSet { get; set; } = null!;
}
```

A model class must satisfy the following conditions:

- It must have a no-argument constructor.

- It must implement the IDataRow interface.

Implementing the IDataRow interface simply involves defining the DataSet property. No special logic is required.

### Writing a Simple Select Query

Now let's write a select query in TypeSafe.

A query that selects all rows and all columns can be written like this:

```cs
var s = Sql.DefineDataSet<Sale>();
// select query debug print
Console.WriteLine(s.ToText());
```

Running this code will generate the following SQL statement:


```sql
SELECT
    *
FROM
    sale AS s
```

### Functions in select queries


Now let's turn this select query into a function so that we can reuse it.

```cs
private static FluentSelectQuery<Sale> SelectSale()
{
    var s = Sql.DefineDataSet<Sale>();
    return Sql.From(() => s);
}
```

To call this function, you would write it like this:


```cs
Console.WriteLine(SelectSale().ToText());
```

### Modifying a Select Query

Let's get the select query that has been made into a function and add a Where clause to it.
The generated query will be published as a function named "SelectFilteredSale".


```cs
private static FluentSelectQuery<Sale> SelectFilterdSale()
{
    // Give the query a name
    var all_sale = SelectSale;
    // Define the query as a dataset and give it an alias name
    var sl = Sql.DefineDataSet(() => all_sale());

    return Sql.From(() => sl)
        .Where(() => sl.sale_id == 1);
}
```

The generated SQL looks like this:

```sql
SELECT
    *
FROM
    (
        SELECT
            *
        FROM
            sale AS s
    ) AS sl
WHERE
    sl.sale_id = 1
```

You can see that the query obtained by the function "SelectSale" is reused as a subquery.


### Injecting test data

Finally, let's inject some test data. First, define the test data as follows:

```cs
private static FluentSelectQuery<Sale> SelectSaleTestDataQuery()
{
    return new FluentSelectQuery<Sale>([
        new Sale{sale_id = 1, product_name = "apple", unit_price = 105, quantity = 5, tax_rate = 0.07},
        new Sale{sale_id = 2, product_name = "orange", unit_price = 203, quantity = 3, tax_rate = 0.07},
    ]).Comment("test data");
}	
```

This code is a select query using the Values clause and has the following meaning:

```sql
/* test data */
SELECT
    v.sale_id,
    v.product_name,
    v.unit_price,
    v.quantity,
    v.tax_rate
FROM
    (
        VALUES
            (1, CAST('apple' AS text), 105, 5, CAST(0.07 AS double precision)),
            (2, CAST('orange' AS text), 203, 3, CAST(0.07 AS double precision))
    ) AS v (
        sale_id, product_name, unit_price, quantity, tax_rate
    )
```

This is very easy because we can use the model class as is.

Now let's inject it.

```cs
private static FluentSelectQuery<Sale> SelectFilterdSaleWithTestData()
{

    var query = SelectFilterdSale();

    // Important: Give your test data the same name as your table name
    var sale = SelectSaleTestDataQuery();

    // Add test data to the WITH clause
    query.With(() => sale);

    return query;
}
```

The above code will generate the following query:

```sql
WITH
    sale AS (
        /* test data */
        SELECT
            v.sale_id,
            v.product_name,
            v.unit_price,
            v.quantity,
            v.tax_rate
        FROM
            (
                VALUES
                    (1, CAST('apple' AS text), 105, 5, CAST(0.07 AS double precision)),
                    (2, CAST('orange' AS text), 203, 3, CAST(0.07 AS double precision))
            ) AS v (
                sale_id, product_name, unit_price, quantity, tax_rate
            )
    )
SELECT
    *
FROM
    (
        SELECT
            *
        FROM
            sale AS s
    ) AS sl
WHERE
    sl.sale_id = 1
```

For the full demo code and a more complex example, see the source code below.

https://github.com/mk3008/Carbunql/blob/main/demo/TypeSafeBuild/Program.cs


## Features
- Type-safe query building
- Supports table joins
- Supports window functions
- Functions for select queries
- Import function-formed select queries with CTE or subquery
- Generate unit test queries using WITH clause
- Refactoring of various alias names (using VisualStudio functions)
- Supports adding SQL comments

## Constraints
- Minimal grammar check
- Supports Postgres as DBMS (may change in future)

# Getting started

> PM> Install-Package [Carbunql.TypeSafe](https://www.nuget.org/packages/Carbunql.TypeSafe)

