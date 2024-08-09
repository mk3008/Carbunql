using Carbunql.Extensions;
using Carbunql.Tables;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses virtual tables from SQL text or token stream.
/// </summary>
public class VirtualTableParser
{
    private static string[] QueryOperators = ["union", "union all", "except", "minus", "intersect"];

    /// <summary>
    /// Parses a virtual table from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed virtual table.</returns>
    public static VirtualTable Parse(ITokenReader r)
    {
        r.Read("(");

        var first = r.Peek();

        if (string.IsNullOrEmpty(first)) throw new NotSupportedException();

        // virtualTable
        if (first.IsEqualNoCase("select"))
        {
            var sq = SelectQueryParser.Parse(r);
            r.Read(")");

            // check operatorable query
            if (r.Peek().IsEqualNoCase(QueryOperators))
            {
                // read operator
                var op = r.Read();

                if (r.Peek() == "(")
                {
                    var vt = Parse(r);
                    var sq2 = vt.GetSelectQuery();
                    sq.AddOperatableValue(op, sq2);
                }
                else
                {
                    var sq2 = ReadQueryParser.Parse(r);
                    sq.AddOperatableValue(op, sq2);
                }
            }

            return new VirtualTable(sq);
        }
        else if (first.IsEqualNoCase("values"))
        {
            var t = new VirtualTable(ValuesClauseParser.Parse(r));
            r.Read(")");
            return t;
        }
        else if (first == ")")
        {
            // empty bracket pattern
            r.Read(")");
        }
        else if (first == "(")
        {
            var t = Parse(r);
            r.Read(")");

            // check operatorable query
            if (r.Peek().IsEqualNoCase(QueryOperators))
            {
                // convert to SelectQuery
                var sq = t.GetSelectQuery();

                // read operator
                var op = r.Read();

                if (r.Peek() == "(")
                {
                    var vt = Parse(r);
                    var sq2 = vt.GetSelectQuery();

                    sq.AddOperatableValue(op, sq2);
                    return new VirtualTable(sq);
                }
                else
                {
                    var sq2 = ReadQueryParser.Parse(r);
                    sq.AddOperatableValue(op, sq2);
                    return new VirtualTable(sq);
                }
            }

            return t;
        }

        throw new NotSupportedException($"Unsupported token:{first}");
    }
}
