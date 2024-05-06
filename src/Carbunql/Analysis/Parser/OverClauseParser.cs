using Carbunql.Clauses;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses OVER clauses from SQL text or token streams.
/// </summary>
public static class OverClauseParser
{
    /// <summary>
    /// Parses an OVER clause from SQL text.
    /// </summary>
    /// <param name="text">The SQL text containing the OVER clause.</param>
    /// <returns>The parsed OVER clause.</returns>
    public static OverClause Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

    /// <summary>
    /// Parses an OVER clause from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed OVER clause.</returns>
    public static OverClause Parse(ITokenReader r)
    {
        var clause = new OverClause();
        clause.WindowDefinition = WindowDefinitionParser.Parse(r);
        return clause;
    }

    /// <summary>
    /// Parses an OVER clause from within parentheses in the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed OVER clause.</returns>
    public static OverClause ParseAsInner(ITokenReader r)
    {
        r.Read("(");
        using var ir = new BracketInnerTokenReader(r);
        var v = Parse(ir);
        return v;
    }
}
