using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

public static class CommonTableParser
{
	public static CommonTable Parse(string text)
	{
		using var r = new SqlTokenReader(text);
		return Parse(r);
	}

	public static CommonTable Parse(ITokenReader r)
	{
		var alias = r.Read();
		ValueCollection? colAliases = null;
		if (r.Peek().IsEqualNoCase("("))
		{
			colAliases = ValueCollectionParser.ParseAsInner(r);
		}

		r.Read("as");

		var material = Materialized.Undefined;
		if (r.Peek().IsEqualNoCase("materialized"))
		{
			r.Read("materialized");
			material = Materialized.Materialized;
		}
		else if (r.Peek().IsEqualNoCase("not materialized"))
		{
			r.Read("not materialized");
			material = Materialized.NotMaterialized;
		}

		var t = VirtualTableParser.Parse(r);
		if (colAliases != null)
		{
			return new CommonTable(t, alias, colAliases) { Materialized = material };
		}
		else
		{
			return new CommonTable(t, alias) { Materialized = material };
		}
	}
}