using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis.Parser;

public static class SelectableTableParser
{
    private static string[] SelectTableBreakTokens = new[] { "on" };

    public static SelectableTable Parse(string text)
    {
        using var r = new TokenReader(text);
        return Parse(r);
    }

    public static SelectableTable Parse(TokenReader r)
    {
        var relationtokens = TableJoinEnumReader.GetCommandAttributes().Select(x => x.Value.Text);
        var breaktokens = TokenReader.BreakTokens.Union(relationtokens).Union(SelectTableBreakTokens);

        var v = TableParser.Parse(r);

        if (r.PeekRawToken().AreContains(breaktokens))
        {
            return new SelectableTable(v, v.GetDefaultName());
        }

        r.TryReadToken("as");
        var alias = r.ReadToken();

        if (!r.PeekRawToken().AreEqual("("))
        {
            return new SelectableTable(v, alias);
        }

        r.ReadToken("(");
        var (_, inner) = r.ReadUntilCloseBracket();
        var colAliases = ValueCollectionParser.Parse(inner);

        return new SelectableTable(v, alias, colAliases);
    }
}