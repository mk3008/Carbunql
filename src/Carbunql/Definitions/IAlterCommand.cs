using Carbunql.Clauses;

namespace Carbunql.Definitions;

public interface IAlterCommand : IQueryCommandable
{
	bool TryIntegrate(TableDefinitionClause clause);
}
