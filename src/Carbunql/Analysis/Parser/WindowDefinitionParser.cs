using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis.Parser;

public static class WindowDefinitionParser
{
    public static WindowDefinition Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

    public static WindowDefinition Parse(ITokenReader r)
    {
        if (r.Peek() != "(")
        {
            return new WindowDefinition(r.Read());
        }

        r.Read("(");
        var definition = new WindowDefinition();
        if (r.Peek().IsEqualNoCase("partition by"))
        {
            r.Read(("partition by"));
            definition.PartitionBy = PartitionClauseParser.Parse(r);
        }

        if (r.Peek().IsEqualNoCase("order by"))
        {
            r.Read(("order by"));
            definition.OrderBy = OrderClauseParser.Parse(r);
        }

        r.Read(")");

        return definition;
    }
}