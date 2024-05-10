using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;
using Carbunql.Values;

namespace Carbunql;

/// <summary>
/// Represents a token used in parsing queries.
/// </summary>
public class Token
{
    /// <summary>
    /// Creates a new instance of the Token class representing a reserved bracket start.
    /// </summary>
    /// <param name="sender">The object that created the token.</param>
    /// <param name="parent">The parent token, if any.</param>
    /// <returns>A Token object representing the reserved bracket start.</returns>
    public static Token ReservedBracketStart(object sender, Token? parent)
    {
        return new Token(sender, parent, "(", true);
    }

    /// <summary>
    /// Creates a new instance of the Token class representing a reserved bracket end.
    /// </summary>
    /// <param name="sender">The object that created the token.</param>
    /// <param name="parent">The parent token, if any.</param>
    /// <returns>A Token object representing the reserved bracket end.</returns>
    public static Token ReservedBracketEnd(object sender, Token? parent)
    {
        return new Token(sender, parent, ")", true);
    }

    /// <summary>
    /// Creates a new instance of the Token class representing an expression bracket start.
    /// </summary>
    /// <param name="sender">The object that created the token.</param>
    /// <param name="parent">The parent token, if any.</param>
    /// <returns>A Token object representing the expression bracket start.</returns>
    public static Token ExpressionBracketStart(object sender, Token? parent)
    {
        return new Token(sender, parent, "(", false);
    }

    /// <summary>
    /// Creates a new instance of the Token class representing an expression bracket end.
    /// </summary>
    /// <param name="sender">The object that created the token.</param>
    /// <param name="parent">The parent token, if any.</param>
    /// <returns>A Token object representing the expression bracket end.</returns>
    public static Token ExpressionBracketEnd(object sender, Token? parent)
    {
        return new Token(sender, parent, ")", false);
    }

    /// <summary>
    /// Creates a new instance of the Token class representing a dot token.
    /// </summary>
    /// <param name="sender">The object that created the token.</param>
    /// <param name="parent">The parent token, if any.</param>
    /// <returns>A Token object representing the dot token.</returns>
    public static Token Dot(object sender, Token? parent)
    {
        return new Token(sender, parent, ".", false);
    }

    /// <summary>
    /// Creates a new instance of the Token class representing a comma token.
    /// </summary>
    /// <param name="sender">The object that created the token.</param>
    /// <param name="parent">The parent token, if any.</param>
    /// <returns>A Token object representing the comma token.</returns>
    public static Token Comma(object sender, Token? parent)
    {
        return new Token(sender, parent, ",", false);
    }

    /// <summary>
    /// Creates a new instance of the Token class representing a reserved token with custom text.
    /// </summary>
    /// <param name="sender">The object that created the token.</param>
    /// <param name="parent">The parent token, if any.</param>
    /// <param name="text">The text of the token.</param>
    /// <returns>A Token object representing the reserved token.</returns>
    public static Token Reserved(object sender, Token? parent, string text)
    {
        return new Token(sender, parent, text, true);
    }

    /// <summary>
    /// Initializes a new instance of the Token class.
    /// </summary>
    /// <param name="sender">The object that created the token.</param>
    /// <param name="parent">The parent token, if any.</param>
    /// <param name="text">The text of the token.</param>
    /// <param name="isReserved">A boolean indicating whether the token is reserved.</param>
    public Token(object sender, Token? parent, string text, bool isReserved = false)
    {
        Sender = sender;
        Parent = parent;
        Text = text;
        IsReserved = isReserved;
    }

    /// <summary>
    /// Gets or sets the object that created the token.
    /// </summary>
    public object Sender { get; init; }

    /// <summary>
    /// Gets or sets the parent token, if any.
    /// </summary>
    public Token? Parent { get; init; }

    /// <summary>
    /// Gets or sets the text of the token.
    /// </summary>
    public string Text { get; init; }

    /// <summary>
    /// Gets or sets a boolean indicating whether the token is reserved.
    /// </summary>
    public bool IsReserved { get; init; }

    /// <summary>
    /// Determines whether the token needs a space separator from the previous token.
    /// </summary>
    /// <param name="prev">The previous token.</param>
    /// <returns>True if a space separator is needed; otherwise, false.</returns>
    public bool NeedsSpace(Token? prev = null)
    {
        if (prev == null) return false;

        if (prev!.Text.Equals("[")) return false;
        if (prev!.Text.Equals("(")) return false;
        if (prev!.Text.Equals(".")) return false;
        if (prev!.Text.Equals("::")) return false;
        if (prev!.Text.IsEqualNoCase("as")) return true;
        if (prev!.Text.IsEqualNoCase("array")) return false;

        if (Text.Equals("[")) return false;
        if (Text.Equals("]")) return false;
        if (Text.Equals(")")) return false;
        if (Text.Equals(",")) return false;
        if (Text.Equals(".")) return false;
        if (Text.Equals("("))
        {
            if (Sender is VirtualTable) return true;
            if (Sender is FunctionTable) return false;
            if (Sender is FunctionValue) return false;
            if (Sender is CastValue) return false;
            if (Sender is Filter) return false;
            if (Sender is WindowDefinition) return false;
            if (Sender is InsertClause) return false;
            return true;
        }
        if (Text.Equals("::")) return false;

        return true;
    }

    /// <summary>
    /// Retrieves all parent tokens, starting from the immediate parent and traversing up the hierarchy.
    /// </summary>
    /// <returns>An enumerable collection of parent tokens.</returns>
    public IEnumerable<Token> Parents()
    {
        // Retrieves all parent tokens, including the immediate parent and its ancestors.
        if (Parent == null) yield break;
        yield return Parent;
        foreach (var item in Parent.Parents()) yield return item;
    }
}
