using Carbunql.Analysis;
using Carbunql.Clauses;
using Carbunql.Definitions;
using Cysharp.Text;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql;

public class DefinitionQuerySet
{
	public DefinitionQuerySet()
	{
	}

	public DefinitionQuerySet(CreateTableQuery createTableQuery)
	{
		CreateTableQuery = createTableQuery;
	}

	public CreateTableQuery? CreateTableQuery { get; set; } = null;

	public List<AlterTableQuery> AlterTableQueries { get; set; } = new();

	public List<CreateIndexQuery> CreateIndexQueries { get; set; } = new();

	public DefinitionQuerySet Merge()
	{
		var q = Normalize();

		var dic = new Dictionary<string, AlterTableQuery>();

		//merge alter table query
		foreach (var alterquery in q.AlterTableQueries)
		{
			var key = alterquery.AlterTableClause.TableFullName;
			if (!dic.ContainsKey(key))
			{
				var alt = new AlterTableQuery(new AlterTableClause(alterquery.AlterTableClause));
				dic.Add(key, alt);
			}
			var value = dic[key];
			foreach (var command in alterquery.AlterTableClause)
			{
				value.AlterTableClause.Add(command);
			}
		}

		q.AlterTableQueries.Clear();
		q.AlterTableQueries.AddRange(dic.Select(x => x.Value));

		return q;
	}

	public DefinitionQuerySet Normalize()
	{
		DefinitionQuerySet q;
		if (CreateTableQuery != null)
		{
			q = CreateTableQuery.Normarize();
		}
		else
		{
			q = new DefinitionQuerySet();
		}

		var clause = q.CreateTableQuery?.DefinitionClause;

		// integrate
		foreach (var atq in AlterTableQueries.SelectMany(x => x.Disassemble()))
		{
			if (clause == null || !atq.TryIntegrate(clause))
			{
				q.AlterTableQueries.Add(atq);
			}
		}

		q.CreateIndexQueries.AddRange(CreateIndexQueries);

		return q;
	}

	public IEnumerable<string> GetColumnNames()
	{
		if (CreateTableQuery != null)
		{
			var def = CreateTableQuery.DefinitionClause;
			if (def == null) throw new NullReferenceException(nameof(CreateTableQuery.DefinitionClause));

			return def.GetColumnNames();
		}
		return Enumerable.Empty<string>();
	}

	public bool TryGetColumnDefinition(string columnName, [MaybeNullWhen(false)] out ColumnDefinition column)
	{
		column = null;
		if (CreateTableQuery?.DefinitionClause == null) return false;

		var def = CreateTableQuery.DefinitionClause;

		column = def.OfType<ColumnDefinition>().Where(x => x.ColumnName == columnName).FirstOrDefault();
		return column is not null;
	}


	public DefinitionQuerySet GenerateMigrationQuery(string expectQuerySet)
	{
		var queryset = DefinitionQuerySetParser.Parse(expectQuerySet);
		return GenerateMigrationQuery(queryset);
	}

