using Carbunql.Tables;

namespace Carbunql.Clauses;

public class IndexOnClause : QueryCommandCollection<SortableItem>
{
	public IndexOnClause(string table)
	{
		Table = table;
	}

	public IndexOnClause(string schema, string table)
	{
		Schema = schema;
		Table = table;
	}

	public string? Schema { get; init; } = null;

	public string Table { get; init; }

	public string TableFullName => (string.IsNullOrEmpty(Schema)) ? Table : Schema + "." + Table;

	public override IEnumerable<Token> GetTokens(Token? parent)
	{
		if (!Items.Any()) yield break;

		var clause = Token.Reserved(this, parent, "on");
		yield return clause;
		yield return new Token(this, parent, TableFullName);

		yield return Token.ReservedBracketStart(this, parent);
		foreach (var item in base.GetTokens(clause)) yield return item;
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