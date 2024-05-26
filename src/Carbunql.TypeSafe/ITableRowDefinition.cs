using Carbunql.Annotations;

namespace Carbunql.TypeSafe;

public interface ITableRowDefinition
{
    [IgnoreMapping]
    IDatasource Datasource { get; set; }
}
