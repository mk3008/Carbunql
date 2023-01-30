using Carbunql.Extensions;
using Cysharp.Text;
using System.Security.Cryptography;

namespace Carbunql.Analysis;

public class VanillaTokenReader : LexReader
{
	public VanillaTokenReader(string text) : base(text)
	{
	}

	private IEnumerable<string> CommentTokens { get; set; } = new string[] { "--", "/*" };

	private IEnumerable<string> JoinTokens { get; set; } = new string[] { "inner", "cross" };

	private IEnumerable<string> OuterJoinTokens { get; set; } = new string[] { "left", "right" };

	private IEnumerable<string> NullsSortTokens { get; set; } = new string[] { "first", "last" };

	private string TokenCache { get; set; } = string.Empty;

	public int CurrentBracketLevel { get; private set; } = 0;

	private void RefreshTokenCache()
	{
		if (!string.IsNullOrEmpty(TokenCache)) return;

		var token = ReadLexs(skipSpace: true).FirstOrDefault();

		if (token == null) return;

		//skip comment block
		while (token.AreContains(CommentTokens))
		{
			if (token == "--")
			{
				//line comment
				ReadUntilLineEnd();
			}
			else
			{
				//block comment
				ReadUntilCloseBlockComment();
			}
			token = ReadLexs(skipSpace: true).FirstOrDefault();
		}

		if (token == null) return;

		TokenCache = token;
	}

	private string Peek()
	{
		RefreshTokenCache();
		return TokenCache;
	}

	private void Commit()
	{
		TokenCache = string.Empty;
	}

	private string TryReadSingleToken(string expect)
	{
		var token = Peek();
		if (token.AreEqual(expect))
		{
			Commit();
			return token;
		}
		throw new Exception($"expect : '{expect}', actual : '{token}'");
	}

	private string TryReadSingleToken(IEnumerable<string> expects)
	{
		var token = Peek();
		if (token.AreContains(expects))
		{
			Commit();
			return token;
		}
		var s = expects.ToList().ToString(", ", "'", "'");
		throw new Exception($"expects : {expects}, actual : '{token}'");
	}

	private string? TryReadSingleTokenOrDefault(string? expect)
	{
		var token = Peek();
		if (expect != null && token.AreEqual(expect))
		{
			Commit();
			return token;
		}
		return null;
	}

	private string ReadSingleTokenOrDefault()
	{
		var token = Peek();
		Commit();
		return token;
	}

	private string ReadAndJoinOrDefault(string basetoken, string expect)
	{
		var next = TryReadSingleTokenOrDefault(expect);
		if (next == null) return basetoken;
		return basetoken + " " + next;
	}

	private string ReadAndJoin(string basetoken, string expect)
	{
		var next = TryReadSingleToken(expect);
		return basetoken + " " + next;
	}

	private string ReadAndJoin(string basetoken, IEnumerable<string> expects)
	{
		var next = TryReadSingleToken(expects);
		return basetoken + " " + next;
	}

	private string ReadUntilCloseBlockComment()
	{
		using var inner = ZString.CreateStringBuilder();

		foreach (var word in ReadLexs(skipSpace: false))
		{
			if (word == null) break;

			inner.Append(word);
			if (word.AreEqual("*/"))
			{
				return inner.ToString();
			}
			if (word.AreEqual("/*"))
			{
				inner.Append(ReadUntilCloseBlockComment());
			}
		}

		throw new SyntaxException("block comment is not closed");
	}

	public virtual string Read()
	{
		var token = ReadSingleTokenOrDefault();

		if (string.IsNullOrEmpty(token)) return string.Empty;

		if (token == "(")
		{
			CurrentBracketLevel++;
			return token;
		}

		if (token == ")")
		{
			CurrentBracketLevel--;
			return token;
		}

		// Explore possible two-word tokens
		if (token.AreEqual("is"))
		{
			return ReadAndJoinOrDefault(token, "not");
		}

		if (token.AreContains(JoinTokens))
		{
			return ReadAndJoin(token, "join");
		}

		if (token.AreContains(new string[] { "group", "partition", "order" }))
		{
			return ReadAndJoin(token, "by");
		}

		if (token.AreContains(OuterJoinTokens))
		{
			//left(), right() function
			if (Peek().AreEqual("(")) return token;

			TryReadSingleTokenOrDefault("outer");
			return ReadAndJoin(token, "join");
		}

		if (token.AreEqual("nulls"))
		{
			return ReadAndJoin(token, NullsSortTokens);
		}

		if (token.AreEqual("union"))
		{
			return ReadAndJoinOrDefault(token, "all");
		}

		if (token.AreEqual("not"))
		{
			return ReadAndJoinOrDefault(token, "materialized");
		}

		if (token.AreEqual(":"))
		{
			//ex ::text
			var c = PeekOrDefaultChar();
			if (c == ':')
			{
				return token + ReadSingleTokenOrDefault();
			}
		}

		return token;
	}
}