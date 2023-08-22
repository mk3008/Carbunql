using Carbunql.Clauses;
using Carbunql.Values;
using MessagePack;

namespace Carbunql.Tables;

[MessagePackObject]
public class FunctionTable : TableBase
{
	public FunctionTable()
	{
		Name = string.Empty;
		Argument = null!;
	}

	public FunctionTable(string name)
	{
		Name = name;
		Argument = new ValueCollection();
	}

	public FunctionTable(string name, ValueBase args)
	{
		Name = name;
		Argument = new ValueCollection
		{
			args
		};
	}

	[Key(1)]
	public string Name { get; init; }

	[Key(2)]
	public ValueCollection Argument { get; init; }

	public override IEnumerable<Token> GetTokens(Token? parent)
	{
		yield return Token.Reserved(this, parent, Name);

		var bracket = Token.ReservedBracketStart(this, parent);
		yield return bracket;
		foreach (var item in Argument.GetTokens(bracket)) yield return item;
		yield return Token.ReservedBracketEnd(this, parent);
	}

	[Key(3)]
	public Dictionary<string, object?> Parameters { get; set; } = new();

	public override IDictionary<string, object?> GetParameters()
	{
		return Parameters;
	}

	public override IEnumerable<SelectQuery> GetInternalQueries()
	{
		foreach (var item in Argument.GetInternalQueries())
		{
			yield return item;
		}
	}

	public override IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		foreach (var item in Argument.GetPhysicalTables())
		{
			yield return item;
		}
	}
}