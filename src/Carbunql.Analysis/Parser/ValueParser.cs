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
		var operatorTokens = new string[] { "+", "-", "*", "/", "=", "!=", ">", "<", "<>", ">=", "<=", "||", "&", "|", "^", "#", "~", "and", "or", "is", "is not" };

		ValueBase value = ParseMain(r);
		var sufix = TryReadSufix(r);
		if (sufix != null) value.Sufix = sufix;

		if (r.Peek().AreContains(operatorTokens))
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
		if (r.ReadOrDefault("like") != null)
		{
			return LikeExpressionParser.Parse(v, r);
		}
		if (r.ReadOrDefault("in") != null)
		{
			return InExpressionParser.Parse(v, r);
		}
		return v;
	}

	internal static ValueBase ParseCore(ITokenReader r)
	{
		var item = r.Read();

		if (item.AreEqual("not"))
		{
			return new NegativeValue(Parse(r));
		}
		if (item.IsNumeric() || item.StartsWith("'") || item.AreEqual("true") || item.AreEqual("false"))
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
			var innerReader = new BracketInnerTokenReader(r);
			var pt = innerReader.Peek();

			ValueBase? v = null;
			if (pt.AreEqual("select"))
			{
				v = new InlineQuery(SelectQueryParser.Parse(innerReader));
			}
			else
			{
				v = new BracketValue(Parse(innerReader));
				r.ReadOrDefault(")");
			}
			return v;
		}

		if (item.AreEqual("case"))
		{
			return CaseExpressionParser.Parse(r);
		}

		if (item.AreEqual("exists"))
		{
			return new ExistsExpression(SelectQueryParser.ParseAsInner(r));
		}

		if (r.Peek().AreEqual("("))
		{
			var t = FunctionValueParser.Parse(r, item);
			r.ReadOrDefault(")");
			return t;
		}

		if (r.Peek().AreEqual("."))
		{
			//table.column
			var table = item;
			r.Read(".");
			return new ColumnValue(table, r.Read());
		}

		//omit table column
		return new ColumnValue(item);
	}

	private static string? TryReadSufix(ITokenReader r)
	{
		//ex ::timestamp, ::numeric(8)
		if (!r.Peek().StartsWith("::")) return null;
		var sufix = r.Read();
		if (!r.Peek().AreEqual("(")) return sufix;

		r.Read("(");

		var innerReader = new BracketInnerTokenReader(r);
		var v = ValueCollectionParser.Parse(innerReader);
		r.ReadOrDefault(")");

		return sufix + "(" + v.ToText() + ")";
	}

	private static string ReadUntilCaseExpressionEnd(ITokenReader r)
	{
		using var inner = ZString.CreateStringBuilder();

		var word = r.Read();
		while (!string.IsNullOrEmpty(word))
		{
			inner.Append(word);
			if (word.TrimStart().AreEqual("end"))
			{
				return inner.ToString();
			}
			if (word.TrimStart().AreEqual("case"))
			{
				inner.Append(ReadUntilCaseExpressionEnd(r));
			}
			word = r.Read();
		}

		throw new SyntaxException("case expression is not end");
	}
}