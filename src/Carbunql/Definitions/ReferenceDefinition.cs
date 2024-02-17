using Carbunql.Clauses;
using Carbunql.Tables;

namespace Carbunql.Definitions;

public class ReferenceDefinition : IConstraint
{
	public ReferenceDefinition(string tableName, List<string> columnNames)
	{
		TableName = tableName;
		ColumnNames = columnNames;
	}

	public string TableName { get; set; }

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
		yield return new Token(this, parent, "references", isReserved: true);
		yield return new Token(this, parent, TableName);

		yield return Token.ExpressionBracketStart(this, parent);
		foreach (var item in ColumnNames)
		{
			yield return new Token(this, parent, item);
		}
		yield return Token.ExpressionBracketEnd(this, parent);
	}
}
