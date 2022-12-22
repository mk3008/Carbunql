using Carbunql.Core.Clauses;
using Carbunql.Core.Extensions;
using Carbunql.Core.Tables;

namespace Carbunql.Analysis.Parser;

public static class TableParser
{
    public static TableBase Parse(string text)
    {
        using var r = new TokenReader(text);
        return Parse(r);
    }

    public static TableBase Parse(TokenReader r)
    {
        var item = r.ReadToken();

        if (item == "(")
        {
            //virtualTable
            var (first, inner) = r.ReadUntilCloseBracket();
            if (first.AreEqual("select"))
            {
                return new VirtualTable(SelectQueryParser.Parse(inner));
            }
            else if (first.AreEqual("values"))
            {
                return new VirtualTable(ValuesClauseParser.Parse(inner));
            }
            throw new NotSupportedException();
        }

        if (r.PeekRawToken().AreEqual("."))
        {
            //schema.table
            var schema = item;
            r.ReadToken(".");
            return new PhysicalTable(schema, r.ReadToken());
        }

        //table
        return new PhysicalTable(item);
    }
}