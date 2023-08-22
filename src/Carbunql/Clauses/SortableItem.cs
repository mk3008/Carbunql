using Carbunql.Extensions;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Clauses;

[MessagePackObject]
public class SortableItem : IQueryCommandable
{
	public SortableItem(ValueBase value, bool isAscending = true, NullSort tp = NullSort.Undefined)
	{
		Value = value;
		IsAscending = isAscending;
		NullSort = tp;
	}

	[Key(0)]
	public ValueBase Value { get; init; }

	[Key(1)]
	public bool IsAscending { get; set; } = true;

	[Key(2)]
	public NullSort NullSort { get; set; } = NullSort.Undefined;

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

	public IDictionary<string, object?> GetParameters()
	{
		return Value.GetParameters();
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		foreach (var item in Value.GetTokens(parent)) yield return item;
		if (!IsAscending) yield return Token.Reserved(this, parent, "desc");
		if (NullSort != NullSort.Undefined) yield return Token.Reserved(this, parent, NullSort.ToCommandText());
	}
}