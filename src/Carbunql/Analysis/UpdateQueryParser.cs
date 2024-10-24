﻿using Carbunql.Analysis.Parser;
using Carbunql.Extensions;

namespace Carbunql.Analysis;

public static class UpdateQueryParser
{
    public static UpdateQuery Parse(string text)
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

    public static UpdateQuery Parse(ITokenReader r)
    {
        var uq = new UpdateQuery();

        // with
        if (r.Peek().IsEqualNoCase("with"))
        {
            uq.WithClause = WithClauseParser.Parse(r);
        }

        // update
        uq.UpdateClause = UpdateClauseParser.Parse(r);

        // set
        uq.SetClause = SetClauseParser.Parse(r);

        if (r.Peek().IsEqualNoCase("from"))
        {
            uq.FromClause = FromClauseParser.Parse(r);
        }

        if (r.Peek().IsEqualNoCase("where"))
        {
            uq.WhereClause = WhereClauseParser.Parse(r);
        }

        if (r.Peek().IsEqualNoCase("returning"))
        {
            uq.ReturningClause = ReturningClauseParser.Parse(r);
        }

        return uq;
    }
}
