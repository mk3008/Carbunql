using Carbunql.Clauses;
using Carbunql.Tables;

namespace Carbunql.Values;

/// <summary>
/// Represents a LIKE clause in a query.
/// </summary>
public class LikeClause : ValueBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LikeClause"/> class with the specified value, argument, and negation indicator.
    /// </summary>
    /// <param name="value">The value to compare.</param>
    /// <param name="argument">The argument to compare against.</param>
    /// <param name="isNegative">Indicates whether the comparison is negated.</param>
    public LikeClause(ValueBase value, ValueBase argument, bool isNegative = false)
    {
        Value = value;
        Argument = argument;
        IsNegative = isNegative;
    }

    /// <summary>
    /// Gets the value to compare.
    /// </summary>
    public ValueBase Value { get; init; }

    /// <summary>
    /// Gets the argument to compare against.
    /// </summary>
    public ValueBase Argument { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the comparison is negated.
    /// </summary>
    public bool IsNegative { get; set; }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        foreach (var item in Value.GetTokens(parent)) yield return item;

        if (IsNegative) yield return Token.Reserved(this, parent, "not");

        yield return Token.Reserved(this, parent, "like");
        foreach (var item in Argument.GetTokens(parent)) yield return item;
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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
