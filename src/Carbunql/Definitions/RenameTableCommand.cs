using Carbunql.Clauses;
using Carbunql.Tables;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Definitions;

public class RenameTableCommand : IAlterCommand
{
	public RenameTableCommand(ITable t, string newTableName)
	{
		NewTableName = newTableName;
		Schema = t.Schema;
		Table = t.Table;
	}

	public string NewTableName { get; set; }

	public string? Schema { get; init; }

	public string Table { get; init; } = string.Empty;

	public IEnumerable<CommonTable> GetCommonTables()
	{
		yield break;
	}

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		yield break;
	}

	public IEnumerable<QueryParameter> GetParameters()
	{
		yield break;
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		yield break;
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		yield return new Token(this, parent, "rename", isReserved: true);
		yield return new Token(this, parent, "to", isReserved: true);
		yield return new Token(this, parent, NewTableName);
	}

	public bool TrySet(TableDefinitionClause clause)
	{
		return false;
	}

	public bool TryToIndex([MaybeNullWhen(false)] out CreateIndexQuery query)
	{
		query = default;
		return false;
	}
}