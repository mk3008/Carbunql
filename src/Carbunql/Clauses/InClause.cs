using Carbunql.Tables;
using Carbunql.Values;
using MessagePack;

namespace Carbunql.Clauses;

/// <summary>
/// Represents an IN clause in a SQL query.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class InClause : ValueBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InClause"/> class.
    /// </summary>
    public InClause()
    {
        Value = null!;
        Argument = null!;
        IsNegative = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InClause"/> class with the specified value and argument.
    /// </summary>
    /// <param name="value">The value of the IN clause.</param>
    /// <param name="argument">The argument of the IN clause.</param>
    public InClause(ValueBase value, ValueBase argument)
    {
        Value = value;
        Argument = (argument is BracketValue || argument is QueryContainer) ? argument : new BracketValue(argument);
        IsNegative = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InClause"/> class with the specified value, argument, and negativity indicator.
    /// </summary>
    /// <param name="value">The value of the IN clause.</param>
    /// <param name="argument">The argument of the IN clause.</param>
    /// <param name="isNegative">Indicates whether the IN clause is negative.</param>
    public InClause(ValueBase value, ValueBase argument, bool isNegative)
    {
        Value = value;
        Argument = (argument is BracketValue || argument is QueryContainer) ? argument : new BracketValue(argument);
        IsNegative = isNegative;
    }

    /// <summary>
    /// Gets the value of the IN clause.
    /// </summary>
    public ValueBase Value { get; init; }

    /// <summary>
    /// Gets the argument of the IN clause.
    /// </summary>
    public ValueBase Argument { get; init; }

    /// <summary>
    /// Indicates whether the IN clause is negative.
    /// </summary>
    public bool IsNegative { get; set; }

    /// <summary>
    /// Gets the internal queries associated with this IN clause.
    /// </summary>
    protected override IEnumerable<SelectQuery> GetInternalQueriesCore()
    {
        foreach (var item in Value.GetInternalQueries())
        {
            yield return item;
        }
        foreach (var item in Argument.GetInternalQueries())
        {
            yield return item;
        }
    }

    /// <summary>
    /// Gets the tokens representing this IN clause.
    /// </summary>
    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        foreach (var item in Value.GetTokens(parent)) yield return item;

        if (IsNegative) yield return Token.Reserved(this, parent, "not");

        yield return Token.Reserved(this, parent, "in");
        foreach (var item in Argument.GetTokens(parent)) yield return item;
    }

    /// <summary>
    /// Gets the parameters associated with this IN clause.
    /// </summary>
    protected override IEnumerable<QueryParameter> GetParametersCore()
    {
        foreach (var item in Value.GetParameters())
        {
            yield return item;
        }
        foreach (var item in Argument.GetParameters())
        {
            yield return item;
        }
    }

    /// <summary>
    /// Gets the physical tables associated with this IN clause.
    /// </summary>
    protected override IEnumerable<PhysicalTable> GetPhysicalTablesCore()
    {
        foreach (var item in Value.GetPhysicalTables())
        {
            yield return item;
        }
        foreach (var item in Argument.GetPhysicalTables())
        {
            yield return item;
        }
    }

    /// <summary>
    /// Gets the common tables associated with this IN clause.
    /// </summary>
    protected override IEnumerable<CommonTable> GetCommonTablesCore()
    {
        foreach (var item in Value.GetCommonTables())
        {
            yield return item;
        }
        foreach (var item in Argument.GetCommonTables())
        {
            yield return item;
        }
    }
}
