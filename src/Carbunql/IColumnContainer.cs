using Carbunql.Values;

namespace Carbunql;

public interface IColumnContainer
{
    IEnumerable<ColumnValue> GetColumns();
}
