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
		var operatorTokens = new string[] { "+", "-", "*", "/", "%", "=", "!=", ">", "<", "<>", ">=", "<=", "||", "&", "|", "^", "#", "~", "~*", "!~", "!~*", "and", "or", "is", "is not" };

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
		if (r.ReadOrDefault("between") != null)
		{
			return BetweenExpressionParser.Parse(v, r);
		}
		else if (r.ReadOrDefault("like") != null)
		{
			return LikeExpressionParser.Parse(v, r);
		}
		else if (r.ReadOrDefault("in") != null)
		{
			return InExpressionParser.Parse(v, r);
		}
		else if (r.ReadOrDefault("::") != null)
		{
			return CastValueParser.Parse(v, "::", r);
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

		if (item.StartsWith(new String[] { ":", "@", "?"}))
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