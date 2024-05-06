using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses a common table expression (CTE) from SQL text or token streams.
/// </summary>
public static class CommonTableParser
{
    /// <summary>
    /// Parses a common table expression (CTE) from SQL text.
    /// </summary>
    /// <param name="text">The SQL text containing the CTE.</param>
    /// <returns>The parsed CTE.</returns>
    public static CommonTable Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

    /// <summary>
    /// Parses a common table expression (CTE) from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed CTE.</returns>
    public static CommonTable Parse(ITokenReader r)
    {
        var alias = r.Read();
        ValueCollection? colAliases = null;
        if (r.Peek().IsEqualNoCase("("))
        {
            colAliases = ValueCollectionParser.ParseAsInner(r);
        }

        r.Read("as");

        var material = Materialized.Undefined;
        if (r.Peek().IsEqualNoCase("materialized"))
        {
            r.Read("materialized");
            material = Materialized.Materialized;
        }
        else if (r.Peek().IsEqualNoCase("not materialized"))
        {
            r.Read("not materialized");
            material = Materialized.NotMaterialized;
        }

        var t = VirtualTableParser.Parse(r);
        return colAliases != null
            ? new CommonTable(t, alias, colAliases) { Materialized = material }
            : new CommonTable(t, alias) { Materialized = material };
    }
}
