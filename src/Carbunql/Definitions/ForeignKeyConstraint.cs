using Carbunql.Clauses;
using Carbunql.Tables;
using Carbunql.Values;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Definitions;

/// <summary>
/// Represents a foreign key constraint.
/// </summary>
internal class ForeignKeyConstraint : IConstraint
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForeignKeyConstraint"/> class with the specified table.
    /// </summary>
    /// <param name="t">The table to which the foreign key constraint belongs.</param>
    public ForeignKeyConstraint(ITable t)
    {
        Schema = t.Schema;
        Table = t.Table;
    }

    /// <summary>
    /// Gets or sets the name of the foreign key constraint.
    /// </summary>
    public string ConstraintName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of column names associated with the foreign key constraint.
    /// </summary>
    public List<string> ColumnNames { get; set; } = new();

    /// <summary>
    /// Gets or sets the reference definition for the foreign key constraint.
    /// </summary>
    public ReferenceDefinition Reference { get; set; } = null!;

    /// <summary>
    /// Gets the column name (always empty for foreign key constraints).
    /// </summary>
    public string ColumnName => string.Empty;

    /// <summary>
    /// Gets or sets the schema of the table to which the foreign key constraint belongs.
    /// </summary>
    public string Schema { get; init; }

    /// <summary>
    /// Gets or sets the name of the table to which the foreign key constraint belongs.
    /// </summary>
    public string Table { get; init; }

    /// <summary>
    /// Gets the common tables associated with the foreign key constraint (currently empty).
    /// </summary>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        yield break;
    }

    /// <summary>
    /// Gets the internal queries associated with the foreign key constraint (currently empty).
    /// </summary>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        yield break;
    }

    /// <summary>
    /// Gets the parameters associated with the foreign key constraint (currently empty).
    /// </summary>
    public IEnumerable<QueryParameter> GetParameters()
    {
        yield break;
    }

    /// <summary>
    /// Gets the physical tables associated with the foreign key constraint (currently empty).
    /// </summary>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        yield break;
    }

    /// <summary>
    /// Gets the tokens representing the foreign key constraint.
    /// </summary>
    /// <param name="parent">The parent token.</param>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        if (!string.IsNullOrEmpty(ConstraintName))
        {
            yield return new Token(this, parent, "constraint", isReserved: true);
            yield return new Token(this, parent, ConstraintName);
        }

        yield return new Token(this, parent, "foreign key", isReserved: true);
        yield return Token.ExpressionBracketStart(this, parent);
        foreach (var item in ColumnNames)
        {
            yield return new Token(this, parent, item);
        }
        yield return Token.ExpressionBracketEnd(this, parent);

        foreach (var item in Reference.GetTokens(parent))
        {
            yield return item;
        }
    }

    /// <summary>
    /// Attempts to apply the foreign key constraint to a table definition clause (always returns <c>false</c>).
    /// </summary>
    /// <param name="clause">The table definition clause to which the constraint will be applied.</param>
    /// <returns><c>false</c> indicating that the constraint was not applied.</returns>
    public bool TrySet(TableDefinitionClause clause)
    {
        return false;
    }

    /// <summary>
    /// Tries to convert the foreign key constraint to another constraint (always returns <c>true</c>).
    /// </summary>
    /// <param name="constraint">When this method returns, contains the converted constraint.</param>
    /// <returns><c>true</c> indicating that the conversion succeeded.</returns>
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
