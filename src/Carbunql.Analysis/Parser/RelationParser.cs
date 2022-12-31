using Carbunql.Clauses;
using Carbunql.Extensions;

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
        if (join == TableJoin.Cross || join == TableJoin.Comma) return new Relation(table, join);

        r.ReadToken("on");
        var val = ValueParser.Parse(r);

        return new Relation(table, join, val);
    }

    private static TableJoin ParseTableJoin(TokenReader r)
    {
        var atrs = TableJoinEnumReader.GetCommandAttributes();
        var token = r.ReadToken(atrs.Select(x => x.Value.Text).ToArray());

        var q = atrs.Where(x => x.Value.FullText.AreEqual(token));

        if (q.Any()) return q.Select(x => Enum.Parse<TableJoin>(x.Key)).First();
        throw new NotSupportedException();
    }
}