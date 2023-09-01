using Carbunql.Clauses;
using MessagePack;

namespace Carbunql.Values;

[MessagePackObject]
public class FunctionValue : ValueBase
{
	public FunctionValue()
	{
		Name = null!;
		Argument = null!;
	}

	public FunctionValue(string name)
	{
		Name = name;
		Argument = new ValueCollection();
	}

	public FunctionValue(string name, Over winfn)
	{
		Name = name;
		Argument = new ValueCollection();
		Over = winfn;
	}

	public FunctionValue(string name, string arg)
	{
		Name = name;
		Argument = new ValueCollection(arg);
	}

	public FunctionValue(string name, ValueBase args)
	{
		Name = name;
		Argument = new ValueCollection
		{
			args
		};
	}

	public FunctionValue(string name, Func<ValueBase> builder)
	{
		Name = name;
		Argument = new ValueCollection
		{
			builder()
		};
	}

	public FunctionValue(string name, Func<Over> wfbuiilder)
	{
		Name = name;
		Argument = new ValueCollection();
		Over = wfbuiilder();
	}

	public FunctionValue(string name, ValueBase args, Over winfn)
	{
		Name = name;
		Argument = new ValueCollection
		{
			args
		};
		Over = winfn;
	}

	public FunctionValue(string name, ValueBase args, Func<Over> wfbuiilder)
	{
		Name = name;
		Argument = new ValueCollection
		{
			args
		};
		Over = wfbuiilder();
	}

	public FunctionValue(string name, Func<ValueBase> builder, Func<Over> wfbuiilder)
	{
		Name = name;
		Argument = new ValueCollection
		{
			builder()
		};
		Over = wfbuiilder();
	}

	[Key(1)]
	public string Name { get; init; }

	[Key(2)]
	public ValueCollection Argument { get; set; }

	[Key(3)]
	public Over? Over { get; set; }

	[Key(4)]
	public Filter? Filter { get; set; }

	internal override IEnumerable<SelectQuery> GetInternalQueriesCore()
	{
		foreach (var item in Argument.GetInternalQueries())
		{
			yield return item;
		}
		if (Filter != null)
		{
			foreach (var item in Filter.GetInternalQueries())
			{
				yield return item;
			}
		}
		if (Over != null)
		{
			foreach (var item in Over.GetInternalQueries())
			{
				yield return item;
			}
		}
	}

	public override IEnumerable<Token> GetCurrentTokens(Token? parent)
	{
		yield return Token.Reserved(this, parent, Name);

		var bracket = Token.ReservedBracketStart(this, parent);
		yield return bracket;
		foreach (var item in Argument.GetTokens(bracket)) yield return item;
		yield return Token.ReservedBracketEnd(this, parent);

		if (Filter != null)
		{
			foreach (var item in Filter.GetTokens(parent)) yield return item;
		}

		if (Over != null)
		{
			foreach (var item in Over.GetTokens(parent)) yield return item;
		}
	}
}