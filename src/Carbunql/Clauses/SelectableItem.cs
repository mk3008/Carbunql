using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Clauses;

[MessagePackObject(keyAsPropertyName: true)]
public class SelectableItem : IQueryCommandable, ISelectable
{
	public SelectableItem(ValueBase value, string alias)
	{
		Value = value;
		Alias = alias;
	}

	public ValueBase Value { get; private set; }

	public string Alias { get; private set; }

	public void SetValue(ValueBase value)
	{
		Value = value;
	}

	public void SetAlias(string alias)
	{
		Alias = alias;
	}

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		foreach (var item in Value.GetInternalQueries())
		{
			yield return item;
		}
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		foreach (var item in Value.GetPhysicalTables())
		{
			yield return item;
		}
	}

	public IEnumerable<CommonTable> GetCommonTables()
	{
		foreach (var item in Value.GetCommonTables())
		{
			yield return item;
		}
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		foreach (var item in Value.GetTokens(parent)) yield return item;
		if (!string.IsNullOrEmpty(Alias) && Alias != Value.GetDefaultName())
		{
			yield return Token.Reserved(this, parent, "as");
			yield return new Token(this, parent, Alias);
		}
	}

	public IEnumerable<QueryParameter> GetParameters()
	{
		return Value.GetParameters();
	}
}