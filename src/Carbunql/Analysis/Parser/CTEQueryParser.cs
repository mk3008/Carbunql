namespace Carbunql.Analysis.Parser;

public class CTEQueryParser
{
    internal static SelectQuery Parse(ITokenReader r)
    {
        var w = WithClauseParser.Parse(r);
        var sq = SelectQueryParser.Parse(r);
        sq.WithClause = w;
        return sq;
    }
}