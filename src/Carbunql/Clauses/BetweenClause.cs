using Carbunql.Tables;
using Carbunql.Values;

namespace Carbunql.Clauses;

/// <summary>
/// Represents a BETWEEN clause in SQL.
/// </summary>
public class BetweenClause : ValueBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BetweenClause"/> class.
    /// </summary>
    public BetweenClause()
    {
        Value = null!;
        Start = null!;
        End = null!;
        IsNegative = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BetweenClause"/> class with the specified value, start, end, and negativity.
    /// </summary>
    /// <param name="value">The value to compare.</param>
    /// <param name="start">The start of the range.</param>
    /// <param name="end">The end of the range.</param>
    /// <param name="isNegative">Specifies whether the BETWEEN clause is negated.</param>
    public BetweenClause(ValueBase value, ValueBase start, ValueBase end, bool isNegative)
    {
        Value = value;
        Start = start;
        End = end;
        IsNegative = isNegative;
    }

    /// <summary>
    /// Gets or sets the value to compare.
    /// </summary>
    public ValueBase Value { get; init; }

    /// <summary>
    /// Gets or sets the start of the range.
    /// </summary>
    public ValueBase Start { get; init; }

    /// <summary>
    /// Gets or sets the end of the range.
    /// </summary>
    public ValueBase End { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the BETWEEN clause is negated.
    /// </summary>
    public bool IsNegative { get; init; }

    /// <summary>
    /// Gets the internal queries associated with this clause.
    /// </summary>
    protected override IEnumerable<SelectQuery> GetInternalQueriesCore()
    {
        foreach (var item in Value.GetInternalQueries())
        {
            yield return item;
        }
        foreach (var item in Start.GetInternalQueries())
        {
            yield return item;
        }
        foreach (var item in End.GetInternalQueries())
        {
            yield return item;
        }
    }

    /// <summary>
    /// Gets the parameters associated with this clause.
    /// </summary>
    protected override IEnumerable<QueryParameter> GetParametersCore()
    {
        foreach (var item in Value.GetParameters())
        {
            yield return item;
        }
        foreach (var item in Start.GetParameters())
        {
            yield return item;
        }
        foreach (var item in End.GetParameters())
        {
            yield return item;
        }
    }

    /// <summary>
    /// Gets the physical tables associated with this clause.
    /// </summary>
    protected override IEnumerable<PhysicalTable> GetPhysicalTablesCore()
    {
        foreach (var item in Value.GetPhysicalTables())
        {
            yield return item;
        }
        foreach (var item in Start.GetPhysicalTables())
        {
            yield return item;
        }
        foreach (var item in End.GetPhysicalTables())
        {
            yield return item;
        }
    }

    /// <summary>
    /// Gets the common tables associated with this clause.
    /// </summary>
    protected override IEnumerable<CommonTable> GetCommonTablesCore()
    {
        foreach (var item in Value.GetCommonTables())
        {
            yield return item;
        }
        foreach (var item in Start.GetCommonTables())
        {
            yield return item;
        }
        foreach (var item in End.GetCommonTables())
        {
            yield return item;
        }
    }

    /// <summary>
    /// Gets the tokens representing this clause.
    /// </summary>
    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        foreach (var item in Value.GetTokens(parent)) yield return item;

        if (IsNegative) yield return Token.Reserved(this, parent, "not");

        yield return Token.Reserved(this, parent, "between");
        foreach (var item in Start.GetTokens(parent)) yield return item;
        yield return Token.Reserved(this, parent, "and");
        foreach (var item in End.GetTokens(parent)) yield return item;
    }

    internal override IEnumerable<ColumnValue> GetColumnsCore()
    {
        foreach (var item in Value.GetColumns())
        {
            yield return item;
        }
        foreach (var item in Start.GetColumns())
        {
            yield return item;
        }
        foreach (var item in End.GetColumns())
        {
            yield return item;
        }
    }
}
