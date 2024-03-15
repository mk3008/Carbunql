using Carbunql.Clauses;
using Carbunql.Tables;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Definitions;

internal class ForeignKeyConstraint : IConstraint
{
	public ForeignKeyConstraint(ITable t)
	{
		Schema = t.Schema;
		Table = t.Table;
	}

	public string ConstraintName { get; set; } = string.Empty;

	public List<string> ColumnNames { get; set; } = new();

	public ReferenceDefinition Reference { get; set; } = null!;

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

		yield return new Token(this, parent, "foreign key", isReserved: true);
		yield return Token.ExpressionBracketStart(this, parent);
		foreach (var item in ColumnNames)
		{
			yield return new Token(this, parent, item);
		}
		yield return Token.ExpressionBracketEnd(this, parent);

		foreach (var item in Reference.GetTokens(parent))
		{
			yield return item;
		}
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
