# Carbunql.Postgres
Carbunql.Postgres is a type-safe select query builder.

Note:
This is an experimental library. Specifications are subject to significant changes.

## Demo
Let's build a simple SQL statement using table joins and Where conditions.

### Definition 

Declare the table definition class.The class must have a RecordDefinition attribute.

```cs
[RecordDefinition]
public record struct table_a(int a_id, string text, int value);

[RecordDefinition]
public record struct table_b(int a_id, int b_id, string text, int value);
```

### Buidling

Build the select query.
```cs
// Define an empty select query
SelectQuery sq = new SelectQuery();

// Specifies the table to select.
// Note: Make sure the SQL table alias name and variable name are the same
(FromClause from, table_a a) = sq.FromAs<table_a>("a");

// Write the table join expression.
// Combined expressions can be written in a type-safe manner.
// Note: Make sure that the join destination table alias name and return value variable name are the same.
table_b b = from.InnerJoinAs<table_b>("b").On(b => a.a_id == b.a_id);

// Describe the columns to select.
// If you want to get all columns, use the SelectAll method
sq.SelectAll(() => a);

// Use the Select method to select a specific column.
// You can also give it an alias using the As method.
sq.Select(() => b.b_id).As("table_b_key");

// A similar description can be made for the Where clause.
sq.Where(() => a.a_id == 1);

// Get SQL string
string sql = sq.ToCommand().CommandText;
```

### Result
```sql
SELECT
    a.a_id,
    a.text,
    a.value,
    b.b_id AS table_b_key
FROM
    table_a AS a
    INNER JOIN table_b AS b ON a.a_id = b.a_id
WHERE
    (a.a_id = 1)
```

## Features
- You can write SQL using C# syntax.
- No DBMS execution environment is required.
- Can be used in conjunction with SQL parser library Carbunql.

## Constraints
- The generated SQL statements are assumed to be executed in Postgres.
- Only select queries can be written.
- Table definition classes must be handwritten.

## Getting started
Preparing nuget package