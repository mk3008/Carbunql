using Carbunql.Clauses;
using Carbunql.Definitions;

namespace Carbunql;

public class DefinitionQuerySet
{
	public DefinitionQuerySet(CreateTableQuery createTableQuery)
	{
		CreateTableQuery = createTableQuery;
	}

	public CreateTableQuery CreateTableQuery { get; init; }

	public List<AlterTableQuery> AlterTableQueries { get; set; } = new();

	public List<CreateIndexQuery> CreateIndexQueries { get; set; } = new();

	public DefinitionQuerySet Normarize()
	{
		var q = CreateTableQuery.Normarize();
		var clause = q.CreateTableQuery.DefinitionClause;
		if (clause == null) throw new Exception();

		// External command
		foreach (var constraint in CreateTableQuery.DefinitionClause!.OfType<IConstraint>())
		{
			q.AlterTableQueries.Add(new AlterTableQuery(new AlterTableClause(q.CreateTableQuery, constraint)));
		}

		// apply
		foreach (var atq in AlterTableQueries.SelectMany(x => x.Disassemble()))
		{
			if (!atq.TryIntegrate(clause))
			{
				q.AlterTableQueries.Add(atq);
			}
		}

		q.CreateIndexQueries.AddRange(CreateIndexQueries);
		return q;
	}

	public IEnumerable<string> GetColumnNames()
	{
		var def = CreateTableQuery.DefinitionClause;
		if (def == null) throw new NullReferenceException(nameof(CreateTableQuery.DefinitionClause));

		return def.GetColumnNames();
	}

	public bool Exists(string columnName)
	{
		return GetColumnNames().Contains(columnName);
	}

	public AlterTableQuery CreateDropColumnQuery(string columnName)
	{
		var clause = new AlterTableClause(CreateTableQuery)
		{
			new DropColumnCommand(columnName)
		};
		return new AlterTableQuery(clause);
	}

	public ColumnDefinition GetColumnDefinition(string columnName)
	{
		var def = CreateTableQuery.DefinitionClause;
		if (def == null) throw new NullReferenceException(nameof(CreateTableQuery.DefinitionClause));

		return def.Where(x => x is ColumnDefinition c && c.ColumnName == columnName).Select(x => (ColumnDefinition)x).First();
	}

	public AlterTableQuery CreateAddColumnQuery(string columnName)
	{
		var def = GetColumnDefinition(columnName);

		var clause = new AlterTableClause(CreateTableQuery)
		{
			new AddColumnCommand(def)
		};
		return new AlterTableQuery(clause);
	}

	public List<IAlterCommand> CreateAlterColumnCommands(ColumnDefinition expect, ColumnDefinition actual)
	{
		var commands = new List<IAlterCommand>();

		// type 
		if (expect.ColumnType != actual.ColumnType)
		{
			commands.Add(new ChangeColumnTypeCommand(expect.ColumnName, expect.ColumnType));
		}

		// nullable
		if (expect.IsNullable != actual.IsNullable)
		{
			if (expect.IsNullable)
			{
				commands.Add(new SetNotNullCommand(expect.ColumnName));
			}
			else
			{
				commands.Add(new DropNotNullCommand(expect.ColumnName));
			}
		}

		//default
		if (expect.DefaultValueDefinition != null && actual.DefaultValueDefinition == null)
		{
			commands.Add(new SetDefaultCommand(expect.ColumnName, expect.DefaultValueDefinition));
		}
		else if (expect.DefaultValueDefinition == null && actual.DefaultValueDefinition != null)
		{
			commands.Add(new DropDefaultCommand(expect.ColumnName));
		}
		else if (expect.DefaultValueDefinition != null && actual.DefaultValueDefinition != null)
		{
			if (expect.DefaultValueDefinition.ToText() != actual.DefaultValueDefinition.ToText())
			{
				commands.Add(new SetDefaultCommand(expect.ColumnName, expect.DefaultValueDefinition));
			}
		}

		return commands;
	}
}
