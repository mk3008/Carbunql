using Carbunql.Clauses;

namespace Carbunql.Analysis.Parser;

public static class DeleteClauseParser
{

    public static DeleteClause Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

    public static DeleteClause Parse(ITokenReader r)
    {
        r.Read("delete");
        r.Read("from");
        return new DeleteClause(SelectableTableParser.Parse(r));
    }
}
