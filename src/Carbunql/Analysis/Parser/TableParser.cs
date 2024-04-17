using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;

namespace Carbunql.Analysis.Parser;

public static class TableParser
{
    public static TableBase Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

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

        //table
        return new PhysicalTable(item);
    }
}