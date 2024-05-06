namespace Carbunql.Annotations;

/// <summary>
/// Factory class for creating definition query sets.
/// </summary>
public static class DefinitionQuerySetFactory
{
    /// <summary>
    /// Creates a definition query set for the specified type.
    /// </summary>
    /// <typeparam name="T">The type for which the definition query set is created.</typeparam>
    /// <returns>The created definition query set.</returns>
    public static DefinitionQuerySet Create<T>()
    {
        var q = CreateTableQueryFactory.Create<T>();
        var qs = new DefinitionQuerySet(q);

        CreateIndexQueryFactory.Creates<T>(q).ForEach(x => qs.AddAlterIndexQuery(x));

        qs = qs.ToNormalize();

        return qs;
    }
}
