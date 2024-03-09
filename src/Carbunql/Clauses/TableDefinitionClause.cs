using Carbunql.Definitions;
using Carbunql.Tables;

namespace Carbunql.Clauses;

public class TableDefinitionClause : QueryCommandCollection<ITableDefinition>
{
	public override IEnumerable<Token> GetTokens(Token? parent)
	{
		if (!Items.Any()) yield break;

		var bracket = Token.ReservedBracketStart(this, parent);
		yield return bracket;
		foreach (var item in base.GetTokens(bracket))
		{
			yield return item;
		}
		yield return Token.ReservedBracketEnd(this, parent);
	}

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		yield break;
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		yield break;
	}

	public IEnumerable<CommonTable> GetCommonTables()
	{
		yield break;
	}


	public override IEnumerable<QueryParameter> GetParameters()
	{
		yield break;
	}

	public IEnumerable<string> GetColumnNames()
	{
		var lst = new List<string>();
		foreach (var item in Items.Where(x => !string.IsNullOrEmpty(x.ColumnName)))
		{
			lst.Add(item.ColumnName);
		}
		return lst.Distinct();
	}

	public TableDefinitionClause ToNormalize(ITable table)
	{
		var clause = new TableDefinitionClause();
		foreach (var item in Items.OfType<ColumnDefinition>())
		{
			if (item.TryNormalize(table, out var column))
			{
				clause.Add(column);
			}
		}
		return clause;
	}

	public List<AlterTableQuery> Disasseble(ITable table)
	{
		var lst = new List<AlterTableQuery>();

		//normalize unknown name primary key
		var pkeys = Items.OfType<ColumnDefinition>().Where(x => x.IsPrimaryKey).Select(x => x.ColumnName).Distinct();
		if (pkeys.Any())
		{
			var c = new PrimaryKeyConstraint() { ColumnNames = pkeys.ToList() };
			lst.Add(new AlterTableQuery(new AlterTableClause(table, c)));
		}

		//normalize unknown name unique key
		var ukeys = Items.OfType<ColumnDefinition>().Where(x => x.IsUniqueKey).Select(x => x.ColumnName).Distinct();
		if (ukeys.Any())
		{
			var c = new UniqueConstraint() { ColumnNames = ukeys.ToList() };
			lst.Add(new AlterTableQuery(new AlterTableClause(table, c)));
		}

		//disassemble
		foreach (var def in Items)
		{
			if (def.TryDisasseble(out var constraint))
			{
				lst.Add(new AlterTableQuery(new AlterTableClause(table, constraint)));
			}
		}

		return lst;
	}
}