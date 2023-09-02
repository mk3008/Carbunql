using Carbunql.Clauses;
using MessagePack;

namespace Carbunql.Values;

[MessagePackObject(keyAsPropertyName: true)]
public class ColumnValue : ValueBase
{
	public ColumnValue()
	{
		Column = string.Empty;
	}

	public ColumnValue(string column)
	{
		Column = column;
	}

	public ColumnValue(string table, string column)
	{
		TableAlias = table;
		Column = column;
	}

	public ColumnValue(FromClause from, string column)
	{
		TableAlias = from.Root.Alias;
		Column = column;
	}

	public ColumnValue(SelectableTable table, string column)
	{
		TableAlias = table.Alias;
		Column = column;
	}

	public string TableAlias { get; set; } = string.Empty;

	public string Column { get; init; }

	public override IEnumerable<Token> GetCurrentTokens(Token? parent)
	{
		if (!string.IsNullOrEmpty(TableAlias))
		{
			yield return new Token(this, parent, TableAlias);
			yield return Token.Dot(this, parent);
		}
		yield return new Token(this, parent, Column);
	}

	public override string GetDefaultName()
	{
		if (OperatableValue == null) return Column;
		return string.Empty;
	}
}