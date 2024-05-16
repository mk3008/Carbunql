using Carbunql.Annotations;
using Carbunql.Clauses;

namespace Carbunql.TypeSafe;

public interface ITableRowDefinition
{
    [IgnoreMapping]
    TableDefinitionClause TableDefinition { get; set; }
}
