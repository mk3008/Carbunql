using Carbunql.Tables;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses a function table from SQL text or token streams.
/// </summary>
public static class FunctionTableParser
{
    /// <summary>
    /// Parses a function table from SQL text.
    /// </summary>
    /// <param name="text">The SQL text containing the function table.</param>
    /// <param name="functionName">The name of the function.</param>
    /// <returns>The parsed function table.</returns>
    public static FunctionTable Parse(string text, string functionName)
    {
        var r = new SqlTokenReader(text);
        return Parse(r, functionName);
    }

    /// <summary>
    /// Parses a function table from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <param name="functionName">The name of the function.</param>
    /// <returns>The parsed function table.</returns>
    public static FunctionTable Parse(ITokenReader r, string functionName)
    {
        var arg = ValueCollectionParser.ParseAsInner(r);

        return new FunctionTable(functionName, arg);
    }
}
