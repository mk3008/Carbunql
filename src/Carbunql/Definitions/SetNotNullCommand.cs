﻿using Carbunql.Clauses;
using Carbunql.Tables;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Definitions;

public class SetNotNullCommand : IAlterCommand
{
	public SetNotNullCommand(ITable t, string columnName)
	{
		ColumnName = columnName;
		Schema = t.Schema;
		Table = t.Table;
	}

	public string ColumnName { get; set; }

	public string? Schema { get; init; }

	public string Table { get; init; } = string.Empty;

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
		yield return new Token(this, parent, "alter column", isReserved: true);
		yield return new Token(this, parent, ColumnName);
		yield return new Token(this, parent, "set", isReserved: true);
		yield return new Token(this, parent, "not null", isReserved: true);
	}

	public bool TrySet(TableDefinitionClause clause)
	{
		var c = clause.OfType<ColumnDefinition>().Where(x => x.ColumnName == ColumnName).First();
		c.IsNullable = false;
		return true;
	}

	public bool TryToIndex([MaybeNullWhen(false)] out CreateIndexQuery query)
	{
		query = default;
		return false;
	}
}