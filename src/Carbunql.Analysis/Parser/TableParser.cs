using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;

namespace Carbunql.Analysis.Parser;

public static class TableParser
{
	public static TableBase Parse(string text)
	{
		using var r = new TokenReader(text);
		return Parse(r);
	}

	public static TableBase Parse(ITokenReader r)
	{
		if (r.Peek().IsEqualNoCase("("))
		{
			return VirtualTableParser.Parse(r);
		}

		var item = r.Read();

		if (r.Peek().IsEqualNoCase("."))
		{
			//schema.table
			var schema = item;
			r.Read(".");
			return new PhysicalTable(schema, r.Read());
		}

		if (r.Peek().IsEqualNoCase("("))
		{
			return FunctionTableParser.Parse(r, item);
		}

		//table
		return new PhysicalTable(item);
	}
}