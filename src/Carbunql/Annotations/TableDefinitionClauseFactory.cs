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
        var clause = Create(typeof(T));

        ColumnDefinitionFactory.Create<T>(clause).ToList().ForEach(x => clause.Add(x));

        clause.Add(PrimaryKeyConstraintFactory.Create(typeof(T)));

        return clause;
    }

    internal static TableDefinitionClause Create(Type type)
    {
        var info = TableInfoFactory.Create(type);

        var clause = new TableDefinitionClause(info.Schema, info.Table);

        return clause;
    }
}
