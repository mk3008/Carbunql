using Carbunql.Clauses;

namespace Carbunql.Annotations;

public static class TableDefinitionClauseFactory
{
    /// <summary>
    /// Creates a table definition for the specified type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of class for which to create the table definition.</typeparam>
    /// <returns>The table definition clause for the specified type.</returns>
    public static TableDefinitionClause Create<T>()
    {
        var atr = (TableAttribute?)Attribute.GetCustomAttribute(typeof(T), typeof(TableAttribute));

        TableDefinitionClause clause;
        if (atr != null)
        {
            clause = CreateTableDefinitionClause<T>(atr.Schema, atr.Table);
        }
        else
        {
            clause = CreateTableDefinitionClause<T>(string.Empty, string.Empty);
        }

        ColumnDefinitionFactory.Create<T>(clause).ToList().ForEach(x => clause.Add(x));

        clause.Add(PrimaryKeyConstraintFactory.Create<T>());

        return clause;
    }


    private static TableDefinitionClause CreateTableDefinitionClause<T>(string schema, string table)
    {
        if (string.IsNullOrEmpty(schema))
        {
            schema = DbmsConfiguration.ConvertToDefaultSchemaNameLogic(typeof(T));
        }
        if (string.IsNullOrEmpty(table))
        {
            table = DbmsConfiguration.ConvertToDefaultTableNameLogic(typeof(T));
        }
        if (string.IsNullOrEmpty(table))
        {
            table = DbmsConfiguration.ConvertToDefaultTableNameLogic(typeof(T));
        }

        var clause = new TableDefinitionClause(schema, table);

        return clause;
    }
}
