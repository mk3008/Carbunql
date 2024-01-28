using Carbunql.Tables;

namespace Carbunql.Analysis.Parser;

public class LateralTableParser
{
	public static LateralTable Parse(ITokenReader r)
	{
		r.Read("lateral");
		var t = new LateralTable(TableParser.Parse(r));
		return t;
	}
}
