using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

public static class ArrayValueParser
{
	public static ValueBase Parse(string argument)
	{
		var r = new SqlTokenReader(argument);
		return Parse(r);
	}

	public static ValueBase Parse(ITokenReader r)
	{
		var token = r.Read("array");
		var next = r.Peek();
		if (next.First() == '[' && next.Last() == ']')
		{
			// It is interpreted as a SQL Server token, so disassemble it
			next = r.Read();
			var text = next.Substring(1, next.Length - 2);
			var value = ValueCollectionParser.Parse(text);
			return new ArrayValue(value);
		}
		else if (BracketValueParser.IsBracketValue(next))
		{
			var value = BracketValueParser.Parse(r);
			return new ArrayValue(value);
		}
		throw new NotSupportedException();
	}
}