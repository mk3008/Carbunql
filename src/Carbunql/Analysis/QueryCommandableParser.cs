using Carbunql.Analysis.Parser;
using Carbunql.Building;
using Carbunql.Extensions;

namespace Carbunql.Analysis;

/// <summary>
/// Provides functionality to parse various SQL query commands.
/// </summary>
public static class QueryCommandableParser
{
    /// <summary>
    /// Parses the specified SQL query string.
    /// </summary>
    /// <param name="text">The SQL query string.</param>
    /// <returns>The parsed QueryCommandable object.</returns>
    public static IQueryCommandable Parse(string text)
    {
        var r = new SqlTokenReader(text);
        var q = Parse(r);

        if (!r.Peek().IsEndToken())
        {
            throw new NotSupportedException($"Parsing terminated despite the presence of unparsed tokens. (Token: '{r.Peek()}')");
        }

        return q;
    }

    /// <summary>
    /// Parses the SQL query using the provided ITokenReader.
    /// </summary>
    /// <param name="r">The ITokenReader instance.</param>
    /// <returns>The parsed QueryCommandable object.</returns>
    public static IQueryCommandable Parse(ITokenReader r)
    {
        var token = r.Peek();

        if (token.IsEqualNoCase("with"))
        {
            var clause = WithClauseParser.Parse(r);

            token = r.Peek();
            if (token.IsEqualNoCase("select"))
            {
                var sq = SelectQueryParser.Parse(r);
                sq.WithClause = clause;
                return sq;
            }
            else if (token.IsEqualNoCase("insert into"))
            {
                // NOTE
                // Although this is a preliminary specification,
                // insert queries themselves do not allow CTEs.
                // So if her CTE is mentioned in the insert query,
                // it will be forced to be treated as her CTE in the select query.
                var iq = InsertQueryParser.Parse(r);
                if (iq.Query is SelectQuery sq)
                {
                    foreach (var item in clause)
                    {
                        sq.With(item);
                    }
                }
                return iq;
            }
            else if (token.IsEqualNoCase("update"))
            {
                var uq = UpdateQueryParser.Parse(r);
                uq.WithClause = clause;
                return uq;
            }
            else if (token.IsEqualNoCase("delete"))
            {
                var dq = DeleteQueryParser.Parse(r);
                dq.WithClause = clause;
                return dq;
            }

            throw new NotSupportedException($"Unsupported query command: '{token}'");
        }
        else
        {
            if (token.IsEqualNoCase("select")) return SelectQueryParser.Parse(r);
            if (token.IsEqualNoCase("values")) return ValuesQueryParser.Parse(r);

            if (token.IsEqualNoCase("insert into")) return InsertQueryParser.Parse(r);
            if (token.IsEqualNoCase("update")) return UpdateQueryParser.Parse(r);
            if (token.IsEqualNoCase("delete")) return DeleteQueryParser.Parse(r);

            if (token.IsEqualNoCase("create table")) return CreateTableQueryParser.Parse(r);
            if (token.IsEqualNoCase("create index")) return CreateIndexQueryParser.Parse(r);

            if (token.IsEqualNoCase("alter table")) return AlterTableQueryParser.Parse(r);

            throw new NotSupportedException($"Unsupported query command: '{token}'");
        }
    }
}
