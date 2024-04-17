using Carbunql.Analysis.Parser;
using Carbunql.Extensions;

namespace Carbunql.Analysis;

public static class AlterTableQueryParser
{
    public static AlterTableQuery Parse(string text)
    {
        var r = new SqlTokenReader(text);
        var q = Parse(r);

        if (!r.Peek().IsEndToken())
        {
            throw new NotSupportedException($"Parsing terminated despite the presence of unparsed tokens.(token:'{r.Peek()}')");
        }

        return q;
    }

    public static AlterTableQuery Parse(ITokenReader r)
    {
        return new AlterTableQuery(AlterTableClauseParser.Parse(r));
    }
}