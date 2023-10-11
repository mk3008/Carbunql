using Carbunql.Clauses;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Values;

[MessagePackObject(keyAsPropertyName: true)]
public class AllColumnValue : ValueBase
{
	public AllColumnValue()
	{
		TableAlias = string.Empty;
	}

	public AllColumnValue(string table)
	{
		TableAlias = table;
	}

	public AllColumnValue(FromClause from)
	{
		TableAlias = from.Root.Alias;
	}

	public AllColumnValue(SelectableTable table)
	{
		TableAlias = table.Alias;
	}

	public string TableAlias { get; set; } = string.Empty;

	public string Column => "*";

	public List<string> ActualColumns { get; set; } = new();

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

	protected override IEnumerable<CommonTable> GetCommonTablesCore()
	{
		yield break;
	}

	protected override IEnumerable<SelectQuery> GetInternalQueriesCore()
	{
		yield break;
	}

	protected override IDictionary<string, object?> GetParametersCore()
	{
		return EmptyParameters.Get();
	}

	protected override IEnumerable<PhysicalTable> GetPhysicalTablesCore()
	{
		yield break;
	}
}