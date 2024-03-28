using Carbunql.Clauses;
using Carbunql.Tables;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Definitions;

public class PrimaryKeyConstraint : IConstraint
{
	public PrimaryKeyConstraint(string schema, string table)
	{
		Schema = schema;
		Table = table;
	}

	public PrimaryKeyConstraint(ITable t)
	{
		Schema = t.Schema;
		Table = t.Table;
	}

	public string ConstraintName { get; set; } = string.Empty;

	public List<string> ColumnNames { get; set; } = new();

	public string ColumnName => string.Empty;

	public string? Schema { get; init; }

	public string Table { get; init; }

	public IEnumerable<CommonTable> GetCommonTables()
	{
		yield break;
	}

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		yield break;
	}

	public IEnumerable<QueryParameter> GetParameters()
	{
		yield break;
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		yield break;
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		if (!string.IsNullOrEmpty(ConstraintName))
		{
			yield return new Token(this, parent, "constraint", isReserved: true);
			yield return new Token(this, parent, ConstraintName);
		}

		yield return new Token(this, parent, "primary key", isReserved: true);

		yield return Token.ReservedBracketStart(this, parent);
		foreach (var item in ColumnNames)
		{
			yield return new Token(this, parent, item);
		}
		yield return Token.ReservedBracketEnd(this, parent);
	}

	public bool TrySet(TableDefinitionClause clause)
	{
		return false;
	}

	public bool TryDisasseble([MaybeNullWhen(false)] out IConstraint constraint)
	{
		constraint = this;
		return true;
	}

	//public bool TryToIndex([MaybeNullWhen(false)] out CreateIndexQuery query)
	//{
	//	query = default;
	//	return false;
	//}
}
