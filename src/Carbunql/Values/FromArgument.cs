﻿using Carbunql.Clauses;
using Carbunql.Tables;

namespace Carbunql.Values;

/// <summary>
/// Represents a FROM argument.
/// </summary>
public class FromArgument : ValueBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FromArgument"/> class.
    /// </summary>
    public FromArgument()
    {
        Unit = null!;
        Value = null!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FromArgument"/> class with the specified unit and value.
    /// </summary>
    /// <param name="unit">The unit value.</param>
    /// <param name="value">The value.</param>
    public FromArgument(string unit, ValueBase value)
    {
        Unit = unit;
        Value = value;
    }

    /// <summary>
    /// Gets or sets the unit value.
    /// </summary>
    public string Unit { get; init; }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public ValueBase Value { get; init; }

    /// <inheritdoc/>
    protected override IEnumerable<SelectQuery> GetInternalQueriesCore()
    {
        foreach (var item in Value.GetInternalQueries())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        if (string.IsNullOrEmpty(Unit)) throw new InvalidProgramException();

        yield return Token.Reserved(this, parent, Unit);
        yield return Token.Reserved(this, parent, "from");
        foreach (var item in Value.GetTokens(parent)) yield return item;
    }

    /// <inheritdoc/>
    protected override IEnumerable<QueryParameter> GetParametersCore()
    {
        foreach (var item in Value.GetParameters())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    protected override IEnumerable<PhysicalTable> GetPhysicalTablesCore()
    {
        foreach (var item in Value.GetPhysicalTables())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    protected override IEnumerable<CommonTable> GetCommonTablesCore()
    {
        foreach (var item in Value.GetCommonTables())
        {
            yield return item;
        }
    }

    internal override IEnumerable<ColumnValue> GetColumnsCore()
    {
        foreach (var item in Value.GetColumns())
        {
            yield return item;
        }
    }

    public override IEnumerable<ValueBase> GetValues()
    {
        yield return this;

        foreach (var item in Value.GetValues())
        {
            yield return item;
        }

        if (OperatableValue != null)
        {
            foreach (var item in OperatableValue.Value.GetValues())
            {
                yield return item;
            }
        }
    }
}
