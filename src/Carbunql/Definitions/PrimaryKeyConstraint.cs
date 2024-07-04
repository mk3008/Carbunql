using Carbunql.Clauses;
using Carbunql.Tables;
using Carbunql.Values;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Definitions;

/// <summary>
/// Represents a primary key constraint.
/// </summary>
public class PrimaryKeyConstraint : IConstraint
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PrimaryKeyConstraint"/> class with the specified schema, table, and column names.
    /// </summary>
    public PrimaryKeyConstraint(string schema, string table, IEnumerable<string> columns)
    {
        Schema = schema;
        Table = table;
        PrimaryKeyMaps = columns.Select(x => new PrimaryKeyMap(x, string.Empty)).ToList();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PrimaryKeyConstraint"/> class with the specified schema, table, and primary key maps.
    /// </summary>
    public PrimaryKeyConstraint(string schema, string table, IEnumerable<PrimaryKeyMap> maps)
    {
        Schema = schema;
        Table = table;
        PrimaryKeyMaps = maps.ToList();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PrimaryKeyConstraint"/> class with the specified table and column names.
    /// </summary>
    public PrimaryKeyConstraint(ITable t, IEnumerable<string> columns)
    {
        Schema = t.Schema;
        Table = t.Table;
        PrimaryKeyMaps = columns.Select(x => new PrimaryKeyMap(x, string.Empty)).ToList();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PrimaryKeyConstraint"/> class with the specified table and primary key maps.
    /// </summary>
    public PrimaryKeyConstraint(ITable t, IEnumerable<PrimaryKeyMap> maps)
    {
        Schema = t.Schema;
        Table = t.Table;
        PrimaryKeyMaps = maps.ToList();
    }

    /// <summary>
    /// Gets or sets the name of the constraint.
    /// </summary>
    public string ConstraintName { get; set; } = string.Empty;

    /// <summary>
    /// Gets the list of primary key maps.
    /// </summary>
    public List<PrimaryKeyMap> PrimaryKeyMaps { get; } = new();

    /// <summary>
    /// Gets the names of the columns associated with the primary key constraint.
    /// </summary>
    public IEnumerable<string> ColumnNames => PrimaryKeyMaps.Select(x => x.ColumnName);

    /// <summary>
    /// Gets the name of the column.
    /// </summary>
    public string ColumnName => string.Empty;

    /// <summary>
    /// Gets or sets the schema of the table.
    /// </summary>
    public string Schema { get; init; }

    /// <summary>
    /// Gets or sets the name of the table.
    /// </summary>
    public string Table { get; init; }

    /// <summary>
    /// Gets the common tables associated with the constraint.
    /// </summary>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        yield break;
    }

    /// <summary>
    /// Gets the internal queries associated with the constraint.
    /// </summary>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        yield break;
    }

    /// <summary>
    /// Gets the parameters associated with the constraint.
    /// </summary>
    public IEnumerable<QueryParameter> GetParameters()
    {
        yield break;
    }

    /// <summary>
    /// Gets the physical tables associated with the constraint.
    /// </summary>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        yield break;
    }

    /// <summary>
    /// Generates tokens for the constraint.
    /// </summary>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        if (!string.IsNullOrEmpty(ConstraintName))
        {
            yield return new Token(this, parent, "constraint", isReserved: true);
            yield return new Token(this, parent, ConstraintName);
        }

        yield return new Token(this, parent, "primary key", isReserved: true);

        yield return Token.ReservedBracketStart(this, parent);
        foreach (var item in ColumnNames)
        {
            yield return new Token(this, parent, item);
        }
        yield return Token.ReservedBracketEnd(this, parent);
    }

    /// <summary>
    /// Attempts to apply the constraint to a table definition clause.
    /// </summary>
    public bool TrySet(TableDefinitionClause clause)
    {
        return false;
    }

    /// <summary>
    /// Attempts to disassemble the constraint.
    /// </summary>
    public bool TryDisassemble([MaybeNullWhen(false)] out IConstraint constraint)
    {
        constraint = this;
        return true;
    }

    public IEnumerable<ColumnValue> GetColumns()
    {
        yield break;
    }
}
