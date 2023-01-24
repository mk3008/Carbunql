using Carbunql.Extensions;

namespace Carbunql.Analysis;

public class InnerTokenReader : ITokenReader
{
	public InnerTokenReader(ITokenReader r)
	{
		Reader = r;
		RootCommentLevel = r.CommentLevel;
		BreakTokens = (new List<string> { ";", ")" }).AsReadOnly();
	}

	public InnerTokenReader(ITokenReader r, string endToken)
	{
		Reader = r;
		RootCommentLevel = r.CommentLevel;
		BreakTokens = (new List<string> { ";", endToken }).AsReadOnly();
	}

	public InnerTokenReader(ITokenReader r, IEnumerable<string> endTokens)
	{
		var lst = new List<string>();
		lst.Add(";");
		endTokens.ToList().ForEach(x => lst.Add(x));

		Reader = r;
		RootCommentLevel = r.CommentLevel;
		BreakTokens = lst.AsReadOnly();
	}

	public ITokenReader Reader { get; set; }

	private IReadOnlyList<string> BreakTokens { get; init; }

	private int RootCommentLevel { get; set; }

	public int CommentLevel => Reader.CommentLevel;

	private bool IsTerminated { get; set; } = false;

	private string? TokenCache { get; set; } = null;

	public string TerminatedToken { get; private set; } = string.Empty;

	private string? ReadTokenMain()
	{
		//Token reading processing and termination judgment are performed

		if (!string.IsNullOrEmpty(TokenCache))
		{
			var s = TokenCache;
			TokenCache = null;
			return s;
		}

		var tmp = Reader.ReadToken();

		// termination check
		if (string.IsNullOrEmpty(tmp))
		{
			IsTerminated = true;
			TerminatedToken = string.Empty;
		}
		else if (CommentLevel == 0 && tmp.AreContains(BreakTokens))
		{
			IsTerminated = true;
			TerminatedToken = tmp;
		}

		return tmp;
	}

	public string? PeekRawToken()
	{
		//Cache ReadRawToken to implement peek functionality
		if (!string.IsNullOrEmpty(TokenCache)) return TokenCache;

		TokenCache = ReadTokenMain();
		return TokenCache;
	}

	public string ReadToken()
	{
		var t = ReadTokenMain();
		if (string.IsNullOrEmpty(t)) return string.Empty;
		return t;
	}

	public void SkipSpace()
	{
		Reader.SkipSpace();
	}
}