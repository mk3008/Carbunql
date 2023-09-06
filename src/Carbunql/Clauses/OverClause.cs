using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Clauses;

[MessagePackObject(keyAsPropertyName: true)]
public class OverClause : IQueryCommandable
{
	public OverClause()
	{
		WindowDefinition = null!;
	}

	public OverClause(WindowDefinition definition)
	{
		WindowDefinition = definition;
	}

	public WindowDefinition WindowDefinition { get; set; }

	public IEnumerable<CommonTable> GetCommonTables()
	{
		foreach (var item in WindowDefinition.GetCommonTables())
		{
			yield return item;
		}
	}

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		foreach (var item in WindowDefinition.GetInternalQueries())
		{
			yield return item;
		}
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		foreach (var item in WindowDefinition.GetPhysicalTables())
		{
			yield return item;
		}
	}

	public IDictionary<string, object?> GetParameters()
	{
		return WindowDefinition.GetParameters();
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		var overToken = Token.Reserved(this, parent, "over");
		yield return overToken;

		foreach (var item in WindowDefinition.GetTokens(overToken)) yield return item;
	}
}