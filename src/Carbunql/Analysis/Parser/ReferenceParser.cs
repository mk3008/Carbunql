using Carbunql.Analysis.Parser;
using Carbunql.Definitions;

namespace Carbunql.Analysis;

public static class ReferenceParser
{
	public static ReferenceDefinition Parse(string text)
	{
		var r = new SqlTokenReader(text);
		var q = Parse(r);
		return q;
	}

	public static ReferenceDefinition Parse(ITokenReader r)
	{
		var token = r.Read("references");
		var table = r.Read();
		var columns = ArrayParser.Parse(r);

		return new ReferenceDefinition(table, columns);
	}
}
