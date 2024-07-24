using Carbunql.Clauses;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Values;

/// <summary>
/// Represents a case expression.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class CaseExpression : ValueBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CaseExpression"/> class.
    /// </summary>
    public CaseExpression()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CaseExpression"/> class with the specified condition.
    /// </summary>
    /// <param name="condition">The condition of the case expression.</param>
    public CaseExpression(ValueBase condition)
    {
        CaseCondition = condition;
    }

    /// <summary>
    /// Gets or sets the condition of the case expression.
    /// </summary>
    public ValueBase? CaseCondition { get; init; }

    /// <summary>
    /// Gets or sets the list of when expressions.
    /// </summary>
    public List<WhenExpression> WhenExpressions { get; init; } = new();

    /// <inheritdoc/>
    protected override IEnumerable<SelectQuery> GetInternalQueriesCore()
    {
        if (CaseCondition != null)
        {
            foreach (var item in CaseCondition.GetInternalQueries())
            {
                yield return item;
            }
        }
        foreach (var exp in WhenExpressions)
        {
            foreach (var item in exp.GetInternalQueries())
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    protected override IEnumerable<PhysicalTable> GetPhysicalTablesCore()
    {
        if (CaseCondition != null)
        {
            foreach (var item in CaseCondition.GetPhysicalTables())
            {
                yield return item;
            }
        }
        foreach (var exp in WhenExpressions)
        {
            foreach (var item in exp.GetPhysicalTables())
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    protected override IEnumerable<CommonTable> GetCommonTablesCore()
    {
        if (CaseCondition != null)
        {
            foreach (var item in CaseCondition.GetCommonTables())
            {
                yield return item;
            }
        }
        foreach (var exp in WhenExpressions)
        {
            foreach (var item in exp.GetCommonTables())
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        var current = Token.Reserved(this, parent, "case");

        yield return current;
        if (CaseCondition != null) foreach (var item in CaseCondition.GetTokens(current)) yield return item;

        foreach (var item in WhenExpressions)
        {
            foreach (var token in item.GetTokens(current)) yield return token;
        }

        yield return Token.Reserved(this, parent, "end");
    }

    /// <inheritdoc/>
    protected override IEnumerable<QueryParameter> GetParametersCore()
    {
        if (CaseCondition != null)
        {
            foreach (var item in CaseCondition.GetParameters())
            {
                yield return item;
            }
        }
        foreach (var item in WhenExpressions)
        {
            foreach (var p in item.GetParameters())
            {
                yield return p;
            }
        }
    }

    internal override IEnumerable<ColumnValue> GetColumnsCore()
    {
        if (CaseCondition != null)
        {
            foreach (var item in CaseCondition.GetColumns())
            {
                yield return item;
            }
        }
        foreach (var exp in WhenExpressions)
        {
            foreach (var item in exp.GetColumns())
            {
                yield return item;
            }
        }
    }

    public override IEnumerable<ValueBase> GetValues()
    {
        yield return this;

        if (CaseCondition != null)
        {
            foreach (var item in CaseCondition.GetValues())
            {
                yield return item;
            }
        }
        foreach (var exp in WhenExpressions)
        {
            foreach (var item in exp.GetValues())
            {
                yield return item;
            }
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
