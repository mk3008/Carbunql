namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses an array from SQL text or token streams.
/// </summary>
public class ArrayParser
{
    /// <summary>
    /// Parses an array from SQL text.
    /// </summary>
    /// <param name="text">The SQL text containing the array.</param>
    /// <returns>The parsed array.</returns>
    public static List<string> Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

    /// <summary>
    /// Parses an array from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed array.</returns>
    public static List<string> Parse(ITokenReader r)
    {
        var lst = new List<string>();

        r.Read("(");
        do
        {
            r.ReadOrDefault(",");
            lst.Add(r.Read());
        } while (r.Peek() != ")");

        r.Read(")");

        return lst;
    }
}
