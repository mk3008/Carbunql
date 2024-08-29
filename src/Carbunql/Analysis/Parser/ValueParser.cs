using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses SQL values from text or token streams.
/// </summary>
public static class ValueParser
{
    private static string[] ArithmeticOperators = new[]
    {
        "+", "-", "*", "/", "%", "=", "!", ">", "<", "|", "&", "^", "#", "~"
    };

    private static string[] LogicalOperators = new[]
    {
        "and", "is", "is distinct from", "is not", "is not distinct from", "or", "uescape"
    };

    private static string[] BareFunctions = new[]{
        "current_date",
        "current_time",
        "current_timestamp",
        "localtime",
        "localtimestamp",
        "session_user",
        "user",
        "current_user"
    };

    /// <summary>
    /// Parses a SQL value from the provided text.
    /// </summary>
    /// <param name="text">The SQL text containing the value.</param>
    /// <returns>The parsed SQL value.</returns>
    public static ValueBase Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

    /// <summary>
    /// Parses a SQL value from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed SQL value.</returns>
    public static ValueBase Parse(ITokenReader r)
    {
        ValueBase value = ParseMain(r);

        if (r.Peek().StartsWith(ArithmeticOperators) || r.Peek().IsEqualNoCase(LogicalOperators))
        {
            var op = r.Read();
            if (op == "*" && r.Peek().IsEqualNoCase("from"))
            {
                r.RollBack();
                return value;
            }

            value.AddOperatableValue(op, Parse(r));
        }
        return value;
    }

    private static ValueBase ParseMain(ITokenReader r)
    {
        var v = ParseCore(r);

        var item = r.Peek();

        if (item.IsEqualNoCase("at time zone"))
        {
            return AtTimeZoneClauseParser.Parse(v, r);
        }
        else if (item.IsEqualNoCase("without time zone"))
        {
            return WithoutTimeZoneClauseParser.Parse(v, r);
        }

        var isNegative = false;
        if (item.IsEqualNoCase("not"))
        {
            r.Read("not");
            item = r.Peek();
            isNegative = true;
        }

        if (item.IsEqualNoCase("between"))
        {
            return BetweenClauseParser.Parse(v, r, isNegative);
        }
        else if (item.IsEqualNoCase("like"))
        {
            return LikeClauseParser.Parse(v, r, isNegative);
        }
        else if (item.IsEqualNoCase("in"))
        {
            return InClauseParser.Parse(v, r, isNegative);
        }

        if (isNegative)
        {
            throw new SyntaxException("A 'not' token that negates a value has the syntax 'not between', 'not like', or 'not in'.");
        }

        if (CastValueParser.IsCastValue(item))
        {
            var symbol = r.Read();
            return CastValueParser.Parse(v, symbol, r);
        }

        return v;
    }

    internal static ValueBase ParseCore(ITokenReader r)
    {
        var item = r.Peek();

        if (string.IsNullOrEmpty(item)) throw new EndOfStreamException();

        if (item.IsEqualNoCase("interval"))
        {
            r.Read();
            return new Interval(Parse(r));
        }

        if (NegativeValueParser.IsNegativeValue(item))
        {
            return NegativeValueParser.Parse(r);
        }

        if (item.IsEqualNoCase("null") || LiteralValueParser.IsLiteralValue(item))
        {
            return LiteralValueParser.Parse(r);
        }

        if (item == "+" || item == "-")
        {
            // Signs indicating positive and negative are not considered operators.
            // ex. '+1', '-1' 
            var sign = r.Read();
            var v = (LiteralValue)Parse(r);
            v.CommandText = sign + v.CommandText;
            return v;
        }

        if (item.IsEqualNoCase("array"))
        {
            return ArrayValueParser.Parse(r);
        }

        if (BracketValueParser.IsBracketValue(item))
        {
            return BracketValueParser.Parse(r);
        }

        if (CaseExpressionParser.IsCaseExpression(item))
        {
            return CaseExpressionParser.Parse(r);
        }

        if (ExistsExpressionParser.IsExistsExpression(item))
        {
            return ExistsExpressionParser.Parse(r);
        }

        if (ParameterValueParser.IsParameterValue(item))
        {
            return ParameterValueParser.Parse(r);
        }

        item = r.Read();

        if (r.Peek() == "(")
        {
            return FunctionValueParser.Parse(r, functionName: item);
        }

        if (r.Peek() == ".")
        {
            var value = item;
            while (r.Peek() == ".")
            {
                r.Read(".");
                value += "." + r.Read();
            };

            var parts = value.Split(".");
            var column = parts[parts.Length - 1];
            var table = value.Substring(0, value.Length - column.Length - 1);

            return new ColumnValue(table, column);
        }

        if (item.IsEqualNoCase(BareFunctions))
        {
            return new BareFunctionValue(item);
        }

        // Omit table column
        return new ColumnValue(item);
    }
}
