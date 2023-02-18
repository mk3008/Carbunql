using Carbunql.Clauses;

namespace Carbunql.Values;

public class ParameterValue : ValueBase
{
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

	public string Key { get; set; }

	public object? Value { get; set; } = null;

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
