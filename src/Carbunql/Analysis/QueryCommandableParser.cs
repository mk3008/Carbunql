using Carbunql.Analysis.Parser;
using Carbunql.Building;
using Carbunql.Extensions;

namespace Carbunql.Analysis;

public static class QueryCommandableParser
{
	public static IQueryCommandable Parse(string text)
	{
		var r = new SqlTokenReader(text);
		var q = Parse(r);

		if (!r.Peek().IsEndToken())
		{
			throw new NotSupportedException($"Parsing terminated despite the presence of unparsed tokens.(token:'{r.Peek()}')");
		}

		return q;
	}

	public static IQueryCommandable Parse(ITokenReader r)
	{
		var token = r.Peek();

		if (token.IsEqualNoCase("with"))
		{
			var clause = WithClauseParser.Parse(r);

			token = r.Peek();
			if (token.IsEqualNoCase("select"))
			{
				var sq = SelectQueryParser.Parse(r);
				sq.WithClause = clause;
				return sq;
			}
			else if (token.IsEqualNoCase("insert into"))
			{
				// NOTE
				// Although this is a preliminary specification,
				// insert queries themselves do not allow CTEs.
				// So if her CTE is mentioned in the insert query,
				// it will be forced to be treated as her CTE in the select query.
				var iq = InsertQueryParser.Parse(r);
				if (iq.Query is SelectQuery sq)
				{
					foreach (var item in clause)
					{
						sq.With(item);
					}
				}
				return iq;
			}

			throw new NotSupportedException();
		}
		else
		{
			if (token.IsEqualNoCase("select")) return SelectQueryParser.Parse(r);
			if (token.IsEqualNoCase("values")) return ValuesQueryParser.Parse(r);

			if (token.IsEqualNoCase("insert into")) return InsertQueryParser.Parse(r);

			if (token.IsEqualNoCase("create table")) return CreateTableQueryParser.Parse(r);
			if (token.IsEqualNoCase("create index")) return CreateIndexQueryParser.Parse(r);

			if (token.IsEqualNoCase("alter table")) return AlterTableQueryParser.Parse(r);

			throw new NotSupportedException();
		}
	}
}