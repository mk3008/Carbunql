using Carbunql.Clauses;
using Carbunql.Tables;

namespace Carbunql.Definitions;

public class PrimaryKeyConstraint : IConstraint
{
	public string ConstraintName { get; set; } = string.Empty;

	public List<string> ColumnNames { get; set; } = new();

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
}
