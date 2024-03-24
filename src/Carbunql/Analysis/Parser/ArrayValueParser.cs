using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

public static class ArrayValueParser
{
	public static ArrayValue Parse(string argument)
	{
		var r = new SqlTokenReader(argument);
		return Parse(r);
	}

	public static ArrayValue Parse(ITokenReader r)
	{
		r.Read("array");
		var val = r.Read();
		if (val.First() == '[' && val.Last() == ']')
		{
			var text = val.Substring(1, val.Length - 2);
			var collection = ValueCollectionParser.Parse(text);
			return new ArrayValue(collection);
		}
		throw new NotSupportedException();
	}
}