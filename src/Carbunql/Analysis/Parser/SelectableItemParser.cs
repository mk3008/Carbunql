using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses selectable items from SQL text or token streams.
/// </summary>
public static class SelectableItemParser
{
    /// <summary>
    /// Parses a selectable item from SQL text.
    /// </summary>
    /// <param name="text">The SQL text containing the selectable item.</param>
    /// <returns>The parsed selectable item.</returns>
    public static SelectableItem Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

    /// <summary>
    /// Parses a selectable item from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed selectable item.</returns>
    public static SelectableItem Parse(ITokenReader r)
    {
        var v = ValueParser.Parse(r);
        r.ReadOrDefault("as");

        if (r.Peek().IsEqualNoCase(ReservedText.All()))
        {
            return new SelectableItem(v, v.GetDefaultName());
        }

        return new SelectableItem(v, r.Read());
    }
}
