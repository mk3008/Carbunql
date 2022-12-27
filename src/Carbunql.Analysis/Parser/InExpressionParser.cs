using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

public static class InExpressionParser
{
    public static InExpression Parse(ValueBase value, string argument)
    {
        using var r = new TokenReader(argument);
        return Parse(value, r);
    }

    public static InExpression Parse(ValueBase value, TokenReader r)
    {
        r.ReadToken("(");
        var (first, inner) = r.ReadUntilCloseBracket();
        if (first.AreEqual("select"))
        {
            //sub query
            return new InExpression(value, ValueParser.Parse("(" + inner + ")"));
        }
        else
        {
            //value collection
            var bv = new BracketValue(ValueCollectionParser.Parse(inner));
            return new InExpression(value, bv);
        }
    }
}