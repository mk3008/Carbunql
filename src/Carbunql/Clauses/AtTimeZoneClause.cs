using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Clauses;

/// <summary>
/// Represents a clause for specifying the time zone for a datetime value.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class AtTimeZoneClause : ValueBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AtTimeZoneClause"/> class.
    /// </summary>
    public AtTimeZoneClause()
    {
        Value = null!;
        TimeZone = null!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AtTimeZoneClause"/> class with the specified value and time zone.
    /// </summary>
    /// <param name="value">The datetime value.</param>
    /// <param name="timeZone">The time zone.</param>
    public AtTimeZoneClause(ValueBase value, ValueBase timeZone)
    {
        Value = value;
        TimeZone = timeZone;
    }

    /// <summary>
    /// Gets or sets the datetime value.
    /// </summary>
    public ValueBase Value { get; init; }

    /// <summary>
    /// Gets or sets the time zone.
    /// </summary>
    public ValueBase TimeZone { get; init; }

    /// <summary>
    /// Gets the internal queries associated with this clause.
    /// </summary>
    protected override IEnumerable<SelectQuery> GetInternalQueriesCore()
    {
        foreach (var item in Value.GetInternalQueries())
        {
            yield return item;
        }
        foreach (var item in TimeZone.GetInternalQueries())
        {
            yield return item;
        }
    }

    /// <summary>
    /// Gets the tokens representing this clause.
    /// </summary>
    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        foreach (var item in Value.GetTokens(parent)) yield return item;

        yield return Token.Reserved(this, parent, "at time zone");
        foreach (var item in TimeZone.GetTokens(parent)) yield return item;
    }

    /// <summary>
    /// Gets the parameters associated with this clause.
    /// </summary>
    protected override IEnumerable<QueryParameter> GetParametersCore()
    {
        foreach (var item in Value.GetParameters())
        {
            yield return item;
        }
        foreach (var item in TimeZone.GetParameters())
        {
            yield return item;
        }
    }

    /// <summary>
    /// Gets the physical tables associated with this clause.
    /// </summary>
    protected override IEnumerable<PhysicalTable> GetPhysicalTablesCore()
    {
        foreach (var item in Value.GetPhysicalTables())
        {
            yield return item;
        }
        foreach (var item in TimeZone.GetPhysicalTables())
        {
            yield return item;
        }
    }

    /// <summary>
    /// Gets the common tables associated with this clause.
    /// </summary>
    protected override IEnumerable<CommonTable> GetCommonTablesCore()
    {
        foreach (var item in Value.GetCommonTables())
        {
            yield return item;
        }
        foreach (var item in TimeZone.GetCommonTables())
        {
            yield return item;
        }
    }
}
