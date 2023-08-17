using Carbunql.Clauses;

namespace Carbunql.Values;

public class FunctionValue : ValueBase
{
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

	public string Name { get; init; }

	public ValueCollection Argument { get; init; }

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