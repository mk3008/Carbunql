namespace Carbunql.Definitions;

/// <summary>
/// Represents a database table.
/// </summary>
public interface ITable
{
    /// <summary>
    /// Gets the schema of the table.
    /// </summary>
    string Schema { get; }

    /// <summary>
    /// Gets the name of the table.
    /// </summary>
    string Table { get; }
}

/// <summary>
/// Extension methods for the <see cref="ITable"/> interface.
/// </summary>
public static class TableExtension
{
    /// <summary>
    /// Gets the full name of the table, including the schema if present.
    /// </summary>
    /// <param name="t">The table.</param>
    /// <returns>The full name of the table.</returns>
    public static string GetTableFullName(this ITable t)
    {
        return string.IsNullOrEmpty(t.Schema) ? t.Table : t.Schema + "." + t.Table;
    }
}
