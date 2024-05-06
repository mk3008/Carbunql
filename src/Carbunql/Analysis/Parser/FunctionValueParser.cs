using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses a function value from SQL text or token streams.
/// </summary>
public static class FunctionValueParser
{
    /// <summary>
    /// Parses a function value from SQL text.
    /// </summary>
    /// <param name="text">The SQL text containing the function value.</param>
    /// <param name="functionName">The name of the function.</param>
    /// <returns>The parsed function value.</returns>
    public static FunctionValue Parse(string text, string functionName)
    {
        var r = new SqlTokenReader(text);
        return Parse(r, functionName);
    }

    /// <summary>
    /// Parses a function value from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <param name="functionName">The name of the function.</param>
    /// <returns>The parsed function value.</returns>
    public static FunctionValue Parse(ITokenReader r, string functionName)
    {
        var arg = ValueCollectionParser.ParseAsInner(r);

        Filter? filter = null;
        OverClause? over = null;

        if (r.Peek().IsEqualNoCase("filter"))
        {
            r.Read("filter");
            filter = FilterParser.Parse(r);
        }

        if (r.Peek().IsEqualNoCase("over"))
        {
            r.Read("over");
            over = OverClauseParser.Parse(r);
        }

        var fnc = new FunctionValue(functionName, arg);
        fnc.Filter = filter;
        fnc.Over = over;

        return fnc;
    }
}
