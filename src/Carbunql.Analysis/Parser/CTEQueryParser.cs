namespace Carbunql.Analysis.Parser;

public class CTEQueryParser
{
    internal static CTEQuery Parse(ITokenReader r)
    {
        var w = WithClauseParser.Parse(r);
        var q = ReadQueryParser.Parse(r);
        return new CTEQuery(w) { Query = q };
    }
}