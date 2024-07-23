using Carbunql.Clauses;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Values;

/// <summary>
/// Represents a negative value in a query.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class NegativeValue : ValueBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NegativeValue"/> class with a null inner value.
    /// </summary>
    public NegativeValue()
    {
        Inner = null!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NegativeValue"/> class with the specified inner value.
    /// </summary>
    /// <param name="inner">The inner value to be negated.</param>
    public NegativeValue(ValueBase inner)
    {
        Inner = inner;
    }

    /// <summary>
    /// Gets or sets the inner value to be negated.
    /// </summary>
    public ValueBase Inner { get; init; }

    public override IEnumerable<ValueBase> GetValues()
    {
        yield return this;

        foreach (var item in Inner.GetValues())
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

    /// <inheritdoc/>
    protected override IEnumerable<SelectQuery> GetInternalQueriesCore()
    {
        foreach (var item in Inner.GetInternalQueries())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        yield return Token.Reserved(this, parent, "not");
        foreach (var item in Inner.GetTokens(parent)) yield return item;
    }

    /// <inheritdoc/>
    protected override IEnumerable<QueryParameter> GetParametersCore()
    {
        return Inner.GetParameters();
    }

    /// <inheritdoc/>
    protected override IEnumerable<PhysicalTable> GetPhysicalTablesCore()
    {
        foreach (var item in Inner.GetPhysicalTables())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    protected override IEnumerable<CommonTable> GetCommonTablesCore()
    {
        foreach (var item in Inner.GetCommonTables())
        {
            yield return item;
        }
    }

    internal override IEnumerable<ColumnValue> GetColumnsCore()
    {
        foreach (var item in Inner.GetColumns())
        {
            yield return item;
        }
    }
}