	public DefinitionQuerySet GenerateMigrationQuery(DefinitionQuerySet expectQuerySet)
	{
		var queryset = new DefinitionQuerySet();

		// Normalize for easier comparison
		var actual = Normalize();
		var expect = expectQuerySet.Normalize();
		if (expect.CreateTableQuery is null) throw new InvalidOperationException("create table query is missing.");
		if (actual.CreateTableQuery is null) throw new InvalidOperationException("create table query is missing.");

		var actualcolumns = actual.GetColumnNames().ToList();
		var expectcolumns = expect.GetColumnNames().ToList();

		//drop column
		foreach (var item in actualcolumns.Where(x => !expectcolumns.Contains(x)))
		{
			var clause = new AlterTableClause(expect.CreateTableQuery)
			{
				new DropColumnCommand(item)
			};
			queryset.AlterTableQueries.Add(new AlterTableQuery(clause));
		}

		//add column
		foreach (var item in expectcolumns.Where(x => !actualcolumns.Contains(x)))
		{
			if (expect.TryGetColumnDefinition(item, out var column))
			{
				var clause = new AlterTableClause(expect.CreateTableQuery)
				{
					new AddColumnCommand(column)
				};
				queryset.AlterTableQueries.Add(new AlterTableQuery(clause));
			}
		}

		//diff column
		foreach (var column in actualcolumns.Where(x => expectcolumns.Contains(x)))
		{
			if (actual.TryGetColumnDefinition(column, out var actualColumn) && expect.TryGetColumnDefinition(column, out var expectColumn))
			{
				//column type change
				if (actualColumn.ColumnType.ToText() != expectColumn.ColumnType.ToText())
				{
					var clause = new AlterTableClause(expect.CreateTableQuery)
					{
						new ChangeColumnTypeCommand(column, expectColumn.ColumnType)
					};
					queryset.AlterTableQueries.Add(new AlterTableQuery(clause));
				}

				//default value change
				if (actualColumn.DefaultValue != null && expectColumn.DefaultValue != null && actualColumn.DefaultValue.ToText() != expectColumn.DefaultValue.ToText())
				{
					//default value change(set)
					var clause = new AlterTableClause(expect.CreateTableQuery)
					{
						new SetDefaultCommand(column, expectColumn.DefaultValue)
					};
					queryset.AlterTableQueries.Add(new AlterTableQuery(clause));
				}
				else if (actualColumn.DefaultValue != null && expectColumn.DefaultValue == null)
				{
					//default value change(drop)
					var clause = new AlterTableClause(expect.CreateTableQuery)
					{
						new DropDefaultCommand(column)
					};
					queryset.AlterTableQueries.Add(new AlterTableQuery(clause));
				}
				else if (actualColumn.DefaultValue == null && expectColumn.DefaultValue != null)
				{
					//default value change(set)
					var clause = new AlterTableClause(expect.CreateTableQuery)
					{
						new SetDefaultCommand(column, expectColumn.DefaultValue)
					};
					queryset.AlterTableQueries.Add(new AlterTableQuery(clause));
				}

				// nullable
				if (actualColumn.IsNullable != expectColumn.IsNullable)
				{
					if (expectColumn.IsNullable)
					{
						var clause = new AlterTableClause(expect.CreateTableQuery)
						{
							new DropNotNullCommand(expectColumn.ColumnName)
						};
						queryset.AlterTableQueries.Add(new AlterTableQuery(clause));
					}
					else
					{
						var clause = new AlterTableClause(expect.CreateTableQuery)
						{
							new SetNotNullCommand(expectColumn.ColumnName)
						};
						queryset.AlterTableQueries.Add(new AlterTableQuery(clause));
					}
				}
			}
		}


		var actualCommands = GetAddConstraintCommands(actual).ToDictionary(x => x, x => x.ToText());
		var expectCommands = GetAddConstraintCommands(expect).ToDictionary(x => x, x => x.ToText());

		foreach (var item in actualCommands)
		{
			//drop
			if (!expectCommands.ContainsValue(item.Value) && !string.IsNullOrEmpty(item.Key.Constraint.ConstraintName))
			{
				var clause = new AlterTableClause(expect.CreateTableQuery)
				{
					new DropConstraintCommand(item.Key.Constraint.ConstraintName)
				};
				queryset.AlterTableQueries.Add(new AlterTableQuery(clause));
			}
		}

		foreach (var item in expectCommands)
		{
			//add
			if (!actualCommands.ContainsValue(item.Value))
			{
				var clause = new AlterTableClause(expect.CreateTableQuery)
				{
					item.Key
				};
				queryset.AlterTableQueries.Add(new AlterTableQuery(clause));
			}
		}

		return queryset;
	}

	private IEnumerable<AddConstraintCommand> GetAddConstraintCommands(DefinitionQuerySet queryset)
	{
		foreach (var q in queryset.AlterTableQueries)
		{
			foreach (var cmd in GetAddConstraintCommands(q))
			{
				yield return cmd;
			}
		}
	}

	private IEnumerable<AddConstraintCommand> GetAddConstraintCommands(AlterTableQuery query)
	{
		foreach (var cmd in query.AlterTableClause.OfType<AddConstraintCommand>())
		{
			yield return cmd;
		}
	}

	public string ToText()
	{
		var sb = ZString.CreateStringBuilder();

		if (CreateTableQuery != null)
		{
			sb.AppendLine(CreateTableQuery.ToText());
		}

		foreach (var item in AlterTableQueries)
		{
			if (sb.Length > 0) sb.AppendLine(";");
			sb.AppendLine(item.ToText());
		}
		foreach (var item in CreateIndexQueries)
		{
			if (sb.Length > 0) sb.AppendLine(";");
			sb.AppendLine(item.ToText());
		}

		return sb.ToString();
	}
}
