using Carbunql.Annotations;

namespace Carbunql.TypeSafe;

public interface ITableRowDefinition
{
    [IgnoreMapping]
    CreateTableQuery CreateTableQuery { get; set; }
}
