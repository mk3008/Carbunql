using Carbunql.Clauses;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Values;

/// <summary>
/// Represents an operatable value in a query.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class OperatableValue : IQueryCommandable, IColumnContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OperatableValue"/> class with the specified operator and value.
    /// </summary>
    /// <param name="operator">The operator to apply to the value.</param>
    /// <param name="value">The value to which the operator is applied.</param>
    public OperatableValue(string @operator, ValueBase value)
    {
        Operator = @operator;
        Value = value;
    }

    /// <summary>
    /// Gets or sets the operator to apply to the value.
    /// </summary>
    public string Operator { get; init; }

    /// <summary>
    /// Gets or sets the value to which the operator is applied.
    /// </summary>
    public ValueBase Value { get; init; }

    /// <inheritdoc/>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        foreach (var item in Value.GetCommonTables())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        foreach (var item in Value.GetInternalQueries())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    public IEnumerable<QueryParameter> GetParameters()
    {
        return Value.GetParameters();
    }

    /// <inheritdoc/>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        foreach (var item in Value.GetPhysicalTables())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        if (!string.IsNullOrEmpty(Operator))
        {
            yield return Token.Reserved(this, parent, Operator);
        }
        foreach (var item in Value.GetTokens(parent)) yield return item;
    }

    public IEnumerable<ColumnValue> GetColumns()
    {
        foreach (var item in Value.GetColumns())
        {
            yield return item;
        }
    }
}
