using Carbunql.Clauses;
using Carbunql.Tables;
namespace Carbunql.Values;

/// <summary>
/// Represents an "AS" argument value.
/// </summary>
public class AsArgument : ValueBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AsArgument"/> class.
    /// </summary>
    public AsArgument()
    {
        Value = null!;
        Type = null!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsArgument"/> class with the specified value and type.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="type">The type.</param>
    public AsArgument(ValueBase value, ValueBase type)
    {
        Value = value;
        Type = type;
    }

    /// <summary>
    /// Gets or sets the value of the "AS" argument.
    /// </summary>
    public ValueBase Value { get; init; }

    /// <summary>
    /// Gets or sets the type of the "AS" argument.
    /// </summary>
    public ValueBase Type { get; init; }

    /// <inheritdoc/>
    protected override IEnumerable<SelectQuery> GetInternalQueriesCore()
    {
        foreach (var item in Value.GetInternalQueries())
        {
            yield return item;
        }
        foreach (var item in Type.GetInternalQueries())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        foreach (var item in Value.GetTokens(parent)) yield return item;
        yield return Token.Reserved(this, parent, "as");
        foreach (var item in Type.GetTokens(parent)) yield return item;
    }

    /// <inheritdoc/>
    protected override IEnumerable<PhysicalTable> GetPhysicalTablesCore()
    {
        foreach (var item in Value.GetPhysicalTables())
        {
            yield return item;
        }
        foreach (var item in Type.GetPhysicalTables())
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
        foreach (var item in Type.GetCommonTables())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    protected override IEnumerable<QueryParameter> GetParametersCore()
    {
        foreach (var item in Value.GetParameters())
        {
            yield return item;
        }
        foreach (var item in Type.GetParameters())
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

        foreach (var item in Type.GetValues())
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
