using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Values;

/// <summary>
/// Represents a literal value in a query.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class LiteralValue : ValueBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LiteralValue"/> class with a default command text of "null".
    /// </summary>
    public LiteralValue()
    {
        CommandText = "null";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LiteralValue"/> class with the specified command text.
    /// If the provided command text is null, sets the command text to "null".
    /// </summary>
    /// <param name="commandText">The command text representing the literal value.</param>
    public LiteralValue(string? commandText)
    {
        CommandText = commandText ?? "null";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LiteralValue"/> class with the specified integer value.
    /// If the provided value is null, sets the command text to "null".
    /// </summary>
    /// <param name="value">The integer value.</param>
    public LiteralValue(int? value)
    {
        CommandText = value?.ToString() ?? "null";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LiteralValue"/> class with the specified boolean value.
    /// If the provided value is null, sets the command text to "null".
    /// </summary>
    /// <param name="value">The boolean value.</param>
    public LiteralValue(bool? value)
    {
        CommandText = value?.ToString() ?? "null";
    }

    /// <summary>
    /// Gets or sets the command text representing the literal value.
    /// </summary>
    public string CommandText { get; set; }

    /// <summary>
    /// Gets a value indicating whether the literal value is null.
    /// </summary>
    public bool IsNullValue => CommandText.IsEqualNoCase("null");

    /// <inheritdoc/>
    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        yield return new Token(this, parent, CommandText);
    }

    /// <inheritdoc/>
    protected override IEnumerable<CommonTable> GetCommonTablesCore()
    {
        yield break;
    }

    /// <inheritdoc/>
    protected override IEnumerable<SelectQuery> GetInternalQueriesCore()
    {
        yield break;
    }

    /// <inheritdoc/>
    protected override IEnumerable<QueryParameter> GetParametersCore()
    {
        yield break;
    }

    /// <inheritdoc/>
    protected override IEnumerable<PhysicalTable> GetPhysicalTablesCore()
    {
        yield break;
    }

    internal override IEnumerable<ColumnValue> GetColumnsCore()
    {
        yield break;
    }
}
