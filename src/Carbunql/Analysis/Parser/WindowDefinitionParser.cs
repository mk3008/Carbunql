using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses WINDOW definitions from SQL text or token stream.
/// </summary>
public static class WindowDefinitionParser
{
    /// <summary>
    /// Parses a WINDOW definition from SQL text.
    /// </summary>
    /// <param name="text">The SQL text to parse.</param>
    /// <returns>The parsed WINDOW definition.</returns>
    public static WindowDefinition Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

    /// <summary>
    /// Parses a WINDOW definition from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed WINDOW definition.</returns>
    public static WindowDefinition Parse(ITokenReader r)
    {
        if (r.Peek() != "(")
        {
            return new WindowDefinition(r.Read());
        }

        r.Read("(");
        var definition = new WindowDefinition();
        if (r.Peek().IsEqualNoCase("partition by"))
        {
            r.Read("partition by");
            definition.PartitionBy = PartitionClauseParser.Parse(r);
        }

        if (r.Peek().IsEqualNoCase("order by"))
        {
            r.Read("order by");
            definition.OrderBy = OrderClauseParser.Parse(r);
        }

        r.Read(")");

        return definition;
    }
}
