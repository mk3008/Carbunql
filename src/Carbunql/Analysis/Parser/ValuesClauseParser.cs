using Carbunql.Extensions;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses VALUES clause from SQL text or token stream.
/// </summary>
internal static class ValuesClauseParser
{
    /// <summary>
    /// Parses VALUES clause from SQL text.
    /// </summary>
    /// <param name="text">The SQL text containing the VALUES clause.</param>
    /// <returns>The parsed VALUES query.</returns>
    public static ValuesQuery Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

    /// <summary>
    /// Parses VALUES clause from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed VALUES query.</returns>
    public static ValuesQuery Parse(ITokenReader r)
    {
        var fn = () =>
        {
            if (!r.Peek().IsEqualNoCase(",")) return false;
            r.Read(",");
            return true;
        };

        r.ReadOrDefault("values");

        var lst = new List<ValueCollection>();
        do
        {
            lst.Add(ValueCollectionParser.ParseAsInner(r));
        } while (fn());

        return new ValuesQuery(lst);
    }
}
