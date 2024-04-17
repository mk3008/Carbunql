using Carbunql.Analysis.Parser;
using Carbunql.Building;
using Carbunql.Extensions;

namespace Carbunql.Analysis;

public static class InsertQueryParser
{
    public static InsertQuery Parse(string text)
    {
        var r = new SqlTokenReader(text);

        var iq = Parse(r);

        if (!r.Peek().IsEndToken())
        {
            throw new NotSupportedException($"Parsing terminated despite the presence of unparsed tokens.(token:'{r.Peek()}')");
        }

        return iq;
    }

    internal static InsertQuery Parse(ITokenReader r)
    {
        if (r.Peek().IsEqualNoCase("with"))
        {
            // NOTE
            // Although this is a preliminary specification,
            // insert queries themselves do not allow CTEs.
            // So if her CTE is mentioned in the insert query,
            // it will be forced to be treated as her CTE in the select query.
            var withClause = WithClauseParser.Parse(r);
            var iq = ParseMain(r);
            if (iq.Query is SelectQuery sq)
            {
                foreach (var item in withClause)
                {
                    sq.With(item);
                }
            }
            return iq;
        }
        else
        {
            return ParseMain(r);
        }
        throw new NotSupportedException();
    }

    internal static InsertQuery ParseMain(ITokenReader r)
    {
        var iq = new InsertQuery();

        iq.InsertClause = InsertClauseParser.Parse(r);
        iq.Query = QueryParser.Parse(r);

        if (r.Peek().IsEqualNoCase("returning"))
        {
            iq.ReturningClause = ReturningClauseParser.Parse(r);
        }

        return iq;
    }
}