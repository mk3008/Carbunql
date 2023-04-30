using Carbunql.Clauses;

namespace Carbunql.Values;

public class LiteralValue : ValueBase
{
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

	public override IEnumerable<Token> GetCurrentTokens(Token? parent)
	{
		yield return new Token(this, parent, CommandText);
	}
}