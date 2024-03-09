using Carbunql.Extensions;

namespace Carbunql.Analysis;

public static class DefinitionQuerySetParser
{
	public static DefinitionQuerySet Parse(string text)
	{
		var r = new SqlTokenReader(text);
		var q = new DefinitionQuerySet();

		while (r.TryReadNextQuery(out var token))
		{
			if (token.IsEqualNoCase("create table"))
			{
				q.CreateTableQuery = CreateTableQueryParser.Parse(r);
			}
			else if (token.IsEqualNoCase("alter table"))
			{
				q.AlterTableQueries.Add(AlterTableQueryParser.Parse(r));
			}
			else if (token.IsEqualNoCase("create index") || token.IsEqualNoCase("create unique index"))
			{
				q.AlterIndexQueries.Add(CreateIndexQueryParser.Parse(r));
			}

			if (!r.Peek().IsEndToken())
			{
				throw new NotSupportedException($"Parsing terminated despite the presence of unparsed tokens.(token:'{r.Peek()}')");
			}
		}

		return q;
	}
}
