using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses various types of tables from SQL text or token streams.
/// </summary>
public static class TableParser
{
    /// <summary>
    /// Parses a table from SQL text.
    /// </summary>
    /// <param name="text">The SQL text containing the table.</param>
    /// <returns>The parsed table.</returns>
    public static TableBase Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

    /// <summary>
    /// Parses a table from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed table.</returns>
    public static TableBase Parse(ITokenReader r)
    {
        if (r.Peek().IsEqualNoCase("lateral"))
        {
            return LateralTableParser.Parse(r);
        }

        if (r.Peek().IsEqualNoCase("("))
        {
            return VirtualTableParser.Parse(r);
        }

        var item = r.Read();

        if (r.Peek().IsEqualNoCase("."))
        {
            var value = item;
            while (r.Peek() == ".")
            {
                r.Read(".");
                value += "." + r.Read();
            };

            var parts = value.Split(".");
            var table = parts[parts.Length - 1];
            var schema = value.Substring(0, value.Length - table.Length - 1);

            return new PhysicalTable(schema, table);
        }

        if (r.Peek().IsEqualNoCase("("))
        {
            return FunctionTableParser.Parse(r, item);
        }

        // Assume it's a physical table
        return new PhysicalTable(item);
    }
}
