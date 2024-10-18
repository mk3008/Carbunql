using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses a FROM clause from SQL text or token streams.
/// </summary>
public static class FromClauseParser
{
    /// <summary>
    /// Parses a FROM clause from SQL text.
    /// </summary>
    /// <param name="text">The SQL text containing the FROM clause.</param>
    /// <returns>The parsed FROM clause.</returns>
    public static FromClause Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

    /// <summary>
    /// Parses a FROM clause from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed FROM clause.</returns>
    public static FromClause Parse(ITokenReader r)
    {
        r.ReadOrDefault("from");

        var relationtokens = ReservedText.GetRelationTexts();

        var root = SelectableTableParser.Parse(r);
        var from = new FromClause(root);

        if (!r.Peek().IsEqualNoCase(relationtokens))
        {
            return from;
        }
        from.Relations ??= new List<Relation>();

        do
        {
            from.Relations.Add(RelationParser.Parse(r));

        } while (r.Peek().IsEqualNoCase(relationtokens));

        return from;
    }
}
