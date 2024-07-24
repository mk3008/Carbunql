using Carbunql.Clauses;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Values;

/// <summary>
/// Represents a WHEN expression.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class WhenExpression : IQueryCommandable, IColumnContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WhenExpression"/> class with a condition and a value.
    /// </summary>
    /// <param name="condition">The condition for the WHEN expression.</param>
    /// <param name="value">The value associated with the condition.</param>
    public WhenExpression(ValueBase condition, ValueBase value)
    {
        Condition = condition;
        Value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WhenExpression"/> class with only a value.
    /// </summary>
    /// <param name="value">The value associated with the WHEN expression.</param>
    public WhenExpression(ValueBase value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets or sets the condition for the WHEN expression.
    /// </summary>
    public ValueBase? Condition { get; init; }

    /// <summary>
    /// Gets or sets the value associated with the WHEN expression.
    /// </summary>
    public ValueBase Value { get; private set; }

    /// <summary>
    /// Sets the value of the WHEN expression.
    /// </summary>
    /// <param name="value">The value to set.</param>
    public void SetValue(ValueBase value)
    {
        Value = value;
    }

    /// <inheritdoc/>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        if (Condition != null)
        {
            foreach (var item in Condition.GetInternalQueries())
            {
                yield return item;
            }
        }
        foreach (var item in Value.GetInternalQueries())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        if (Condition != null)
        {
            yield return Token.Reserved(this, parent, "when");
            foreach (var item in Condition.GetTokens(parent)) yield return item;
            yield return Token.Reserved(this, parent, "then");
            foreach (var item in Value.GetTokens(parent)) yield return item;
        }
        else
        {
            yield return Token.Reserved(this, parent, "else");
            foreach (var item in Value.GetTokens(parent)) yield return item;
        }
    }

    /// <inheritdoc/>
    public IEnumerable<QueryParameter> GetParameters()
    {
        if (Condition != null)
        {
            foreach (var item in Condition.GetParameters())
            {
                yield return item;
            }
        }
        foreach (var item in Value.GetParameters())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        if (Condition != null)
        {
            foreach (var item in Condition.GetPhysicalTables())
            {
                yield return item;
            }
        }
        foreach (var item in Value.GetPhysicalTables())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        if (Condition != null)
        {
            foreach (var item in Condition.GetCommonTables())
            {
                yield return item;
            }
        }
        foreach (var item in Value.GetCommonTables())
        {
            yield return item;
        }
    }

    public IEnumerable<ColumnValue> GetColumns()
    {
        if (Condition != null)
        {
            foreach (var item in Condition.GetColumns())
            {
                yield return item;
            }
        }
        foreach (var item in Value.GetColumns())
        {
            yield return item;
        }
    }

    public IEnumerable<ValueBase> GetValues()
    {
        if (Condition != null)
        {
            foreach (var item in Condition.GetValues())
            {
                yield return item;
            }
        }
        foreach (var item in Value.GetValues())
        {
            yield return item;
        }
    }
}
