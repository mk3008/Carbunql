using Carbunql.Clauses;
using Carbunql.Tables;
using System.Diagnostics.CodeAnalysis;

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

	public IEnumerable<AlterTableQuery> ToAlterTableQueries(ITable t)
	{
		yield return new AlterTableQuery(t) { AlterColumnCommand = this.ToAddCommand() };
	}

	public bool TryToPlainColumn(ITable t, [MaybeNullWhen(false)] out ColumnDefinition column)
	{
		column = null;
		return false;
	}
}
