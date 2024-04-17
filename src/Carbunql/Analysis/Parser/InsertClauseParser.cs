using Carbunql.Clauses;

namespace Carbunql.Analysis.Parser;

public static class InsertClauseParser
{
    public static InsertClause Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

    public static InsertClause Parse(ITokenReader r)
    {
        r.Read("insert into");

        var t = PhysicalTableParser.Parse(r);

        if (r.Peek() != "(") return new InsertClause(t);

        r.Read("(");
        var aliases = ValueCollectionParser.Parse(r);
        r.Read(")");

        return new InsertClause(t)
        {
            ColumnAliases = aliases,
        };
    }
}
