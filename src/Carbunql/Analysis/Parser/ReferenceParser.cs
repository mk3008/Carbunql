using Carbunql.Analysis.Parser;
using Carbunql.Definitions;
using Carbunql.Extensions;

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

		var option = string.Empty;
		if (r.Peek().IsEqualNoCase("on"))
		{
			r.Read();
			option = r.Read();
		}

		return new ReferenceDefinition(table, columns) { Option = option };
	}
}
