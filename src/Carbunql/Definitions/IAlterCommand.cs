using Carbunql.Clauses;

namespace Carbunql.Definitions;

public interface IAlterCommand : IQueryCommandable, ITable
{
	bool TrySet(TableDefinitionClause clause);

	//bool TryToIndex([MaybeNullWhen(false)] out CreateIndexQuery query);
}
