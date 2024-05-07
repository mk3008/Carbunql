using Carbunql.Clauses;
using Carbunql.Tables;

namespace Carbunql.Building;

/// <summary>
/// Static class providing extension methods for common table expressions (CTEs).
/// </summary>
public static class CommonTableExtension
{
    /// <summary>
    /// Sets the alias for the common table expression (CTE).
    /// </summary>
    /// <param name="source">The common table expression.</param>
    /// <param name="alias">The alias to set.</param>
    /// <returns>The modified common table expression.</returns>
    public static CommonTable As(this CommonTable source, string alias)
    {
        source.SetAlias(alias);
        return source;
    }

    /// <summary>
    /// Converts the common table expression (CTE) to a physical table.
    /// </summary>
    /// <param name="source">The common table expression.</param>
    /// <returns>A physical table.</returns>
    public static PhysicalTable ToPhysicalTable(this CommonTable source)
    {
        var t = new PhysicalTable(source.Alias);
        t.ColumnNames ??= new List<string>();
        foreach (var item in source.GetColumnNames())
        {
            t.ColumnNames.Add(item);
        }
        return t;
    }
}
