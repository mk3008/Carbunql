using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Values;
using Cysharp.Text;

namespace Carbunql.Analysis.Parser;

public static class ValueParser
{
	public static ValueBase Parse(string text)
	{
		using var r = new TokenReader(text);
		return Parse(r);
	}

	private static string[] ArithmeticOperators = new[]
	{
		"+", "-", "*", "/", "%", "=", "!", ">", "<", "|", "&", "^", "#", "~"
	};

	private static string[] LogicalOperators = new[]
	{
		"and", "is", "is distinct from", "is not", "is not distinct from", "or"
	};

	private static string[] Operators => ArithmeticOperators.Concat(LogicalOperators).ToArray();

	public static ValueBase Parse(ITokenReader r)
	{

		ValueBase value = ParseMain(r);

		if (r.Peek().StartsWith(ArithmeticOperators) || r.Peek().IsEqualNoCase(LogicalOperators))
		{
			var op = r.Read();
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
			return WithoutTImeZoneClauseParser.Parse(v, r);
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

		if (NegativeValueParser.IsNegativeValue(item))
		{
			return NegativeValueParser.Parse(r);
		}

		if (item == "null" || LiteralValueParser.IsLiteralValue(item))
		{
			return LiteralValueParser.Parse(r);
		}

		if (item == "+" || item == "-")
		{
			//Signs indicating positive and negative are not considered operators.
			//ex. '+1', '-1' 
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
			//table.column
			var table = item;
			r.Read(".");
			return new ColumnValue(table, r.Read());
		}

		//omit table column
		return new ColumnValue(item);
	}

	private static string ReadUntilCaseExpressionEnd(ITokenReader r)
	{
		using var inner = ZString.CreateStringBuilder();

		var word = r.Read();
		while (!string.IsNullOrEmpty(word))
		{
			inner.Append(word);
			if (word.TrimStart().IsEqualNoCase("end"))
			{
				return inner.ToString();
			}
			if (word.TrimStart().IsEqualNoCase("case"))
			{
				inner.Append(ReadUntilCaseExpressionEnd(r));
			}
			word = r.Read();
		}

		throw new SyntaxException("case expression is not end");
	}
}