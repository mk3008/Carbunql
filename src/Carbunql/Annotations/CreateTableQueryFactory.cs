namespace Carbunql.Annotations;

public static class CreateTableQueryFactory
{
    public static CreateTableQuery Create<T>()
    {
        var clause = TableDefinitionClauseFactory.Create<T>();
        return new CreateTableQuery(clause);
    }
}
