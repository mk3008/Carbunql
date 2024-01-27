using Carbunql.Extensions;
using Cysharp.Text;

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

		var token = Reads(skipSpace: true).FirstOrDefault();

		if (token == null) return;

		//skip comment block
		while (token.IsEqualNoCase(CommentTokens))
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
			token = Reads(skipSpace: true).FirstOrDefault();
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

	private string ReadSingleToken(string expect)
	{
		var token = Peek();
		if (token.IsEqualNoCase(expect))
		{
			Commit();
			return token;
		}
		throw new Exception($"expect : '{expect}', actual : '{token}'");
	}

	private string ReadSingleToken(IEnumerable<string> expects)
	{
		var token = Peek();
		if (token.IsEqualNoCase(expects))
		{
			Commit();
			return token;
		}
		var s = expects.ToList().ToString(", ", "'", "'");
		throw new Exception($"expects : {expects}, actual : '{token}'");
	}

	private string? ReadSingleTokenOrDefault(string? expect)
	{
		var token = Peek();
		if (expect != null && token.IsEqualNoCase(expect))
		{
			Commit();
			return token;
		}
		return null;
	}

	private string ReadSingleToken()
	{
		var token = Peek();
		Commit();
		return token;
	}

	private string ReadAndJoinOrDefault(string basetoken, string expect)
	{
		var next = ReadSingleTokenOrDefault(expect);
		if (next == null) return basetoken;
		return basetoken + " " + next;
	}

	private string ReadAndJoin(string basetoken, string expect)
	{
		var next = ReadSingleToken(expect);
		return basetoken + " " + next;
	}

	private string ReadAndJoin(string basetoken, IEnumerable<string> expects)
	{
		var next = ReadSingleToken(expects);
		return basetoken + " " + next;
	}

	private string ReadUntilCloseBlockComment()
	{
		using var inner = ZString.CreateStringBuilder();

		foreach (var word in Reads(skipSpace: false))
		{
			if (word == null) break;

			inner.Append(word);
			if (word.IsEqualNoCase("*/"))
			{
				return inner.ToString();
			}
			if (word.IsEqualNoCase("/*"))
			{
				inner.Append(ReadUntilCloseBlockComment());
			}
		}

		throw new SyntaxException("block comment is not closed");
	}

	public virtual string Read()
	{
		var token = ReadSingleToken();

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
		if (token.IsEqualNoCase("is"))
		{
			var isToken = ReadAndJoinOrDefault(token, "not");
			var isDistinctToken = ReadAndJoinOrDefault(isToken, "distinct");
			if (isToken == isDistinctToken) return isToken;
			return ReadAndJoin(isDistinctToken, "from");
		}

		if (token.IsEqualNoCase(JoinTokens))
		{
			return ReadAndJoin(token, "join");
		}

		if (token.IsEqualNoCase(new string[] { "group", "partition", "order" }))
		{
			return ReadAndJoin(token, "by");
		}

		if (token.IsEqualNoCase(OuterJoinTokens))
		{
			//left(), right() function
			if (Peek().IsEqualNoCase("(")) return token;

			ReadSingleTokenOrDefault("outer");
			return ReadAndJoin(token, "join");
		}

		if (token.IsEqualNoCase("nulls"))
		{
			return ReadAndJoin(token, NullsSortTokens);
		}

		if (token.IsEqualNoCase("union"))
		{
			return ReadAndJoinOrDefault(token, "all");
		}

		if (token.IsEqualNoCase("not"))
		{
			return ReadAndJoinOrDefault(token, "materialized");
		}

		if (token.IsEqualNoCase("double"))
		{
			return ReadAndJoinOrDefault(token, "precision");
		}

		if (token.IsEqualNoCase("at") || token.IsEqualNoCase("without"))
		{
			token = ReadAndJoin(token, "time");
			return ReadAndJoin(token, "zone");
		}

		return token;
	}
}