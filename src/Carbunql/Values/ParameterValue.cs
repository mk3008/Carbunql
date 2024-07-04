using Carbunql.Clauses;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Values;

/// <summary>
/// Represents a parameterized value in a query.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class ParameterValue : ValueBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterValue"/> class.
    /// </summary>
    public ParameterValue()
    {
        Key = null!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterValue"/> class with the specified key.
    /// </summary>
    /// <param name="key">The key of the parameterized value.</param>
    public ParameterValue(string key)
    {
        Key = key;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterValue"/> class with the specified key and value.
    /// </summary>
    /// <param name="key">The key of the parameterized value.</param>
    /// <param name="value">The value of the parameter.</param>
    public ParameterValue(string key, object? value)
    {
        Key = key;
        Value = value;
        Parameters.Add(new QueryParameter(key, value));
    }

    /// <summary>
    /// Gets or sets the key of the parameterized value.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Gets or sets the value of the parameter.
    /// </summary>
    public object? Value { get; set; } = null;

    /// <summary>
    /// Gets or sets the list of query parameters associated with the parameterized value.
    /// </summary>
    public List<QueryParameter> Parameters { get; set; } = new();

    /// <inheritdoc/>
    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        yield return new Token(this, parent, Key);
    }

    /// <inheritdoc/>
    protected override IEnumerable<QueryParameter> GetParametersCore()
    {
        return Parameters;
    }

    /// <inheritdoc/>
    protected override IEnumerable<SelectQuery> GetInternalQueriesCore()
    {
        yield break;
    }

    /// <inheritdoc/>
    protected override IEnumerable<PhysicalTable> GetPhysicalTablesCore()
    {
        yield break;
    }

    /// <inheritdoc/>
    protected override IEnumerable<CommonTable> GetCommonTablesCore()
    {
        yield break;
    }

    internal override IEnumerable<ColumnValue> GetColumnsCore()
    {
        yield break;
    }
}
