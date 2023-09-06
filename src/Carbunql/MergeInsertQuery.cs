using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;
using Carbunql.Values;

namespace Carbunql;

public class MergeInsertQuery : IQueryCommandable
{
	public ValueCollection? Datasource { get; set; }

	public ValueCollection? Destination { get; set; }

	public virtual IDictionary<string, object?> GetParameters()
	{
		var prm = EmptyParameters.Get();
		prm = prm.Merge(Destination?.GetParameters());
		prm = prm.Merge(Datasource?.GetParameters());
		return prm;
	}

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		if (Datasource != null)
		{
			foreach (var item in Datasource.GetInternalQueries())
			{
				yield return item;
			}
		}
		if (Destination != null)
		{
			foreach (var item in Destination.GetInternalQueries())
			{
				yield return item;
			}
		}
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		if (Datasource != null)
		{
			foreach (var item in Datasource.GetPhysicalTables())
			{
				yield return item;
			}
		}
		if (Destination != null)
		{
			foreach (var item in Destination.GetPhysicalTables())
			{
				yield return item;
			}
		}
	}
	public IEnumerable<CommonTable> GetCommonTables()
	{
		if (Datasource != null)
		{
			foreach (var item in Datasource.GetCommonTables())
			{
				yield return item;
			}
		}
		if (Destination != null)
		{
			foreach (var item in Destination.GetCommonTables())
			{
				yield return item;
			}
		}
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		if (Destination == null) yield break;
		if (Datasource == null) yield break;

		foreach (var item in GetInsertTokens(parent)) yield return item;
		foreach (var item in GetValuesTokens(parent)) yield return item;
	}

	private IEnumerable<Token> GetInsertTokens(Token? parent)
	{
		if (Destination == null) yield break;

		var t = Token.Reserved(this, parent, "insert");
		yield return t;
		var bracket = Token.ReservedBracketStart(this, parent);
		yield return bracket;
		foreach (var item in Destination.GetTokens(bracket)) yield return item;
		yield return Token.ReservedBracketEnd(this, parent);
	}

	private IEnumerable<Token> GetValuesTokens(Token? parent)
	{
		if (Datasource == null) yield break;

		var t = Token.Reserved(this, parent, "values");
		yield return t;
		var bracket = Token.ReservedBracketStart(this, parent);
		yield return bracket;
		foreach (var item in Datasource.GetTokens(bracket)) yield return item;
		yield return Token.ReservedBracketEnd(this, parent);
	}
}