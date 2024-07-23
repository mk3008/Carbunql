using Carbunql.Clauses;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Values;

/// <summary>
/// Represents an array value.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class ArrayValue : ValueBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArrayValue"/> class.
    /// </summary>
    public ArrayValue()
    {
        Argument = null!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArrayValue"/> class with the specified argument.
    /// </summary>
    /// <param name="arg">The argument.</param>
    public ArrayValue(ValueBase arg)
    {
        Argument = arg;
    }

    /// <inheritdoc/>
    public string Name => "array";

    /// <summary>
    /// Gets or sets the argument of the array value.
    /// </summary>
    public ValueBase Argument { get; set; }

    /// <inheritdoc/>
    protected override IEnumerable<SelectQuery> GetInternalQueriesCore()
    {
        foreach (var item in Argument.GetInternalQueries())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        yield return Token.Reserved(this, parent, Name);

        if (Argument is ValueCollection) yield return Token.Reserved(this, parent, "[");
        foreach (var item in Argument.GetTokens(parent)) yield return item;
        if (Argument is ValueCollection) yield return Token.Reserved(this, parent, "]");
    }

    /// <inheritdoc/>
    protected override IEnumerable<QueryParameter> GetParametersCore()
    {
        return Argument.GetParameters();
    }

    /// <inheritdoc/>
    protected override IEnumerable<PhysicalTable> GetPhysicalTablesCore()
    {
        foreach (var item in Argument.GetPhysicalTables())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    protected override IEnumerable<CommonTable> GetCommonTablesCore()
    {
        foreach (var item in Argument.GetCommonTables())
        {
            yield return item;
        }
    }

    internal override IEnumerable<ColumnValue> GetColumnsCore()
    {
        foreach (var item in Argument.GetColumns())
        {
            yield return item;
        }
    }

    public override IEnumerable<ValueBase> GetValues()
    {
        yield return this;

        foreach (var item in Argument.GetValues())
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
