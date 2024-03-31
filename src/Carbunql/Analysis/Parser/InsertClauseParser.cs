using Carbunql.Clauses;

namespace Carbunql.Analysis.Parser;

public static class InsertClauseParser
{
	public static InsertClause Parse(string text)
	{
		var r = new SqlTokenReader(text);
		return Parse(r);
	}

	public static InsertClause Parse(ITokenReader r)
	{
		r.Read("insert into");
		return new InsertClause(SelectableTableParser.Parse(r));
	}
}