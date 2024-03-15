using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis.Parser;

public static class AlterTableClauseParser
{
	public static AlterTableClause Parse(string text)
	{
		var r = new SqlTokenReader(text);
		var q = Parse(r);

		if (!r.Peek().IsEndToken())
		{
			throw new NotSupportedException($"Parsing terminated despite the presence of unparsed tokens.(token:'{r.Peek()}')");
		}

		return q;
	}

	public static AlterTableClause Parse(ITokenReader r)
	{
		var t = ParseAsAlterTableCommand(r);
		do
		{
			t.Add(AlterCommandParser.Parse(t, r));
		} while (r.TryRead(",", out _));
		return t;
	}

	private static AlterTableClause ParseAsAlterTableCommand(ITokenReader r)
	{
		r.Read("alter table");

		var token = r.Read();

		var schema = string.Empty;
		string? table;
		if (r.Peek() == ".")
		{
			r.Read(".");
			schema = token;
			table = r.Read();
		}
		else
		{
			table = token;
		}

		return new AlterTableClause(schema, table);
	}
}