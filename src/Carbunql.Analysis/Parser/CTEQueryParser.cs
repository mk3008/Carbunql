namespace Carbunql.Analysis.Parser;

public class CTEQueryParser
{
    internal static CTEQuery Parse(TokenReader r)
    {
        var w = WithClauseParser.Parse(r);
        var q = QueryParser.Parse(r);
        return new CTEQuery(w) { Query = q };
    }
}