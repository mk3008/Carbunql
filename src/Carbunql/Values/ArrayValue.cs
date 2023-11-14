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
		if (arg is ValueCollection vc)
		{
			Argument = vc;
		}
		else
		{
			Argument = new ValueCollection(arg);
		}
	}

	public string Name => "array";

	public ValueCollection Argument { get; set; }

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

		yield return Token.Reserved(this, parent, "[");
		foreach (var item in Argument.GetTokens(parent)) yield return item;
		yield return Token.Reserved(this, parent, "]");
	}

	protected override IDictionary<string, object?> GetParametersCore()
	{
		var prm = Argument.GetParameters();
		return prm;
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