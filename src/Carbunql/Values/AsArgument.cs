using Carbunql.Clauses;
using MessagePack;

namespace Carbunql.Values;

[MessagePackObject]
public class AsArgument : ValueBase
{
	public AsArgument()
	{
		Value = null!;
		Type = null!;
	}

	public AsArgument(ValueBase value, ValueBase type)
	{
		Value = value;
		Type = type;
	}

	[Key(1)]
	public ValueBase Value { get; init; }

	[Key(2)]
	public ValueBase Type { get; init; }

	internal override IEnumerable<SelectQuery> GetInternalQueriesCore()
	{
		foreach (var item in Value.GetInternalQueries())
		{
			yield return item;
		}
		foreach (var item in Type.GetInternalQueries())
		{
			yield return item;
		}
	}

	public override IEnumerable<Token> GetCurrentTokens(Token? parent)
	{
		foreach (var item in Value.GetTokens(parent)) yield return item;
		yield return Token.Reserved(this, parent, "as");
		foreach (var item in Type.GetTokens(parent)) yield return item;
	}
}