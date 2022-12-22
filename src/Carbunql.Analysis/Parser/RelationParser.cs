using Carbunql.Core;
using Carbunql.Core.Clauses;
using Carbunql.Core.Extensions;

namespace Carbunql.Analysis.Parser;

public static class RelationParser
{
    public static Relation Parse(string text)
    {
        using var r = new TokenReader(text);
        return Parse(r);
    }

    public static Relation Parse(TokenReader r)
    {
        var join = ParseTableJoin(r);
        var table = SelectableTableParser.Parse(r);
        if (join == TableJoin.Cross) return new Relation(table, join);

        r.ReadToken("on");
        var val = ValueParser.Parse(r);

        return new Relation(table, join, val);
    }

    private static TableJoin ParseTableJoin(TokenReader r)
    {
        var tp = r.ReadToken(new string[] { "inner", "left", "right", "cross" });

        if (tp.AreEqual("inner join"))
        {
            return TableJoin.Inner;
        }
        else if (tp.AreEqual("left join") || tp.AreEqual("left outer join"))
        {
            return TableJoin.Left;
        }
        else if (tp.AreEqual("right join") || tp.AreEqual("right outer join"))
        {
            return TableJoin.Right;
        }
        else if (tp.AreEqual("cross join"))
        {
            return TableJoin.Cross;
        }
        throw new NotSupportedException();
    }
}