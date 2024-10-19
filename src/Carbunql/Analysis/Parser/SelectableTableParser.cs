using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Values;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses selectable tables from SQL text or token streams.
/// </summary>
public static class SelectableTableParser
{
    /// <summary>
    /// Parses a selectable table from SQL text.
    /// </summary>
    /// <param name="text">The SQL text containing the selectable table.</param>
    /// <returns>The parsed selectable table.</returns>
    public static SelectableTable Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

    /// <summary>
    /// Parses a selectable table from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed selectable table.</returns>
    public static SelectableTable Parse(ITokenReader r)
    {
        var v = TableParser.Parse(r);
        var t = r.Peek();

        if (string.IsNullOrEmpty(t) || t.IsEqualNoCase(ReservedText.All(ReservedTokenFilter)))
        {
            if (TryParseColumnAliases(r, out var columnAliases))
            {
                return new SelectableTable(v, v.GetDefaultName(), columnAliases);
            }
            else
            {
                return new SelectableTable(v, v.GetDefaultName());
            }
        }
        else
        {
            r.ReadOrDefault("as");

            var next = r.Peek();
            if (next.IsEqualNoCase(["from", "set", "using"]))
            {
                return new SelectableTable(v, v.GetDefaultName());
            }

            var alias = r.Read();

            if (TryParseColumnAliases(r, out var columnAliases))
            {
                return new SelectableTable(v, alias, columnAliases);
            }
            else
            {
                return new SelectableTable(v, alias);
            }
        }
    }

    /// <summary>
    /// Filters out reserved tokens to determine if a token is part of a column alias or not.
    /// </summary>
    /// <param name="text">The token text.</param>
    /// <returns><c>true</c> if the token is not a reserved token; otherwise, <c>false</c>.</returns>
    private static bool ReservedTokenFilter(string text)
    {
        if (ReservedText.As == text) return false;
        return true;
    }

    /// <summary>
    /// Tries to parse column aliases from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <param name="columnAliases">The parsed column aliases.</param>
    /// <returns><c>true</c> if column aliases were successfully parsed; otherwise, <c>false</c>.</returns>
    private static bool TryParseColumnAliases(ITokenReader r, [MaybeNullWhen(false)] out ValueCollection columnAliases)
    {
        columnAliases = default;
        if (r.Peek() != "(") return false;

        r.Read("(");
        columnAliases = ValueCollectionParser.Parse(r);
        r.Read(")");
        return true;
    }
}
