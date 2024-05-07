using Carbunql.Analysis.Parser;
using Carbunql.Extensions;

namespace Carbunql.Analysis;

/// <summary>
/// Provides methods for parsing ALTER TABLE queries from SQL token streams.
/// </summary>
public static class AlterTableQueryParser
{
    /// <summary>
    /// Parses an ALTER TABLE query from SQL text.
    /// </summary>
    /// <param name="text">The SQL text to parse.</param>
    /// <returns>The parsed ALTER TABLE query.</returns>
    public static AlterTableQuery Parse(string text)
    {
        var r = new SqlTokenReader(text);
        var query = Parse(r);

        // If there are unparsed tokens remaining, it indicates an issue with parsing.
        if (!r.Peek().IsEndToken())
        {
            throw new NotSupportedException($"Parsing terminated despite the presence of unparsed tokens. (Token: '{r.Peek()}')");
        }

        return query;
    }

    /// <summary>
    /// Parses an ALTER TABLE query from a token reader.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed ALTER TABLE query.</returns>
    public static AlterTableQuery Parse(ITokenReader r)
    {
        return new AlterTableQuery(AlterTableClauseParser.Parse(r));
    }
}
