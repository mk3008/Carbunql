using Carbunql.Core.Clauses;
using Carbunql.Core.Extensions;
using Carbunql.Core.Values;

namespace Carbunql.Analysis.Parser;

public static class ValueParser
{
    public static ValueBase Parse(string text)
    {
        using var r = new TokenReader(text);
        return Parse(r);
    }

    public static ValueBase Parse(TokenReader r)
    {
        var operatorTokens = new string[] { "+", "-", "*", "/", "=", "!=", ">", "<", "<>", ">=", "<=", "||", "&", "|", "^", "#", "~", "and", "or" };

        ValueBase value = ParseMain(r);
        var sufix = TryReadSufix(r);
        if (sufix != null) value.Sufix = sufix;

        if (r.PeekRawToken().AreContains(operatorTokens))
        {
            var op = r.ReadToken();
            value.AddOperatableValue(op, Parse(r));
        }
        return value;
    }

    private static ValueBase ParseMain(TokenReader r)
    {
        var v = ParseCore(r);
        if (r.TryReadToken("between") != null)
        {
            return BetweenExpressionParser.Parse(v, r);
        }
        if (r.TryReadToken("like") != null)
        {
            return LikeExpressionParser.Parse(v, r);
        }
        return v;
    }

    internal static ValueBase ParseCore(TokenReader r)
    {
        var breaktokens = TokenReader.BreakTokens;

        var item = r.ReadToken();

        if (item.AreEqual("not"))
        {
            return new NegativeValue(Parse(r));
        }

        if (item.IsNumeric() || item.StartsWith("'") || item.AreEqual("true") || item.AreEqual("false"))
        {
            return new LiteralValue(item);
        }

        if (item == "(")
        {
            var (first, inner) = r.ReadUntilCloseBracket();
            if (first.AreEqual("select"))
            {
                return new InlineQuery(SelectQueryParser.Parse(inner));
            }
            return new BracketValue(Parse(inner));
        }

        if (item == "case")
        {
            var text = "case " + r.ReadUntilCaseExpressionEnd();
            return CaseExpressionParser.Parse(text);
        }

        if (item == "exists")
        {
            r.ReadToken("(");
            var (first, inner) = r.ReadUntilCloseBracket();
            return new ExistsExpression(SelectQueryParser.Parse(inner));
        }

        if (item == "in")
        {
            r.ReadToken("(");
            var (first, inner) = r.ReadUntilCloseBracket();
            if (first.AreEqual("select"))
            {
                return new InExpression(SelectQueryParser.Parse(inner));
            }
            return FunctionValueParser.Parse("(" + inner + ")", item);
        }

        if (r.PeekRawToken().AreEqual("("))
        {
            return FunctionValueParser.Parse(r, item);
        }

        if (r.PeekRawToken().AreEqual("."))
        {
            //table.column
            var table = item;
            r.ReadToken(".");
            return new ColumnValue(table, r.ReadToken());
        }

        //omit table column
        return new ColumnValue(item);
    }

    private static string? TryReadSufix(TokenReader r)
    {
        if (!r.PeekRawToken().AreEqual(":")) return null;
        var sufix = r.ReadToken(":");
        if (!r.PeekRawToken().AreEqual("(")) return sufix;

        r.ReadToken("(");
        var (_, inner) = r.ReadUntilCloseBracket();
        return sufix + "(" + inner + ")";
    }
}