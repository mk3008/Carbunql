using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis.Parser;

public static class FromClauseParser
{
	public static FromClause Parse(string text)
	{
		using var r = new TokenReader(text);
		return Parse(r);
	}

	public static FromClause Parse(ITokenReader r)
	{
		var relationtokens = ReservedText.GetRelationTexts();

		var root = SelectableTableParser.Parse(r);
		var from = new FromClause(root);

		if (!r.Peek().AreContains(relationtokens))
		{
			return from;
		}
		from.Relations ??= new List<Relation>();

		do
		{
			from.Relations.Add(RelationParser.Parse(r));

		} while (r.Peek().AreContains(relationtokens));

		return from;
	}
}