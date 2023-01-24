using Carbunql.Extensions;
using Carbunql.Tables;
using System.ComponentModel.DataAnnotations;

namespace Carbunql.Analysis.Parser;

public class VirtualTableParser
{
	public static VirtualTable Parse(ITokenReader r)
	{
		r.ReadToken("(");
		var ir = new InnerTokenReader(r);

		var first = ir.PeekRawToken();

		if (first == null) throw new NotSupportedException();

		//virtualTable
		if (first.AreEqual("select"))
		{
			return new VirtualTable(SelectQueryParser.Parse(ir));
		}
		else if (first.AreEqual("values"))
		{
			return new VirtualTable(ValuesClauseParser.Parse(ir));
		}
		throw new NotSupportedException();
	}
}
