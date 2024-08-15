using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Building;

/// <summary>
/// Extension methods for the <see cref="SelectClause"/> class.
/// </summary>
public static class SelectClauseExtension
{
    /// <summary>
    /// Converts a <see cref="ColumnValue"/> to a <see cref="SelectableItem"/>.
    /// </summary>
    /// <param name="source">The source <see cref="ColumnValue"/>.</param>
    /// <returns>The <see cref="SelectableItem"/> representing the column.</returns>
    public static SelectableItem ToSelectable(this ColumnValue source)
    {
        return new SelectableItem(source, source.GetDefaultName());
    }

    /// <summary>
    /// Converts a <see cref="ValueBase"/> to a <see cref="SelectableItem"/> with a specified name.
    /// </summary>
    /// <param name="source">The source <see cref="ValueBase"/>.</param>
    /// <param name="name">The name of the selectable item.</param>
    /// <returns>The <see cref="SelectableItem"/> representing the value.</returns>
    public static SelectableItem ToSelectable(this ValueBase source, string name = "column")
    {
        return new SelectableItem(source, name);
    }

    /// <summary>
    /// Sets the alias for the <see cref="SelectableItem"/>.
    /// </summary>
    /// <param name="source">The source <see cref="SelectableItem"/>.</param>
    /// <param name="alias">The alias to set.</param>
    public static void As(this SelectableItem source, string alias)
    {
        source.SetAlias(alias);
    }
}
