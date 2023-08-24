using Carbunql.Clauses;
using MessagePack;
using System.Collections.Immutable;

namespace Carbunql.Tables;

[MessagePackObject]
public class PhysicalTable : TableBase
{
	public PhysicalTable()
	{
		Table = string.Empty;
	}

	public PhysicalTable(string table)
	{
		Table = table;
	}

	public PhysicalTable(string schema, string table)
	{
		Schame = schema;
		Table = table;
	}

	[Key(1)]
	public string? Schame { get; init; }

	[Key(2)]
	public string Table { get; init; }

	[Key(3)]
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

	public override string GetTableFullName() => !string.IsNullOrEmpty(Schame) ? Schame + "." + Table : Table;

	public override IList<string> GetColumnNames()
	{
		if (ColumnNames == null) return ImmutableList<string>.Empty;
		return ColumnNames;
	}

	public override IEnumerable<SelectQuery> GetInternalQueries()
	{
		yield break;
	}

	public override IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		yield return this;
	}
}