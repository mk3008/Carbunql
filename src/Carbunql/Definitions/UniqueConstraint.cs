using Carbunql.Clauses;
using Carbunql.Tables;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Definitions;

/// <summary>
/// Represents a unique constraint.
/// </summary>
public class UniqueConstraint : IConstraint, ITable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UniqueConstraint"/> class with the specified table.
    /// </summary>
    public UniqueConstraint(ITable t)
    {
        Schema = t.Schema;
        Table = t.Table;
    }

    /// <summary>
    /// Gets or sets the name of the constraint.
    /// </summary>
    public string ConstraintName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of column names associated with the unique constraint.
    /// </summary>
    public List<string> ColumnNames { get; set; } = new();

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

        yield return new Token(this, parent, "unique", isReserved: true);
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
}
