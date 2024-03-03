using Carbunql.Clauses;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Definitions;

public interface ITableDefinition : IQueryCommandable
{
	bool TryIntegrate(TableDefinitionClause clause);

	bool TryDisasseble([MaybeNullWhen(false)] out IConstraint constraint);

	string ColumnName { get; }
}
