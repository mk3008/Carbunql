using Carbunql.Definitions;
using Carbunql.Tables;

namespace Carbunql.Clauses;

public class IndexOnClause : QueryCommandCollection<SortableItem>, ITable
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

	public IndexOnClause(ITable t)
	{
		Schema = t.Schema;
		Table = t.Table;

	}
	public string Schema { get; init; } = null;

	public string Table { get; init; }

	//public string TableFullName => (string.IsNullOrEmpty(Schema)) ? Table : Schema + "." + Table;

	public string? Using { get; set; } = null;

	public override IEnumerable<Token> GetTokens(Token? parent)
	{
		if (!Items.Any()) yield break;

		var clause = Token.Reserved(this, parent, "on");
		yield return clause;
		yield return new Token(this, parent, this.GetTableFullName());

		if (!string.IsNullOrEmpty(Using))
		{
			yield return new Token(this, parent, "using", isReserved: true);
			yield return new Token(this, parent, Using);
		}

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