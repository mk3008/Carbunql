using Carbunql.Clauses;
using Carbunql.Tables;

namespace Carbunql.Building;

public static class CommonTableExtension
{
    public static CommonTable As(this CommonTable source, string alias)
    {
        source.SetAlias(alias);
        return source;
    }

    public static PhysicalTable ToPhysicalTable(this CommonTable source)
    {
        var t = new PhysicalTable(source.Alias);
        t.ColumnNames ??= new();
        foreach (var item in source.GetColumnNames()) t.ColumnNames.Add(item);
        return t;
    }
}