using Carbunql.Definitions;
using Carbunql.Tables;

namespace Carbunql.Clauses;

public class TableDefinitionClause : QueryCommandCollection<ITableDefinition>
{
	public override IEnumerable<Token> GetTokens(Token? parent)
	{
		if (!Items.Any()) yield break;

		var bracket = Token.ReservedBracketStart(this, parent);
		yield return bracket;
		foreach (var item in base.GetTokens(bracket))
		{
			yield return item;
		}
		yield return Token.ReservedBracketEnd(this, parent);
	}

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		yield break;
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		yield break;
	}

	public IEnumerable<CommonTable> GetCommonTables()
	{
		yield break;
	}


	public override IEnumerable<QueryParameter> GetParameters()
	{
		yield break;
	}
}