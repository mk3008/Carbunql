using Carbunql.Tables;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses physical table references from token streams.
/// </summary>
public class PhysicalTableParser
{
    /// <summary>
    /// Parses a physical table reference from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed physical table reference.</returns>
    public static PhysicalTable Parse(ITokenReader r)
    {
        var value = r.Read();

        if (r.Peek() != ".") return new PhysicalTable(value);

        while (r.Peek() == ".")
        {
            r.Read(".");
            value += "." + r.Read();
        }

        var parts = value.Split(".");
        var table = parts[parts.Length - 1];
        var schema = value.Substring(0, value.Length - table.Length - 1);

        return new PhysicalTable(schema, table);
    }
}
