using Carbunql.Clauses;

namespace Carbunql.Analysis.Parser;

public static class UpdateClauseParser
{

    public static UpdateClause Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

    public static UpdateClause Parse(ITokenReader r)
    {
        r.Read("update");
        return new UpdateClause(SelectableTableParser.Parse(r));
    }
}
