using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Values;

[MessagePackObject(keyAsPropertyName: true)]
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

	public FunctionValue(string name, OverClause winfn)
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

	public FunctionValue(string name, Func<OverClause> wfbuiilder)
	{
		Name = name;
		Argument = new ValueCollection();
		Over = wfbuiilder();
	}

	public FunctionValue(string name, ValueBase args, OverClause winfn)
	{
		Name = name;
		Argument = new ValueCollection
		{
			args
		};
		Over = winfn;
	}

	public FunctionValue(string name, ValueBase args, Func<OverClause> wfbuiilder)
	{
		Name = name;
		Argument = new ValueCollection
		{
			args
		};
		Over = wfbuiilder();
	}

	public FunctionValue(string name, Func<ValueBase> builder, Func<OverClause> wfbuiilder)
	{
		Name = name;
		Argument = new ValueCollection
		{
			builder()
		};
		Over = wfbuiilder();
	}

	public string Name { get; init; }

	public ValueCollection Argument { get; set; }

	public OverClause? Over { get; set; }

	public Filter? Filter { get; set; }

	protected override IEnumerable<SelectQuery> GetInternalQueriesCore()
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

	protected override IDictionary<string, object?> GetParametersCore()
	{
		var prm = Argument.GetParameters();
		prm = prm.Merge(Filter?.GetParameters());
		prm = prm.Merge(Over?.GetParameters());
		return prm;
	}

	protected override IEnumerable<PhysicalTable> GetPhysicalTablesCore()
	{
		foreach (var item in Argument.GetPhysicalTables())
		{
			yield return item;
		}
		if (Filter != null)
		{
			foreach (var item in Filter.GetPhysicalTables())
			{
				yield return item;
			}
		}
		if (Over != null)
		{
			foreach (var item in Over.GetPhysicalTables())
			{
				yield return item;
			}
		}
	}

	protected override IEnumerable<CommonTable> GetCommonTablesCore()
	{
		foreach (var item in Argument.GetCommonTables())
		{
			yield return item;
		}
		if (Filter != null)
		{
			foreach (var item in Filter.GetCommonTables())
			{
				yield return item;
			}
		}
		if (Over != null)
		{
			foreach (var item in Over.GetCommonTables())
			{
				yield return item;
			}
		}
	}
}