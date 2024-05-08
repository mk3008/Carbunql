namespace Carbunql.Clauses;

/// <summary>
/// Represents an interface for selectable items in a SQL query.
/// </summary>
public interface ISelectable
{
    /// <summary>
    /// Gets the alias associated with the selectable item.
    /// </summary>
    string Alias { get; }
}
