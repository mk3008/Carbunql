using Carbunql.Clauses;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Definitions;

public interface IAlterCommand : IQueryCommandable, ITable
{
	bool TrySet(TableDefinitionClause clause);

	//bool TryToIndex([MaybeNullWhen(false)] out CreateIndexQuery query);
}
