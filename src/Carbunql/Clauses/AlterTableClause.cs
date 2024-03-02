using Carbunql.Definitions;
using Carbunql.Tables;

namespace Carbunql.Clauses;

public class AlterTableClause : QueryCommandCollection<IAlterCommand>, IQueryCommandable, ITable
{
	public AlterTableClause(ITable t)
	{
		Schema = t.Schema;
		Table = t.Table;
	}

	public AlterTableClause(ITable t, IAlterCommand command)
	{
		Schema = t.Schema;
		Table = t.Table;
		Items.Add(command);
	}

	public AlterTableClause(string schema, string table)
	{
		Schema = schema;
		Table = table;
	}

	public AlterTableClause(string table)
	{
		Table = table;
	}

	public string? Schema { get; init; } = null;

	public string Table { get; init; }

	public string TableFullName => (string.IsNullOrEmpty(Schema)) ? Table : Schema + "." + Table;

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		yield break;
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		yield break;
	}

	public override IEnumerable<Token> GetTokens(Token? parent)
	{
		if (!Items.Any()) throw new InvalidOperationException();


		var altertable = Token.Reserved(this, parent, "alter table");
		yield return altertable;
		yield return new Token(this, parent, TableFullName);

		foreach (var item in base.GetTokens(altertable))
		{
			yield return item;
		}
	}

	public IEnumerable<CommonTable> GetCommonTables()
	{
		yield break;
	}
}