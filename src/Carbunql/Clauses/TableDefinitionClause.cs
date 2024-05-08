using Carbunql.Definitions;
using Carbunql.Tables;

namespace Carbunql.Clauses;

/// <summary>
/// Represents a collection of queries containing table definitions.
/// </summary>
public class TableDefinitionClause : QueryCommandCollection<ITableDefinition>, ITable
{
    /// <summary>
    /// Gets or sets the schema name of the table.
    /// </summary>
    public string Schema { get; init; }

    /// <summary>
    /// Gets or sets the table name.
    /// </summary>
    public string Table { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TableDefinitionClause"/> class with a table instance.
    /// </summary>
    /// <param name="t">The table instance.</param>
    public TableDefinitionClause(ITable t)
    {
        Schema = t.Schema;
        Table = t.Table;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TableDefinitionClause"/> class with the specified schema and table name.
    /// </summary>
    /// <param name="schema">The schema name of the table.</param>
    /// <param name="table">The table name.</param>
    public TableDefinitionClause(string schema, string table)
    {
        Schema = schema;
        Table = table;
    }

    /// <summary>
    /// Gets the tokens of the collection.
    /// </summary>
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

    /// <summary>
    /// Gets the internal queries.
    /// </summary>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        yield break;
    }

    /// <summary>
    /// Gets the physical tables.
    /// </summary>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        yield break;
    }

    /// <summary>
    /// Gets the common tables.
    /// </summary>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        yield break;
    }

    /// <summary>
    /// Gets the parameters.
    /// </summary>
    public override IEnumerable<QueryParameter> GetParameters()
    {
        yield break;
    }

    /// <summary>
    /// Gets the column names.
    /// </summary>
    public IEnumerable<string> GetColumnNames()
    {
        var lst = new List<string>();
        foreach (var item in Items.Where(x => !string.IsNullOrEmpty(x.ColumnName)))
        {
            lst.Add(item.ColumnName);
        }
        return lst.Distinct();
    }

    /// <summary>
    /// Normalizes the table.
    /// </summary>
    public TableDefinitionClause ToNormalize()
    {
        var clause = new TableDefinitionClause(this);
        foreach (var item in Items.OfType<ColumnDefinition>())
        {
            if (item.TryNormalize(out var column))
            {
                clause.Add(column);
            }
        }
        return clause;
    }

    /// <summary>
    /// Disassembles the table.
    /// </summary>
    public List<AlterTableQuery> Disassemble()
    {
        var lst = new List<AlterTableQuery>();

        // Normalize unknown name primary keys.
        var pkeys = Items.OfType<ColumnDefinition>().Where(x => x.IsPrimaryKey).Select(x => x.ColumnName).Distinct();
        if (pkeys.Any())
        {
            var c = new PrimaryKeyConstraint(this, pkeys);
            lst.Add(new AlterTableQuery(new AlterTableClause(this, c)));
        }

        // Normalize unknown name unique keys.
        var ukeys = Items.OfType<ColumnDefinition>().Where(x => x.IsUniqueKey).Select(x => x.ColumnName).Distinct();
        if (ukeys.Any())
        {
            var c = new UniqueConstraint(this) { ColumnNames = ukeys.ToList() };
            lst.Add(new AlterTableQuery(new AlterTableClause(this, c)));
        }

        //disassemble
        foreach (var def in Items)
        {
            if (def.TryDisasseble(out var constraint))
            {
                lst.Add(new AlterTableQuery(new AlterTableClause(this, constraint)));
            }
        }

        return lst;
    }
}
