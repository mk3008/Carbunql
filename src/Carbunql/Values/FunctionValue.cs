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

	public FunctionValue(string name, WindowFunction winfn)
	{
		Name = name;
		Argument = new ValueCollection();
		WindowFunction = winfn;
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

	public FunctionValue(string name, Func<WindowFunction> wfbuiilder)
	{
		Name = name;
		Argument = new ValueCollection();
		WindowFunction = wfbuiilder();
	}

	public FunctionValue(string name, ValueBase args, WindowFunction winfn)
	{
		Name = name;
		Argument = new ValueCollection
		{
			args
		};
		WindowFunction = winfn;
	}

	public FunctionValue(string name, ValueBase args, Func<WindowFunction> wfbuiilder)
	{
		Name = name;
		Argument = new ValueCollection
		{
			args
		};
		WindowFunction = wfbuiilder();
	}

	public FunctionValue(string name, Func<ValueBase> builder, Func<WindowFunction> wfbuiilder)
	{
		Name = name;
		Argument = new ValueCollection
		{
			builder()
		};
		WindowFunction = wfbuiilder();
	}

	[Key(1)]
	public string Name { get; init; }

	[Key(2)]
	public ValueCollection Argument { get; init; }

	[Key(3)]
	public WindowFunction? WindowFunction { get; init; }

	internal override IEnumerable<SelectQuery> GetInternalQueriesCore()
	{
		foreach (var item in Argument.GetInternalQueries())
		{
			yield return item;
		}
		if (WindowFunction != null)
		{
			foreach (var item in WindowFunction.GetInternalQueries())
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

		if (WindowFunction != null)
		{
			foreach (var item in WindowFunction.GetTokens(parent)) yield return item;
		}
	}
}