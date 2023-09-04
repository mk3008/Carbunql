using Carbunql.Clauses;
using Carbunql.Tables;
using Carbunql.Values;

namespace Carbunql;

public class Token
{
	public static Token ReservedBracketStart(object sender, Token? parent)
	{
		return new Token(sender, parent, "(", true);
	}

	public static Token ReservedBracketEnd(object sender, Token? parent)
	{
		return new Token(sender, parent, ")", true);
	}

	public static Token ExpressionBracketStart(object sender, Token? parent)
	{
		return new Token(sender, parent, "(", false);
	}

	public static Token ExpressionBracketEnd(object sender, Token? parent)
	{
		return new Token(sender, parent, ")", false);
	}

	public static Token Dot(object sender, Token? parent)
	{
		return new Token(sender, parent, ".", false);
	}

	public static Token Comma(object sender, Token? parent)
	{
		return new Token(sender, parent, ",", false);
	}

	public static Token Reserved(object sender, Token? parent, string text)
	{
		return new Token(sender, parent, text, true);
	}

	public Token(object sender, Token? parent, string text, bool isReserved = false)
	{
		Sender = sender;
		Parent = parent;
		Text = text;
		IsReserved = isReserved;
	}

	public object Sender { get; init; }

	public Token? Parent { get; init; }

	public string Text { get; init; }

	public bool IsReserved { get; init; }

	public bool NeedsSpace(Token? prev = null)
	{
		if (prev == null) return false;

		if (prev!.Text.Equals("(")) return false;
		if (prev!.Text.Equals(".")) return false;
		if (prev!.Text.Equals("::")) return false;
		if (prev!.Text.Equals("as")) return true;

		if (Text.Equals(")")) return false;
		if (Text.Equals(",")) return false;
		if (Text.Equals(".")) return false;
		if (Text.Equals("("))
		{
			if (Sender is VirtualTable) return true;
			if (Sender is FunctionTable) return false;
			if (Sender is FunctionValue) return false;
			if (Sender is Filter) return false;
			if (Sender is WindowDefinition) return false;
			return true;
		}
		if (Text.Equals("::")) return false;

		return true;
	}

	public IEnumerable<Token> Parents()
	{
		if (Parent == null) yield break;
		yield return Parent;
		foreach (var item in Parent.Parents()) yield return item;
	}
}