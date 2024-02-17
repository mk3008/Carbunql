using Carbunql.Clauses;
using Carbunql.Tables;

namespace Carbunql.Definitions;

internal class NotNullConstraint : IConstraint
{
	public string ConstraintName { get; set; } = string.Empty;

	public string ColumnName { get; set; } = string.Empty;

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

		yield return new Token(this, parent, "not null", isReserved: true);
		yield return new Token(this, parent, ColumnName);
	}
}
