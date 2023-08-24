using Carbunql.Clauses;
using MessagePack;

namespace Carbunql.Values;

[MessagePackObject]
public class ParameterValue : ValueBase
{
	public ParameterValue()
	{
		Key = null!;
	}

	public ParameterValue(string key)
	{
		Key = key;
	}

	public ParameterValue(string key, object? value)
	{
		Key = key;
		Value = value;
		Parameters.Add(key, value);
	}

	[Key(1)]
	public string Key { get; set; }

	[Key(2)]
	public object? Value { get; set; } = null;

	[Key(3)]
	public Dictionary<string, object?> Parameters { get; set; } = new();

	public override IDictionary<string, object?> GetParameters()
	{
		return Parameters;
	}

	public override IEnumerable<Token> GetCurrentTokens(Token? parent)
	{
		yield return new Token(this, parent, Key);
	}
}
