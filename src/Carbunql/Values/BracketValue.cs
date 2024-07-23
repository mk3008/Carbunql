using Carbunql.Clauses;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Values;

/// <summary>
/// Represents a bracketed value.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class BracketValue : ValueBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BracketValue"/> class.
    /// </summary>
    public BracketValue()
    {
        Inner = null!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BracketValue"/> class with the specified inner value.
    /// </summary>
    /// <param name="inner">The inner value.</param>
    public BracketValue(ValueBase inner)
    {
        Inner = inner;
    }

    /// <summary>
    /// Gets or sets the inner value.
    /// </summary>
    public ValueBase Inner { get; init; }

    /// <inheritdoc/>
    protected override IEnumerable<SelectQuery> GetInternalQueriesCore()
    {
        foreach (var item in Inner.GetInternalQueries())
        {
            yield return item;
        }
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

    /// <inheritdoc/>
    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        if (Inner == null) yield break;

        var bracket = Token.ExpressionBracketStart(this, parent);
        yield return bracket;
        foreach (var item in Inner.GetTokens(bracket)) yield return item;
        yield return Token.ExpressionBracketEnd(this, parent);
    }

    internal override IEnumerable<ColumnValue> GetColumnsCore()
    {
        foreach (var item in Inner.GetColumns())
        {
            yield return item;
        }
    }

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
}
