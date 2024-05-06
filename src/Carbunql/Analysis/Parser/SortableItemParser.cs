using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses sortable items used in ORDER BY clauses from SQL text or token streams.
/// </summary>
public static class SortableItemParser
{
    /// <summary>
    /// Parses a sortable item from SQL text.
    /// </summary>
    /// <param name="text">The SQL text containing the sortable item.</param>
    /// <returns>The parsed sortable item.</returns>
    public static SortableItem Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

    /// <summary>
    /// Parses a sortable item from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed sortable item.</returns>
    public static SortableItem Parse(ITokenReader r)
    {
        var v = ValueParser.Parse(r);
        var isasc = true;

        if (r.Peek().IsEqualNoCase(ReservedText.All()))
        {
            return new SortableItem(v);
        }

        if (r.Peek().IsEqualNoCase("asc"))
        {
            r.Read("asc");
            isasc = true;
        }
        else if (r.Peek().IsEqualNoCase("desc"))
        {
            r.Read("desc");
            isasc = false;
        }

        var nullSort = NullSort.Undefined;
        if (r.Peek().IsEqualNoCase("nulls first"))
        {
            r.Read("nulls first");
            nullSort = NullSort.First;
        }
        else if (r.Peek().IsEqualNoCase("nulls last"))
        {
            r.Read("nulls last");
            nullSort = NullSort.Last;
        }

        return new SortableItem(v, isasc, nullSort);
    }
}
