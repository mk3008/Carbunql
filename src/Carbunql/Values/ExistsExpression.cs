namespace Carbunql.Values;

/// <summary>
/// Represents an EXISTS expression.
/// </summary>
public class ExistsExpression : QueryContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExistsExpression"/> class with the specified query.
    /// </summary>
    /// <param name="query">The query.</param>
    public ExistsExpression(IQueryCommandable query) : base(query)
    {
    }

    /// <inheritdoc/>
    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        yield return Token.Reserved(this, parent, "exists");

        var bracket = Token.ReservedBracketStart(this, parent);
        yield return bracket;
        foreach (var item in Query.GetTokens(bracket)) yield return item;
        yield return Token.ReservedBracketEnd(this, parent);
    }
}

