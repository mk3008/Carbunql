namespace Carbunql.Analysis;

public class BracketInnerTokenReader : ITokenReader, IDisposable
{
	private string StartSymbol { get; init; } = "(";

	private string EndSymbol { get; init; } = ")";

	public BracketInnerTokenReader(ITokenReader r, string startSymbol, string endSymbol)
	{
		StartSymbol = startSymbol;
		EndSymbol = endSymbol;

		r.Read(StartSymbol);

		Reader = r;
		RootBracketLevel = r.CurrentBracketLevel;
	}

	public BracketInnerTokenReader(ITokenReader r)
	{
		r.Read(StartSymbol);

		Reader = r;
		RootBracketLevel = r.CurrentBracketLevel;
	}

	private ITokenReader Reader { get; set; }

	private int RootBracketLevel { get; set; }

	public int CurrentBracketLevel => Reader.CurrentBracketLevel;

	private bool IsTerminated { get; set; } = false;

	public string Peek()
	{
		if (IsTerminated) return string.Empty;

		return Reader.Peek();
	}

	public string Read()
	{
		if (IsTerminated) return string.Empty;

		var token = Reader.Read();
		if (token == EndSymbol && RootBracketLevel > Reader.CurrentBracketLevel)
		{
			IsTerminated = true;
		}
		return token;
	}

	public void Dispose()
	{
		Reader.Read(EndSymbol);
	}

	public void RollBack()
	{
		throw new NotImplementedException();
	}
}