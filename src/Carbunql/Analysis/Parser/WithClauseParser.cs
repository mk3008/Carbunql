using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses WITH clauses from SQL text or token stream.
/// </summary>
public static class WithClauseParser
{
    /// <summary>
    /// Parses a WITH clause from SQL text.
    /// </summary>
    /// <param name="text">The SQL text to parse.</param>
    /// <returns>The parsed WITH clause.</returns>
    public static WithClause Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

    /// <summary>
    /// Parses a WITH clause from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed WITH clause.</returns>
    public static WithClause Parse(ITokenReader r)
    {
        r.Read("with");

        var recursive = r.ReadOrDefault("recursive") != null;
        return new WithClause(ParseCommonTables(r).ToList()) { HasRecursiveKeyword = recursive };
    }

    private static IEnumerable<CommonTable> ParseCommonTables(ITokenReader r)
    {
        do
        {
            if (r.Peek().IsEqualNoCase(",")) r.Read();
            yield return CommonTableParser.Parse(r);
        }
        while (r.Peek().IsEqualNoCase(","));
    }
}
