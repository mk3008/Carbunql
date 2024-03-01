using Carbunql.Clauses;
using Carbunql.Tables;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Definitions;

public class CheckConstraint : IConstraint
{
	public string ConstraintName { get; set; } = string.Empty;

	public ValueBase Value { get; set; } = null!;

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

		yield return new Token(this, parent, "check", isReserved: true);

		yield return Token.ReservedBracketStart(this, parent);
		foreach (var item in Value.GetTokens(parent))
		{
			yield return item;
		}
		yield return Token.ReservedBracketEnd(this, parent);
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
