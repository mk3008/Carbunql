using Carbunql.Extensions;
using Carbunql.Tables;

namespace Carbunql.Analysis.Parser;

public class VirtualTableParser
{
    public static VirtualTable Parse(ITokenReader r)
    {
        r.Read("(");

        var first = r.Peek();

        if (string.IsNullOrEmpty(first)) throw new NotSupportedException();

        //virtualTable
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
            //empty bracket pattern
            var t = new VirtualTable(Parse(r));
            r.Read(")");
            return t;
        }

        throw new NotSupportedException($"token:{first}");
    }
}
