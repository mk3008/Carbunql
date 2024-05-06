namespace Carbunql.Annotations;

/// <summary>
/// Factory class for creating "CREATE TABLE" queries.
/// </summary>
public static class CreateTableQueryFactory
{
    /// <summary>
    /// Creates a "CREATE TABLE" query for the specified type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of class for which to create the table query.</typeparam>
    /// <returns>The created "CREATE TABLE" query.</returns>
    public static CreateTableQuery Create<T>()
    {
        var clause = TableDefinitionClauseFactory.Create<T>();
        return new CreateTableQuery(clause);
    }
}
