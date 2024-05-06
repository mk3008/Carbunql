using Carbunql.Clauses;

namespace Carbunql.Annotations;

/// <summary>
/// Factory class for creating table definition clauses.
/// </summary>
public static class TableDefinitionClauseFactory
{
    /// <summary>
    /// Creates a table definition for the specified type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of class for which to create the table definition.</typeparam>
    /// <returns>The table definition clause for the specified type.</returns>
    public static TableDefinitionClause Create<T>()
    {
        var clause = CreateTableDefinitionClause(typeof(T));

        ColumnDefinitionFactory.Create<T>(clause).ToList().ForEach(x => clause.Add(x));

        clause.Add(PrimaryKeyConstraintFactory.Create(typeof(T)));

        return clause;
    }

    /// <summary>
    /// Creates a table definition clause for the specified type and schema/table names.
    /// </summary>
    /// <param name="type">The type of class for which to create the table definition.</param>
    /// <param name="schema">The schema name.</param>
    /// <param name="table">The table name.</param>
    /// <returns>The table definition clause for the specified type and names.</returns>
    internal static TableDefinitionClause CreateTableDefinitionClause(Type type)
    {
        var atr = (TableAttribute?)Attribute.GetCustomAttribute(type, typeof(TableAttribute));

        TableDefinitionClause clause;
        if (atr != null)
        {
            clause = CreateTableDefinitionClause(type, atr.Schema, atr.Table);
        }
        else
        {
            clause = CreateTableDefinitionClause(type, string.Empty, string.Empty);
        }

        return clause;
    }

    /// <summary>
    /// Creates a table definition clause with the specified type, schema, and table names.
    /// </summary>
    /// <param name="type">The type of class for which to create the table definition.</param>
    /// <param name="schema">The schema name.</param>
    /// <param name="table">The table name.</param>
    /// <returns>The table definition clause with the specified type, schema, and table names.</returns>
    private static TableDefinitionClause CreateTableDefinitionClause(Type type, string schema, string table)
    {
        if (string.IsNullOrEmpty(schema))
        {
            schema = DbmsConfiguration.ConvertToDefaultSchemaNameLogic(type);
        }
        if (string.IsNullOrEmpty(table))
        {
            table = DbmsConfiguration.ConvertToDefaultTableNameLogic(type);
        }
        if (string.IsNullOrEmpty(table))
        {
            table = DbmsConfiguration.ConvertToDefaultTableNameLogic(type);
        }

        var clause = new TableDefinitionClause(schema, table);

        return clause;
    }
}
