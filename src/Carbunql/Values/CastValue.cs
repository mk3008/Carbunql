using Carbunql.Analysis.Parser;
using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Values;

[MessagePackObject(keyAsPropertyName: true)]
public class CastValue : ValueBase
{
	public CastValue()
	{
		Inner = null!;
		Symbol = null!;
		Type = null!;
	}

	public CastValue(ValueBase inner, string symbol, ValueBase type)
	{
		Inner = inner;
		Symbol = symbol;
		Type = type;
	}

	public CastValue(ValueBase inner, string symbol, string type)
	{
		Inner = inner;
		Symbol = symbol;
		Type = ValueParser.Parse(type);
	}

	public ValueBase Inner { get; init; }

	public string Symbol { get; init; }

	public ValueBase Type { get; init; }

	protected override IEnumerable<SelectQuery> GetInternalQueriesCore()
	{
		foreach (var item in Inner.GetInternalQueries())
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
		if (Symbol.IsEqualNoCase("as"))
		{
			yield return Token.Reserved(this, parent, "cast");

			var bracket = Token.ExpressionBracketStart(this, parent);
			yield return bracket;
			foreach (var item in Inner.GetTokens(bracket)) yield return item;
			yield return Token.Reserved(this, bracket, Symbol);
			foreach (var item in Type.GetTokens(bracket)) yield return item;
			yield return Token.ExpressionBracketEnd(this, parent);
		}
		else
		{
			foreach (var item in Inner.GetTokens(parent)) yield return item;
			yield return Token.Reserved(this, parent, Symbol);
			foreach (var item in Type.GetTokens(parent)) yield return item;
		}
	}

	protected override IEnumerable<QueryParameter> GetParametersCore()
	{
		foreach (var item in Inner.GetParameters())
		{
			yield return item;
		}
		foreach (var item in Type.GetParameters())
		{
			yield return item;
		}
	}

	protected override IEnumerable<PhysicalTable> GetPhysicalTablesCore()
	{
		foreach (var item in Inner.GetPhysicalTables())
		{
			yield return item;
		}
		foreach (var item in Type.GetPhysicalTables())
		{
			yield return item;
		}
	}

	protected override IEnumerable<CommonTable> GetCommonTablesCore()
	{
		foreach (var item in Inner.GetCommonTables())
		{
			yield return item;
		}
		foreach (var item in Type.GetCommonTables())
		{
			yield return item;
		}
	}
}