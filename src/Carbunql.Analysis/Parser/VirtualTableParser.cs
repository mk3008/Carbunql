using Carbunql.Extensions;
using Carbunql.Tables;
using System.ComponentModel.DataAnnotations;

namespace Carbunql.Analysis.Parser;

public class VirtualTableParser
{
	public static VirtualTable Parse(ITokenReader r)
	{
		r.Read("(");

		var first = r.Peek();

		if (first == null) throw new NotSupportedException();

		//virtualTable
		if (first.AreEqual("select"))
		{
			var t = new VirtualTable(SelectQueryParser.Parse(r));
			r.ReadOrDefault(")");
			return t;
		}
		else if (first.AreEqual("values"))
		{
			var t = new VirtualTable(ValuesClauseParser.Parse(r));
			r.Read(")");
			return t;
		}
		throw new NotSupportedException();
	}
}
