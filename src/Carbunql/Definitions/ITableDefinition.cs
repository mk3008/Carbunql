using Carbunql.Clauses;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Definitions;

public interface ITableDefinition : IQueryCommandable, ITable
{
    bool TrySet(TableDefinitionClause clause);

    //bool TryToIndex([MaybeNullWhen(false)] out CreateIndexQuery query);

    bool TryDisasseble([MaybeNullWhen(false)] out IConstraint constraint);

    string ColumnName { get; }
}
