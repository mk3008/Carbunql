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

	public static ValueBase Parse(ITokenReader r)
	{
		var operatorTokens = new string[] { "+", "-", "*", "/", "%", "=", "!=", ">", "<", "<>", ">=", "<=", "||", "&", "|", "^", "#", "~", "~*", "!~", "!~*", "and", "or", "is", "is not", "is distinct from", "is not distinct from" };

		ValueBase value = ParseMain(r);

		if (r.Peek().IsEqualNoCase(operatorTokens))
		{
			var op = r.Read();
			value.AddOperatableValue(op, Parse(r));
		}
		return value;
	}

	private static ValueBase ParseMain(ITokenReader r)
	{
		var v = ParseCore(r);

		var isNegative = false;
		if (r.ReadOrDefault("not") != null)
		{
			isNegative = true;
		}

		if (r.ReadOrDefault("between") != null)
		{
			return BetweenClauseParser.Parse(v, r, isNegative);
		}
		else if (r.ReadOrDefault("like") != null)
		{
			return LikeExpressionParser.Parse(v, r, isNegative);
		}
		else if (r.ReadOrDefault("in") != null)
		{
			return InClauseParser.Parse(v, r, isNegative);
		}
		else if (!isNegative && r.ReadOrDefault("::") != null)
		{
			return CastValueParser.Parse(v, "::", r);
		}

		if (isNegative)
		{
			throw new SyntaxException("A 'not' token that negates a value has the syntax 'not between', 'not like', or 'not in'.");
		}

		return v;
	}

	internal static ValueBase ParseCore(ITokenReader r)
	{
		var item = r.Read();

		if (String.IsNullOrEmpty(item)) throw new EndOfStreamException();

		if (item.IsEqualNoCase("not"))
		{
			return new NegativeValue(Parse(r));
		}
		if (item.IsNumeric() || item.StartsWith("'") || item.IsEqualNoCase("true") || item.IsEqualNoCase("false"))
		{
			return new LiteralValue(item);
		}

		if (item == "+" || item == "-")
		{
			var sign = item;
			var v = (LiteralValue)Parse(r);
			v.CommandText = sign + v.CommandText;
			return v;
		}

		if (item == "(")
		{
			using var ir = new BracketInnerTokenReader(r);
			var pt = ir.Peek();

			ValueBase? v = null;
			if (pt.IsEqualNoCase("select"))
			{
				v = new InlineQuery(SelectQueryParser.Parse(ir));
			}
			else
			{
				v = new BracketValue(Parse(ir));
			}
			return v;
		}

		if (item.IsEqualNoCase("case"))
		{
			return CaseExpressionParser.Parse(r);
		}

		if (item.IsEqualNoCase("exists"))
		{
			return new ExistsExpression(SelectQueryParser.ParseAsInner(r));
		}

		if (r.Peek().IsEqualNoCase("("))
		{
			var t = FunctionValueParser.Parse(r, item);
			return t;
		}

		if (r.Peek().IsEqualNoCase("."))
		{
			//table.column
			var table = item;
			r.Read(".");
			return new ColumnValue(table, r.Read());
		}

		if (item.StartsWith(new String[] { ":", "@", "?" }))
		{
			return new ParameterValue(item);
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