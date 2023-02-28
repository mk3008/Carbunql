using Carbunql.Clauses;
using System.Collections.Immutable;

namespace Carbunql.Tables;

public class PhysicalTable : TableBase
{
	public PhysicalTable(string table)
	{
		Table = table;
	}

	public PhysicalTable(string schema, string table)
	{
		Schame = schema;
		Table = table;
	}

	public string? Schame { get; init; }

	public string Table { get; init; }

	public List<string>? ColumnNames { get; set; }

	public override IEnumerable<Token> GetTokens(Token? parent)
	{
		if (!string.IsNullOrEmpty(Schame))
		{
			yield return new Token(this, parent, Schame);
			yield return Token.Dot(this, parent);
		}
		yield return new Token(this, parent, Table);
	}

	public override string GetDefaultName() => Table;

	public override IList<string> GetColumnNames()
	{
		if (ColumnNames == null) return ImmutableList<string>.Empty;
		return ColumnNames;
	}
}