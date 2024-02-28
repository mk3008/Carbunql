using Carbunql.Analysis.Parser;
using Carbunql.Extensions;

namespace Carbunql.Analysis;

public static class AlterTableQueryParser
{
	public static AlterTableQuery Parse(string text)
	{
		var r = new SqlTokenReader(text);
		var q = Parse(r);

		if (!r.Peek().IsEndToken())
		{
			throw new NotSupportedException($"Parsing terminated despite the presence of unparsed tokens.(token:'{r.Peek()}')");
		}

		return q;
	}

	public static AlterTableQuery Parse(ITokenReader r)
	{
		var t = ParseAsAlterTableCommand(r);
		t.AlterColumnCommand = AlterCommandParser.Parse(r);
		return t;
	}

	private static AlterTableQuery ParseAsAlterTableCommand(ITokenReader r)
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

		return new AlterTableQuery(schema, table);
	}
}