using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis.Parser;

public class DistinctClauseParser
{
	public static DistinctClause Parse(string text)
	{
		var r = new SqlTokenReader(text);
		return Parse(r);
	}

	public static DistinctClause Parse(ITokenReader r)
	{
		r.Read("distinct");

		if (!r.Peek().IsEqualNoCase("on"))
		{
			return new DistinctClause();
		}

		r.Read("on");
		r.Read("(");
		var values = ValueCollectionParser.Parse(r);
		r.Read(")");

		return new DistinctClause(values);
	}
}