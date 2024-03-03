﻿using Carbunql.Clauses;
using Carbunql.Tables;

namespace Carbunql.Definitions;

public class AddColumnCommand : IAlterCommand
{
	public AddColumnCommand(ColumnDefinition definition)
	{
		Definition = definition;
	}

	public ColumnDefinition Definition { get; set; }

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
		yield return new Token(this, parent, "add", isReserved: true);
		yield return new Token(this, parent, "column", isReserved: true);
		foreach (var item in Definition.GetTokens(parent))
		{
			yield return item;
		}
	}

	public bool TryIntegrate(TableDefinitionClause clause)
	{
		var q = clause.OfType<ColumnDefinition>().Where(x => x.ColumnName == Definition.ColumnName);
		if (q.Any()) throw new InvalidOperationException();

		clause.Add(Definition);
		return true;
	}
}