using Carbunql.Extensions;
using Cysharp.Text;

namespace Carbunql.Analysis;

public abstract class TokenReader : IDisposable
{
	public TokenReader(string text)
	{
		Reader = new SqlLexReader(text);
	}

	private static IEnumerable<string> JoinLexs { get; set; } = new string[] { "inner", "cross" };

	private static IEnumerable<string> ByLexs { get; set; } = new string[] { "group", "partition", "order" };

	private static IEnumerable<string> OuterJoinLexs { get; set; } = new string[] { "left", "right" };

	private static IEnumerable<string> NullsSortTokens { get; set; } = new string[] { "first", "last" };

	private SqlLexReader Reader { get; set; }

	public int CurrentBracketLevel { get; private set; } = 0;

	public virtual string Read()
	{
		var lex = Reader.Read();

		if (string.IsNullOrEmpty(lex)) return string.Empty;

		if (lex == "(")
		{
			CurrentBracketLevel++;
			return lex;
		}

		if (lex == ")")
		{
			CurrentBracketLevel--;
			return lex;
		}

		using var sb = ZString.CreateStringBuilder();
		sb.Append(lex);

		// Explore possible two-word tokens
		if (lex.IsEqualNoCase("is"))
		{
			var next = Reader.Peek();
			if (next.IsEqualNoCase("not"))
			{
				sb.Append(" " + Reader.Read(next));
			}

			next = Reader.Peek();
			if (next.IsEqualNoCase("distinct"))
			{
				// is distinct from, is not distinct from
				sb.Append(" " + Reader.Read(next));
				sb.Append(" " + Reader.Read("from"));
				return sb.ToString();
			}

			// is, is not
			return sb.ToString();
		}

		if (lex.IsEqualNoCase(JoinLexs))
		{
			sb.Append(" " + Reader.Read("join"));
			return sb.ToString();
		}

		if (lex.IsEqualNoCase(ByLexs))
		{
			sb.Append(" " + Reader.Read("by"));
			return sb.ToString();
		}

		if (lex.IsEqualNoCase(OuterJoinLexs))
		{
			var next = Reader.Peek();
			//left(), right() function
			if (next.IsEqualNoCase("(")) return lex;

			Reader.TryRead("outer", out _);
			sb.Append(" " + Reader.Read("join"));
			return sb.ToString();
		}

		if (lex.IsEqualNoCase("nulls"))
		{
			sb.Append(" " + Reader.Read(NullsSortTokens));
			return sb.ToString();
		}

		if (lex.IsEqualNoCase("union"))
		{
			if (Reader.TryRead("all", out var all))
			{
				sb.Append(" " + all);
			}
			return sb.ToString();
		}

		if (lex.IsEqualNoCase("not"))
		{
			if (Reader.TryRead("materialized", out var materialized))
			{
				sb.Append(" " + materialized);
			}
			return sb.ToString();
		}

		if (lex.IsEqualNoCase("double"))
		{
			if (Reader.TryRead("precision", out var precision))
			{
				sb.Append(" " + precision);
			}
			return sb.ToString();
		}

		if (lex.IsEqualNoCase("at") || lex.IsEqualNoCase("without"))
		{
			sb.Append(" " + Reader.Read("time"));
			sb.Append(" " + Reader.Read("zone"));
			return sb.ToString();
		}

		return lex;
	}

	public void Dispose()
	{
		((IDisposable)Reader).Dispose();
	}
}