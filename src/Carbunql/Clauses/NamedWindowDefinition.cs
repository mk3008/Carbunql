using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Clauses;

[MessagePackObject(keyAsPropertyName: true)]
public class NamedWindowDefinition : IQueryCommandable
{
	public NamedWindowDefinition()
	{
		Alias = null!;
		WindowDefinition = null!;
	}

	public NamedWindowDefinition(string alias, WindowDefinition definition)
	{
		Alias = alias;
		WindowDefinition = definition;
	}

	public string Alias { get; set; }

	public WindowDefinition WindowDefinition { get; set; }

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		foreach (var item in WindowDefinition.GetInternalQueries()) yield return item;
	}

	public IDictionary<string, object?> GetParameters()
	{
		return WindowDefinition.GetParameters();
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		foreach (var item in WindowDefinition.GetPhysicalTables()) yield return item;
	}

	public IEnumerable<CommonTable> GetCommonTables()
	{
		foreach (var item in WindowDefinition.GetCommonTables()) yield return item;
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		yield return new Token(this, parent, Alias); ;

		yield return Token.Reserved(this, parent, "as");

		foreach (var item in WindowDefinition.GetTokens(parent)) yield return item;
	}
}
