using Carbunql.Analysis.Parser;
using Carbunql.Clauses;
using Carbunql.Definitions;

namespace Carbunql.Annotations;

/// <summary>
/// Factory class for generating index queries.
/// </summary>
public static class CreateIndexQueryFactory
{
    /// <summary>
    /// Generates index queries based on the properties of the specified type from the given table information.
    /// </summary>
    /// <typeparam name="T">Type of the object for which indexes are created.</typeparam>
    /// <param name="t">Table information for creating indexes.</param>
    /// <returns>List of generated index queries.</returns>
    public static List<CreateIndexQuery> Creates<T>(ITable t)
    {
        var queries = new List<CreateIndexQuery>();

        var props = PropertySelector.SelectParentProperties<T>();

        foreach (var prop in props)
        {
            var clause = new IndexOnClause(t);

            ColumnDefinitionFactory.CreateAsParentProperties(t, prop)
                .Select(x => x.ColumnName).ToList()
                .ForEach(x => clause.Add(SortableItemParser.Parse(x)));

            var query = new CreateIndexQuery(clause)
            {
                IndexName = DbmsConfiguration.GetDefaultIndexNameLogic(prop.Name)
            };

            queries.Add(query);
        }

        return queries;
    }
}
