using Carbunql.Tables;
using Carbunql.Values;
using MessagePack;

namespace Carbunql.Clauses;

/// <summary>
/// Represents a WITHOUT TIME ZONE clause in a query.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class WithoutTimeZoneClause : ValueBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WithoutTimeZoneClause"/> class.
    /// </summary>
    public WithoutTimeZoneClause()
    {
        Value = null!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WithoutTimeZoneClause"/> class with the specified value.
    /// </summary>
    /// <param name="value">The value to be associated with the clause.</param>
    public WithoutTimeZoneClause(ValueBase value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets or sets the value associated with the WITHOUT TIME ZONE clause.
    /// </summary>
    public ValueBase Value { get; init; }

    /// <summary>
    /// Retrieves the internal queries associated with the clause.
    /// </summary>
    /// <returns>An enumerable collection of internal queries.</returns>
    protected override IEnumerable<SelectQuery> GetInternalQueriesCore()
    {
        foreach (var item in Value.GetInternalQueries())
        {
            yield return item;
        }
    }

    /// <summary>
    /// Retrieves the current tokens associated with the clause.
    /// </summary>
    /// <param name="parent">The parent token.</param>
    /// <returns>An enumerable collection of tokens.</returns>
    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        foreach (var item in Value.GetTokens(parent)) yield return item;

        yield return Token.Reserved(this, parent, "without time zone");
    }

    /// <summary>
    /// Retrieves the query parameters associated with the clause.
    /// </summary>
    /// <returns>An enumerable collection of query parameters.</returns>
    protected override IEnumerable<QueryParameter> GetParametersCore()
    {
        return Value.GetParameters();
    }

    /// <summary>
    /// Retrieves the physical tables associated with the clause.
    /// </summary>
    /// <returns>An enumerable collection of physical tables.</returns>
    protected override IEnumerable<PhysicalTable> GetPhysicalTablesCore()
    {
        foreach (var item in Value.GetPhysicalTables())
        {
            yield return item;
        }
    }

    /// <summary>
    /// Retrieves the common tables associated with the clause.
    /// </summary>
    /// <returns>An enumerable collection of common tables.</returns>
    protected override IEnumerable<CommonTable> GetCommonTablesCore()
    {
        foreach (var item in Value.GetCommonTables())
        {
            yield return item;
        }
    }

    internal override IEnumerable<ColumnValue> GetColumnsCore()
    {
        foreach (var item in Value.GetColumns())
        {
            yield return item;
        }
    }
}
