﻿using Carbunql.Extensions;

namespace Carbunql.Analysis;

public static class ReadQueryParser
{
	public static ReadQuery Parse(string text)
	{
		using var r = new TokenReader(text);
		return Parse(r);
	}

	public static ReadQuery Parse(ITokenReader r)
	{
		if (r.Peek().AreEqual("select")) return SelectQueryParser.Parse(r);
		if (r.Peek().AreEqual("values")) return ValuesQueryParser.Parse(r);

		throw new NotSupportedException(r.Peek());
	}
}