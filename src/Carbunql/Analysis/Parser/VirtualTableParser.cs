using Carbunql.Extensions;
using Carbunql.Tables;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses virtual tables from SQL text or token stream.
/// </summary>
public class VirtualTableParser
{
    /// <summary>
    /// Parses a virtual table from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed virtual table.</returns>
    public static VirtualTable Parse(ITokenReader r)
    {
        r.ReadOrDefault("(");

        var first = r.Peek();

        if (string.IsNullOrEmpty(first)) throw new NotSupportedException();

        // virtualTable
        if (first.IsEqualNoCase("select"))
        {
            var t = new VirtualTable(SelectQueryParser.Parse(r));
            r.ReadOrDefault(")");
            return t;
        }
        else if (first.IsEqualNoCase("values"))
        {
            var t = new VirtualTable(ValuesClauseParser.Parse(r));
            r.Read(")");
            return t;
        }
        else if (first == "(")
        {
            // empty bracket pattern
            var t = new VirtualTable(Parse(r));
            r.Read(")");
            return t;
        }

        throw new NotSupportedException($"Unsupported token:{first}");
    }
}
