using Carbunql.Extensions;

namespace Carbunql.Clauses;

public class SortableItem : IQueryCommandable
{
	public SortableItem(ValueBase value, bool isAscending = true, NullSort tp = NullSort.Undefined)
	{
		Value = value;
		IsAscending = isAscending;
		NullSort = tp;
	}

	public ValueBase Value { get; init; }

	public bool IsAscending { get; set; } = true;

	public NullSort NullSort { get; set; } = NullSort.Undefined;

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