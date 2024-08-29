using Carbunql.Building;
using Carbunql.Extensions;

namespace Carbunql.Fluent;

public static class SelectQueryConvertExtension
{
    /// <summary>
    /// Converts the IReadQuery to a CreateTableQuery with the specified table name and optional flag for temporary table.
    /// </summary>
    /// <param name="source">The source IReadQuery.</param>
    /// <param name="table">The name of the table.</param>
    /// <param name="isTemporary">A flag indicating whether the table is temporary (default is true).</param>
    /// <returns>The CreateTableQuery representing the IReadQuery with the specified table name and temporary flag.</returns>
    public static CreateTableQuery ToCreateTableQuery(this SelectQuery source, string table, bool isTemporary = true)
    {
        var t = table.ToPhysicalTable();
        return source.ToCreateTableQuery(t, isTemporary);
    }
}
