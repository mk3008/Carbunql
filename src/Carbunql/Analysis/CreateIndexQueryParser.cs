using Carbunql.Analysis.Parser;
using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis;

public static class CreateIndexQueryParser
{
	public static CreateIndexQuery Parse(string text)
	{
		var r = new SqlTokenReader(text);
		var q = Parse(r);

		if (!r.Peek().IsEndToken())
		{
			throw new NotSupportedException($"Parsing terminated despite the presence of unparsed tokens.(token:'{r.Peek()}')");
		}

		return q;
	}

	public static CreateIndexQuery Parse(ITokenReader r)
	{
		var t = ParseAsCreateIndexCommand(r);

		if (r.Peek().IsEqualNoCase("where"))
		{
			t.WhereClause = WhereClauseParser.Parse(r);
		}
		return t;
	}

	private static CreateIndexQuery ParseAsCreateIndexCommand(ITokenReader r)
	{
		var isUnique = false;
		var token = r.Read();
		if (token.IsEqualNoCase("create unique index"))
		{
			isUnique = true;
		}
		else if (token.IsEqualNoCase("create index"))
		{
			isUnique = false;
		}
		else
		{
			throw new NotSupportedException($"Token:{token}");
		}

		var indexName = string.Empty;
		if (!r.Peek().IsEqualNoCase("on"))
		{
			indexName = r.Read();
		}

		var clause = ParseAsOnClause(r);
		return new CreateIndexQuery(clause)
		{
			IsUnique = isUnique,
			IndexName = indexName
		};
	}

	private static IndexOnClause ParseAsOnClause(ITokenReader r)
	{
		r.Read("on");
		var table = ParseAsTableName(r);
		var clause = new IndexOnClause(table.schema, table.name);

		if (r.Peek().IsEqualNoCase("using"))
		{
			r.Read();
			clause.Using = r.Read();
		}

		r.Read("(");
		do
		{
			r.ReadOrDefault(",");
			var c = SortableItemParser.Parse(r);
			clause.Add(c);
		} while (r.Peek() == ",");
		r.Read(")");

		return clause;
	}

	private static (string schema, string name) ParseAsTableName(ITokenReader r)
	{
		var token = r.Read();

		var schema = string.Empty;
		string? name;
		if (r.Peek() == ".")
		{
			r.Read(".");
			schema = token;
			name = r.Read();
		}
		else
		{
			name = token;
		}

		return (schema, name);
	}
}