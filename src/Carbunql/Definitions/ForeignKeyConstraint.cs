using Carbunql.Clauses;
using Carbunql.Tables;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Definitions;

internal class ForeignKeyConstraint : IConstraint
{
	public string ConstraintName { get; set; } = string.Empty;

	public List<string> ColumnNames { get; set; } = new();

	public ReferenceDefinition Reference { get; set; } = null!;

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

	public IEnumerable<AlterTableQuery> ToAlterTableQueries(ITable t)
	{
		yield return new AlterTableQuery(new AlterTableClause(t, ToCommand()));
	}

	public bool TryToPlainColumn(ITable t, [MaybeNullWhen(false)] out ColumnDefinition column)
	{
		column = null;
		return false;
	}

	public AddConstraintCommand ToCommand()
	{
		return new AddConstraintCommand(this);
	}
}
