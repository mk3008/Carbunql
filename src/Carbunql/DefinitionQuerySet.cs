﻿using Carbunql.Clauses;
using Carbunql.Definitions;
using Cysharp.Text;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql;

/// <summary>
/// Represents a set of definition queries for a table.
/// </summary>
public class DefinitionQuerySet : ITable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefinitionQuerySet"/> class with the specified table name.
    /// </summary>
    /// <param name="table">The table name.</param>
    public DefinitionQuerySet(string table)
    {
        Table = table;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefinitionQuerySet"/> class with the specified schema and table names.
    /// </summary>
    /// <param name="schema">The schema name.</param>
    /// <param name="table">The table name.</param>
    public DefinitionQuerySet(string schema, string table)
    {
        Schema = schema;
        Table = table;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefinitionQuerySet"/> class with the specified table.
    /// </summary>
    /// <param name="t">The table.</param>
    public DefinitionQuerySet(ITable t)
    {
        Schema = t.Schema;
        Table = t.Table;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefinitionQuerySet"/> class with the specified drop table query.
    /// </summary>
    /// <param name="dropTableQuery">The drop table query.</param>
    public DefinitionQuerySet(DropTableQuery dropTableQuery)
    {
        Schema = dropTableQuery.Schema;
        Table = dropTableQuery.Table;
        DropTableQuery = dropTableQuery;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefinitionQuerySet"/> class with the specified create table query.
    /// </summary>
    /// <param name="createTableQuery">The create table query.</param>
    public DefinitionQuerySet(CreateTableQuery createTableQuery)
    {
        Schema = createTableQuery.Schema;
        Table = createTableQuery.Table;
        CreateTableQuery = createTableQuery;
    }

    /// <summary>
    /// Gets or sets the schema.
    /// </summary>
    public string Schema { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the table name.
    /// </summary>
    public string Table { get; init; }

    private DropTableQuery? _dropTableQuery = null;

    /// <summary>
    /// Gets or sets the drop table query.
    /// </summary>
    public DropTableQuery? DropTableQuery
    {
        get => _dropTableQuery;
        set
        {
            if (value == null)
            {
                _dropTableQuery = value;
                return;
            }
            if (this.GetTableFullName() != value.GetTableFullName())
            {
                throw new ArgumentException("Unable to add operations for different tables");
            }
            _dropTableQuery = value;
            _createTableQuery = null;
            _alterTableQueries.Clear();
            _alterIndexQueries.Clear();
        }
    }

    private CreateTableQuery? _createTableQuery = null;

    /// <summary>
    /// Gets or sets the create table query.
    /// </summary>
    public CreateTableQuery? CreateTableQuery
    {
        get => _createTableQuery;
        set
        {
            if (value == null)
            {
                _createTableQuery = value;
                return;
            }
            if (this.GetTableFullName() != value.GetTableFullName())
            {
                throw new ArgumentException("Unable to add operations for different tables");
            }
            _createTableQuery = value;
            _dropTableQuery = null;
        }
    }

    private List<AlterTableQuery> _alterTableQueries = new List<AlterTableQuery>();

    /// <summary>
    /// Gets the read-only list of alter table queries.
    /// </summary>
    public IReadOnlyList<AlterTableQuery> AlterTableQueries => _alterTableQueries.AsReadOnly();

    /// <summary>
    /// Adds an alter table query to the set.
    /// </summary>
    /// <param name="query">The alter table query to add.</param>
    public void AddAlterTableQuery(AlterTableQuery query)
    {
        if (DropTableQuery != null) throw new InvalidOperationException();

        if (this.GetTableFullName() != query.GetTableFullName())
        {
            throw new ArgumentException("Unable to add operations for different tables");
        }
        _alterTableQueries.Add(query);
    }

    private List<IAlterIndexQuery> _alterIndexQueries = new List<IAlterIndexQuery>();

    /// <summary>
    /// Gets the read-only list of alter index queries.
    /// </summary>
    public IReadOnlyList<IAlterIndexQuery> AlterIndexQueries => _alterIndexQueries.AsReadOnly();

    /// <summary>
    /// Adds an alter index query to the set.
    /// </summary>
    /// <param name="query">The alter index query to add.</param>
    public void AddAlterIndexQuery(IAlterIndexQuery query)
    {
        if (DropTableQuery != null) throw new InvalidOperationException();

        if (this.GetTableFullName() != query.GetTableFullName())
        {
            throw new ArgumentException("Unable to add operations for different tables");
        }
        _alterIndexQueries.Add(query);
    }
    /// <summary>
    /// Merges the alter table queries into a single alter table query.
    /// </summary>
    /// <returns>The current <see cref="DefinitionQuerySet"/> instance.</returns>
    public DefinitionQuerySet MergeAlterTableQuery()
    {
        var alt = new AlterTableQuery(new AlterTableClause(this));

        // Merge alter table query
        foreach (var alterquery in AlterTableQueries)
        {
            foreach (var command in alterquery.AlterTableClause)
            {
                alt.AlterTableClause.Add(command);
            }
        }

        if (alt.AlterTableClause.Any())
        {
            _alterTableQueries.Clear();
            _alterTableQueries.Add(alt);
        }

        return this;
    }

    /// <summary>
    /// Normalizes the <see cref="DefinitionQuerySet"/>.
    /// </summary>
    /// <param name="doMergeAltarTablerQuery">Specifies whether to merge alter table queries.</param>
    /// <returns>The normalized <see cref="DefinitionQuerySet"/>.</returns>
    public DefinitionQuerySet ToNormalize(bool doMergeAltarTablerQuery = true)
    {
        DefinitionQuerySet q;

        if (CreateTableQuery != null)
        {
            q = CreateTableQuery.ToNormalize();
        }
        else
        {
            q = new DefinitionQuerySet(this);
        }

        var clause = q.CreateTableQuery?.DefinitionClause;

        foreach (var atq in AlterTableQueries.SelectMany(x => x.Disassemble()))
        {
            if (clause != null && atq.TrySet(clause)) continue;
            q.AddAlterTableQuery(atq);
        }

        foreach (var aiq in AlterIndexQueries)
        {
            q.AddAlterIndexQuery(aiq);
        }

        if (doMergeAltarTablerQuery)
        {
            q.MergeAlterTableQuery();
        }

        return q;
    }

    /// <summary>
    /// Gets the column names from the table definition.
    /// </summary>
    /// <returns>The column names.</returns>
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

    /// <summary>
    /// Tries to get the column definition for the specified column name.
    /// </summary>
    /// <param name="columnName">The name of the column to get the definition for.</param>
    /// <param name="column">When this method returns, contains the column definition, if found; otherwise, <c>null</c>.</param>
    /// <returns><c>true</c> if the column definition was found; otherwise, <c>false</c>.</returns>
    public bool TryGetColumnDefinition(string columnName, [MaybeNullWhen(false)] out ColumnDefinition column)
    {
        column = null;
        if (CreateTableQuery?.DefinitionClause == null) return false;

        var def = CreateTableQuery.DefinitionClause;

        column = def.OfType<ColumnDefinition>().FirstOrDefault(x => x.ColumnName == columnName);
        return column != null;
    }

    /// <summary>
    /// Generates a migration query by comparing with an expected query set.
    /// </summary>
    /// <param name="expectedQuerySet">The expected query set to compare with.</param>
    /// <returns>The migration query set.</returns>
    public DefinitionQuerySet GenerateMigrationQuery(DefinitionQuerySet expectedQuerySet)
    {
        var queryset = new DefinitionQuerySet(this);

        // Normalize for easier comparison
        var actual = ToNormalize();
        var expect = expectedQuerySet.ToNormalize();
        if (expect.CreateTableQuery is null) throw new InvalidOperationException("create table query is missing.");
        if (actual.CreateTableQuery is null) throw new InvalidOperationException("create table query is missing.");

        var actualcolumns = actual.GetColumnNames().ToList();
        var expectcolumns = expect.GetColumnNames().ToList();

        // Drop column
        foreach (var item in actualcolumns.Where(x => !expectcolumns.Contains(x)))
        {
            var clause = new AlterTableClause(expect.CreateTableQuery)
        {
            new DropColumnCommand(this, item)
        };
            queryset.AddAlterTableQuery(new AlterTableQuery(clause));
        }

        // Add column
        foreach (var item in expectcolumns.Where(x => !actualcolumns.Contains(x)))
        {
            if (expect.TryGetColumnDefinition(item, out var column))
            {
                var clause = new AlterTableClause(expect.CreateTableQuery)
            {
                new AddColumnCommand(column)
            };
                queryset.AddAlterTableQuery(new AlterTableQuery(clause));
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
                        new ChangeColumnTypeCommand(this, column, expectColumn.ColumnType)
                    };
                    queryset.AddAlterTableQuery(new AlterTableQuery(clause));
                }

                //default value change
                if (actualColumn.DefaultValue != null && expectColumn.DefaultValue != null && actualColumn.DefaultValue.ToText() != expectColumn.DefaultValue.ToText())
                {
                    //default value change(set)
                    var clause = new AlterTableClause(expect.CreateTableQuery)
                    {
                        new SetDefaultCommand(this, column, expectColumn.DefaultValue)
                    };
                    queryset.AddAlterTableQuery(new AlterTableQuery(clause));
                }
                else if (actualColumn.DefaultValue != null && expectColumn.DefaultValue == null)
                {
                    //default value change(drop)
                    var clause = new AlterTableClause(expect.CreateTableQuery)
                    {
                        new DropDefaultCommand(this, column)
                    };
                    queryset.AddAlterTableQuery(new AlterTableQuery(clause));
                }
                else if (actualColumn.DefaultValue == null && expectColumn.DefaultValue != null)
                {
                    //default value change(set)
                    var clause = new AlterTableClause(expect.CreateTableQuery)
                    {
                        new SetDefaultCommand(this, column, expectColumn.DefaultValue)
                    };
                    queryset.AddAlterTableQuery(new AlterTableQuery(clause));
                }

                // nullable
                if (actualColumn.IsNullable != expectColumn.IsNullable)
                {
                    if (expectColumn.IsNullable)
                    {
                        var clause = new AlterTableClause(expect.CreateTableQuery)
                        {
                            new DropNotNullCommand(this, expectColumn.ColumnName)
                        };
                        queryset.AddAlterTableQuery(new AlterTableQuery(clause));
                    }
                    else
                    {
                        var clause = new AlterTableClause(expect.CreateTableQuery)
                        {
                            new SetNotNullCommand(this, expectColumn.ColumnName)
                        };
                        queryset.AddAlterTableQuery(new AlterTableQuery(clause));
                    }
                }
            }
        }

        // alter table query
        var actualCommands = GetAddConstraintCommands(actual).ToDictionary(x => x, x => x.ToText());
        var expectCommands = GetAddConstraintCommands(expect).ToDictionary(x => x, x => x.ToText());

        foreach (var item in actualCommands)
        {
            //drop
            if (!expectCommands.ContainsValue(item.Value) && !string.IsNullOrEmpty(item.Key.Constraint.ConstraintName))
            {
                var clause = new AlterTableClause(expect.CreateTableQuery)
                {
                    new DropConstraintCommand(this, item.Key.Constraint.ConstraintName)
                };
                queryset.AddAlterTableQuery(new AlterTableQuery(clause));
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
                queryset.AddAlterTableQuery(new AlterTableQuery(clause));
            }
        }

        //create index query
        var actualIndexes = actual.AlterIndexQueries.ToDictionary(x => x, x => x.ToText());
        var expectIndexes = expect.AlterIndexQueries.ToDictionary(x => x, x => x.ToText());

        foreach (var item in actualIndexes)
        {
            //drop
            if (!expectIndexes.ContainsValue(item.Value) && !string.IsNullOrEmpty(item.Key.IndexName))
            {
                queryset.AddAlterIndexQuery(new DropIndexQuery(this, item.Key.IndexName));
            }
        }

        foreach (var item in expectIndexes)
        {
            //add
            if (!actualIndexes.ContainsValue(item.Value))
            {
                queryset.AddAlterIndexQuery(item.Key);
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

    public string ToText(bool includeDropTableQuery = false)
    {
        var sb = ZString.CreateStringBuilder();

        var name = this.GetTableFullName();
        //sb.AppendLine("--" + name);
        if (DropTableQuery != null)
        {
            if (!includeDropTableQuery) return string.Empty;
            sb.AppendLine(DropTableQuery.ToText());
            sb.AppendLine(";");
            return sb.ToString();
        }

        if (CreateTableQuery != null)
        {
            sb.AppendLine(CreateTableQuery.ToText());
            sb.AppendLine(";");
        }

        foreach (var item in AlterTableQueries)
        {
            sb.AppendLine(item.ToText());
            sb.AppendLine(";");
        }
        foreach (var item in AlterIndexQueries)
        {
            sb.AppendLine(item.ToText());
            sb.AppendLine(";");
        }

        return sb.ToString();
    }
}
