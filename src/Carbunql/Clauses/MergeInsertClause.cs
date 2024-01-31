using Carbunql.Tables;
using Carbunql.Values;

namespace Carbunql.Clauses;

public class MergeInsertClause : IQueryCommandable
{
	public MergeInsertClause(ValueCollection columnAliases)
	{
		ColumnAliases = columnAliases;
	}

	public ValueCollection ColumnAliases { get; init; }

	public virtual IEnumerable<Token> GetTokens(Token? parent)
	{
		var t = Token.Reserved(this, parent, "insert");

		var bracket = Token.ReservedBracketStart(this, t);
		yield return bracket;
		foreach (var item in ColumnAliases.GetTokens(bracket)) yield return item;
		yield return Token.ReservedBracketEnd(this, parent);
	}

	public virtual IEnumerable<QueryParameter> GetParameters()
	{
		return ColumnAliases.GetParameters();
	}

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		foreach (var item in ColumnAliases.GetInternalQueries())
		{
			yield return item;
		}
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		foreach (var item in ColumnAliases.GetPhysicalTables())
		{
			yield return item;
		}
	}

	public IEnumerable<CommonTable> GetCommonTables()
	{
		foreach (var item in ColumnAliases.GetCommonTables())
		{
			yield return item;
		}
	}
}