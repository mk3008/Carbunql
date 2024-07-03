using Carbunql.Definitions;
using Carbunql.Tables;
using Carbunql.Values;

namespace Carbunql.Clauses;

/// <summary>
/// Represents a clause for altering a table structure.
/// </summary>
public class AlterTableClause : QueryCommandCollection<IAlterCommand>, IQueryCommandable, ITable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AlterTableClause"/> class with the specified table.
    /// </summary>
    /// <param name="t">The table to be altered.</param>
    public AlterTableClause(ITable t)
    {
        Schema = t.Schema;
        Table = t.Table;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AlterTableClause"/> class with the specified table and command.
    /// </summary>
    /// <param name="t">The table to be altered.</param>
    /// <param name="command">The alteration command.</param>
    public AlterTableClause(ITable t, IAlterCommand command)
    {
        Schema = t.Schema;
        Table = t.Table;
        Items.Add(command);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AlterTableClause"/> class with the specified table and constraint.
    /// </summary>
    /// <param name="t">The table to be altered.</param>
    /// <param name="constraint">The constraint to be added.</param>
    public AlterTableClause(ITable t, IConstraint constraint)
    {
        Schema = t.Schema;
        Table = t.Table;
        Items.Add(new AddConstraintCommand(constraint));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AlterTableClause"/> class with the specified schema and table.
    /// </summary>
    /// <param name="schema">The schema of the table to be altered.</param>
    /// <param name="table">The table to be altered.</param>
    public AlterTableClause(string schema, string table)
    {
        Schema = schema;
        Table = table;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AlterTableClause"/> class with the specified table name.
    /// </summary>
    /// <param name="table">The table to be altered.</param>
    public AlterTableClause(string table)
    {
        Schema = string.Empty;
        Table = table;
    }

    /// <summary>
    /// Gets or sets the schema of the table to be altered.
    /// </summary>
    public string Schema { get; init; }

    /// <summary>
    /// Gets or sets the name of the table to be altered.
    /// </summary>
    public string Table { get; init; }

    /// <summary>
    /// Gets the full name of the table to be altered.
    /// </summary>
    public string TableFullName => (string.IsNullOrEmpty(Schema)) ? Table : Schema + "." + Table;

    /// <summary>
    /// Gets the internal queries associated with this clause.
    /// </summary>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        yield break;
    }

    /// <summary>
    /// Gets the physical tables associated with this clause.
    /// </summary>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        yield break;
    }

    /// <summary>
    /// Gets the tokens representing this clause.
    /// </summary>
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

    /// <summary>
    /// Gets the common tables associated with this clause.
    /// </summary>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        yield break;
    }

    public IEnumerable<ColumnValue> GetColumns()
    {
        yield break;
    }
}
