using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses WINDOW clauses from SQL text or token stream.
/// </summary>
public static class WindowClauseParser
{
    /// <summary>
    /// Parses a WINDOW clause from SQL text.
    /// </summary>
    /// <param name="text">The SQL text to parse.</param>
    /// <returns>The parsed WINDOW clause.</returns>
    public static WindowClause Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

    /// <summary>
    /// Parses a WINDOW clause from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed WINDOW clause.</returns>
    public static WindowClause Parse(ITokenReader r)
    {
        r.ReadOrDefault("window");

        return new WindowClause(ParseNamedWindowDefinitions(r).ToList());
    }

    /// <summary>
    /// Parses named window definitions from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>An enumerable collection of parsed named window definitions.</returns>
    private static IEnumerable<NamedWindowDefinition> ParseNamedWindowDefinitions(ITokenReader r)
    {
        do
        {
            if (r.Peek().IsEqualNoCase(",")) r.Read();
            yield return NamedWindowDefinitionParser.Parse(r);
        }
        while (r.Peek().IsEqualNoCase(","));
    }
}
