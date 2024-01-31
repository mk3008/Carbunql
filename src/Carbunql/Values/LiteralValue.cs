using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Values;

[MessagePackObject(keyAsPropertyName: true)]
public class LiteralValue : ValueBase
{
	public LiteralValue()
	{
		CommandText = "null";
	}

	public LiteralValue(string? commandText)
	{
		if (commandText != null)
		{
			CommandText = commandText;
		}
		else
		{
			CommandText = "null";
		}
	}

	public LiteralValue(int? value)
	{
		if (value.HasValue)
		{
			CommandText = value.Value.ToString();
		}
		else
		{
			CommandText = "null";
		}
	}

	public LiteralValue(bool? value)
	{
		if (value.HasValue)
		{
			CommandText = value.Value.ToString();
		}
		else
		{
			CommandText = "null";
		}
	}

	public string CommandText { get; set; }

	public bool IsNullValue => CommandText.IsEqualNoCase("null");

	public override IEnumerable<Token> GetCurrentTokens(Token? parent)
	{
		yield return new Token(this, parent, CommandText);
	}

	protected override IEnumerable<CommonTable> GetCommonTablesCore()
	{
		yield break;
	}

	protected override IEnumerable<SelectQuery> GetInternalQueriesCore()
	{
		yield break;
	}

	protected override IEnumerable<QueryParameter> GetParametersCore()
	{
		yield break;
	}

	protected override IEnumerable<PhysicalTable> GetPhysicalTablesCore()
	{
		yield break;
	}
}