using Carbunql.Clauses;

namespace Carbunql.Analysis.Parser;

public static class NamedWindowDefinitionParser
{
    public static NamedWindowDefinition Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

    public static NamedWindowDefinition Parse(ITokenReader r)
    {
        var alias = r.Read();
        r.Read("as");

        var w = WindowDefinitionParser.Parse(r);

        return new NamedWindowDefinition(alias, w);
    }
}