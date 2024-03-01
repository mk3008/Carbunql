using Carbunql.Extensions;

namespace Carbunql.Analysis;

public static class DefinitionQuerySetParser
{
	public static DefinitionQuerySet Parse(string text)
	{
		var r = new SqlTokenReader(text);
		var q = Parse(r);

		if (!r.Peek().IsEndToken())
		{
			throw new NotSupportedException($"Parsing terminated despite the presence of unparsed tokens.(token:'{r.Peek()}')");
		}

		while (r.TryReadNextQuery(out var token))
		{
			if (token.IsEqualNoCase("alter table"))
			{
				q.AlterTableQueries.Add(AlterTableQueryParser.Parse(r));
			}
			else if (token.IsEqualNoCase("create index") || token.IsEqualNoCase("create unique index"))
			{
				q.CreateIndexQueries.Add(CreateIndexQueryParser.Parse(r));
			}

			if (!r.Peek().IsEndToken())
			{
				throw new NotSupportedException($"Parsing terminated despite the presence of unparsed tokens.(token:'{r.Peek()}')");
			}

		}

		return q;
	}

	public static DefinitionQuerySet Parse(ITokenReader r)
	{
		var ct = CreateTableQueryParser.Parse(r);
		var q = new DefinitionQuerySet(ct);
		return q;
	}
}
