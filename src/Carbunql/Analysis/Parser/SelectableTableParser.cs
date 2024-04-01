using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Values;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Analysis.Parser;

public static class SelectableTableParser
{
    private static string[] SelectTableBreakTokens = new[] { "on" };

    public static SelectableTable Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

    private static bool ReservedTokenFilter(string text)
    {
        if (ReservedText.As == text) return false;
        return true;
    }

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