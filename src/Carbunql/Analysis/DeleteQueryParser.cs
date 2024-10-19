using Carbunql.Analysis.Parser;
using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis;


public static class DeleteQueryParser
{

    public static DeleteQuery Parse(string text)
    {
        var r = new SqlTokenReader(text);
        var query = Parse(r);

        // If there are unparsed tokens remaining, it indicates an issue with parsing.
        if (!r.Peek().IsEndToken())
        {
            throw new NotSupportedException($"Parsing terminated despite the presence of unparsed tokens. (Token: '{r.Peek()}')");
        }

        return query;
    }

    public static DeleteQuery Parse(ITokenReader r)
    {
        var dq = new DeleteQuery();

        // with
        if (r.Peek().IsEqualNoCase("with"))
        {
            dq.WithClause = WithClauseParser.Parse(r);
        }

        // update
        dq.DeleteClause = DeleteClauseParser.Parse(r);

        if (r.Peek().IsEqualNoCase("where"))
        {
            dq.WhereClause = WhereClauseParser.Parse(r);
        }

        if (r.Peek().IsEqualNoCase("returning"))
        {
            dq.ReturningClause = ReturningClauseParser.Parse(r);
        }

        return dq;
    }
}
