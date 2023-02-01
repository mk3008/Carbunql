using Carbunql.Extensions;

namespace Carbunql.Analysis;

public class BracketInnerTokenReader : ITokenReader, IDisposable
{
	public BracketInnerTokenReader(ITokenReader r)
	{
		Reader = r;
		RootBracketLevel = r.CurrentBracketLevel;
	}

	private ITokenReader Reader { get; set; }

	private int RootBracketLevel { get; set; }

	public int CurrentBracketLevel => Reader.CurrentBracketLevel;

	private bool IsTerminated { get; set; } = false;

	public string TerminatedToken { get; private set; } = string.Empty;

	public string Peek()
	{
		if (IsTerminated) return string.Empty;

		return Reader.Peek();
	}

	public string Read()
	{
		if (IsTerminated) return string.Empty;

		var token = Reader.Read();
		if (token == ")" && RootBracketLevel == Reader.CurrentBracketLevel)
		{
			IsTerminated = true;
			TerminatedToken = token;
		}
		return token;
	}

	public void SkipSpace()
	{
		Reader.SkipSpace();
	}

	public void Dispose()
	{
		Reader.ReadOrDefault(")");
	}
}