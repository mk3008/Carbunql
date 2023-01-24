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
		if (r.PeekRawToken().AreEqual("("))
		{
			return VirtualTableParser.Parse(r);
		}

		var item = r.ReadToken();

		if (r.PeekRawToken().AreEqual("."))
		{
			//schema.table
			var schema = item;
			r.ReadToken(".");
			return new PhysicalTable(schema, r.ReadToken());
		}

		if (r.PeekRawToken().AreEqual("("))
		{
			return FunctionTableParser.Parse(r, item);
		}

		//table
		return new PhysicalTable(item);
	}
}