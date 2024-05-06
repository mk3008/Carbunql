using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses relations from SQL text or token streams.
/// </summary>
public static class RelationParser
{
    /// <summary>
    /// Parses a relation from SQL text.
    /// </summary>
    /// <param name="text">The SQL text containing the relation.</param>
    /// <returns>The parsed relation.</returns>
    public static Relation Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

    /// <summary>
    /// Parses a relation from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed relation.</returns>
    public static Relation Parse(ITokenReader r)
    {
        if (r.Peek().IsEqualNoCase((x) =>
        {
            if (x.IsEqualNoCase(ReservedText.Cross)) return true;
            if (x.IsEqualNoCase(ReservedText.Comma)) return true;
            return false;
        }))
        {
            var join = r.Read();
            var table = SelectableTableParser.Parse(r);
            return new Relation(table, join);
        }
        else if (r.Peek().IsEqualNoCase((x) =>
        {
            if (x.IsEqualNoCase(ReservedText.Join)) return true;
            if (x.IsEqualNoCase(ReservedText.Inner)) return true;
            if (x.IsEqualNoCase(ReservedText.Left)) return true;
            if (x.IsEqualNoCase(ReservedText.Right)) return true;
            return false;
        }))
        {
            var join = r.Read();
            var table = SelectableTableParser.Parse(r);
            r.Read("on");
            var val = ValueParser.Parse(r);
            return new Relation(table, join, val);
        }

        throw new NotSupportedException($"Unsupported token:{r.Peek()}");
    }
}
