using Carbunql.Clauses;

namespace Carbunql.Analysis.Parser;

public static class DeleteUsingClauseParser
{

    public static DeleteUsingClause Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

    public static DeleteUsingClause Parse(ITokenReader r)
    {
        r.Read("using");

        var u = new DeleteUsingClause()
        {
            SelectableTableParser.Parse(r)
        };

        while (r.Peek() == ",")
        {
            r.Read(",");
            u.Add(SelectableTableParser.Parse(r));
        }

        return u;
    }
}
