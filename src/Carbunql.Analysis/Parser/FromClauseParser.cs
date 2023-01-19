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
		var relationtokens = TableJoinEnumReader.GetCommandAttributes().Select(x => x.Value.Text);

		var root = SelectableTableParser.Parse(r);
		var from = new FromClause(root);

		if (!r.PeekRawToken().AreContains(relationtokens))
		{
			return from;
		}
		from.Relations ??= new List<Relation>();

		do
		{
			from.Relations.Add(RelationParser.Parse(r));

		} while (r.PeekRawToken().AreContains(relationtokens));

		return from;
	}
}