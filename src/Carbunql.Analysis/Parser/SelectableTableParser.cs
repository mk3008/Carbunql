using Carbunql.Core.Clauses;
using Carbunql.Core.Extensions;

namespace Carbunql.Analysis.Parser;

public static class SelectableTableParser
{
    public static SelectableTable Parse(string text)
    {
        using var r = new TokenReader(text);
        return Parse(r);
    }

    public static SelectableTable Parse(TokenReader r)
    {
        var breaktokens = TokenReader.BreakTokens;

        var v = TableParser.Parse(r);
        r.TryReadToken("as");

        if (r.PeekRawToken().AreContains(breaktokens))
        {
            return new SelectableTable(v, v.GetDefaultName());
        }

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