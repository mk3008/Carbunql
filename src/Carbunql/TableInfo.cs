using Carbunql.Definitions;

namespace Carbunql;

public class TableInfo : ITable
{
    public TableInfo(string tableName)
    {
        Table = tableName;
    }

    public string Schema { get; set; } = string.Empty;

    public string Table { get; set; }
}
