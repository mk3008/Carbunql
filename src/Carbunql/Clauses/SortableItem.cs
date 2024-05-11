using Carbunql.Extensions;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Clauses;

/// <summary>
/// Represents a sortable item used in ORDER BY clauses in a query.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class SortableItem : IQueryCommandable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SortableItem"/> class with the specified value, sort order, and null sorting behavior.
    /// </summary>
    /// <param name="value">The value to sort.</param>
    /// <param name="isAscending">Specifies whether the sorting is ascending (true) or descending (false).</param>
    /// <param name="nullSort">Specifies the behavior for sorting null values.</param>
    public SortableItem(ValueBase value, bool isAscending = true, NullSort nullSort = NullSort.Undefined)
    {
        Value = value;
        IsAscending = isAscending;
        NullSort = nullSort;
    }

    /// <summary>
    /// Gets or sets the value or column to be selected. This can include expressions, inline queries, or simple column names.
    /// </summary>
    public ValueBase Value { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the sorting is ascending (true) or descending (false).
    /// </summary>
    public bool IsAscending { get; set; } = true;

    /// <summary>
    /// Gets or sets the behavior for sorting null values.
    /// </summary>
    public NullSort NullSort { get; set; } = NullSort.Undefined;

    /// <inheritdoc/>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        foreach (var item in Value.GetInternalQueries())
        {
            yield return item;
        }
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
    public IEnumerable<CommonTable> GetCommonTables()
    {
        foreach (var item in Value.GetCommonTables())
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
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        foreach (var item in Value.GetTokens(parent)) yield return item;
        if (!IsAscending) yield return Token.Reserved(this, parent, "desc");
        if (NullSort != NullSort.Undefined) yield return Token.Reserved(this, parent, NullSort.ToCommandText());
    }
}
