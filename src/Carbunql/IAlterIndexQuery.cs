using Carbunql.Building;
using Carbunql.Definitions;

namespace Carbunql;

/// <summary>
/// Represents a query for altering an index.
/// </summary>
public interface IAlterIndexQuery : IQueryCommandable, ICommentable, ITable
{
    /// <summary>
    /// Gets the name of the index.
    /// </summary>
    string? IndexName { get; }
}
