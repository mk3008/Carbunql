using Carbunql.Clauses;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Values;

[MessagePackObject(keyAsPropertyName: true)]
public class ArrayValue : ValueBase
{
	public ArrayValue()
	{
		Argument = null!;
	}

	public ArrayValue(ValueBase arg)
	{
		Argument = arg;
	}

	public string Name => "array";

	public ValueBase Argument { get; set; }

	protected override IEnumerable<SelectQuery> GetInternalQueriesCore()
	{
		foreach (var item in Argument.GetInternalQueries())
		{
			yield return item;
		}
	}

	public override IEnumerable<Token> GetCurrentTokens(Token? parent)
	{
		yield return Token.Reserved(this, parent, Name);

		if (Argument is ValueCollection) yield return Token.Reserved(this, parent, "[");
		foreach (var item in Argument.GetTokens(parent)) yield return item;
		if (Argument is ValueCollection) yield return Token.Reserved(this, parent, "]");
	}

	protected override IEnumerable<QueryParameter> GetParametersCore()
	{
		return Argument.GetParameters();
	}

	protected override IEnumerable<PhysicalTable> GetPhysicalTablesCore()
	{
		foreach (var item in Argument.GetPhysicalTables())
		{
			yield return item;
		}
	}

	protected override IEnumerable<CommonTable> GetCommonTablesCore()
	{
		foreach (var item in Argument.GetCommonTables())
		{
			yield return item;
		}
	}
}