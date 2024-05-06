using Carbunql.Clauses;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses named window definitions from SQL text or token streams.
/// </summary>
public static class NamedWindowDefinitionParser
{
    /// <summary>
    /// Parses a named window definition from SQL text.
    /// </summary>
    /// <param name="text">The SQL text containing the named window definition.</param>
    /// <returns>The parsed named window definition.</returns>
    public static NamedWindowDefinition Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

    /// <summary>
    /// Parses a named window definition from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed named window definition.</returns>
    public static NamedWindowDefinition Parse(ITokenReader r)
    {
        var alias = r.Read();
        r.Read("as");

        var w = WindowDefinitionParser.Parse(r);

        return new NamedWindowDefinition(alias, w);
    }
}
