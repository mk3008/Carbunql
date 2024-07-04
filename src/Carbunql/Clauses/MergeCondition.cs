using Carbunql.Tables;
using Carbunql.Values;

namespace Carbunql.Clauses;

/// <summary>
/// Represents an abstract class for merge conditions in a SQL query.
/// </summary>
public abstract class MergeCondition : IQueryCommandable
{
    /// <summary>
    /// Gets or sets the condition for the merge operation.
    /// </summary>
    public ValueBase? Condition { get; set; }

    /// <summary>
    /// Gets the tokens representing the merge condition.
    /// </summary>
    /// <param name="parent">The parent token.</param>
    /// <returns>The tokens representing the merge condition.</returns>
    public IEnumerable<Token> GetConditionTokens(Token? parent)
    {
        if (Condition == null) yield break;
        yield return Token.Reserved(this, parent, "and");
        foreach (var item in Condition.GetTokens(parent)) yield return item;
    }

    /// <inheritdoc/>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        if (Condition != null)
        {
            foreach (var item in Condition.GetInternalQueries())
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        if (Condition != null)
        {
            foreach (var item in Condition.GetPhysicalTables())
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        if (Condition != null)
        {
            foreach (var item in Condition.GetCommonTables())
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Gets the parameters associated with the merge condition.
    /// </summary>
    /// <returns>The parameters associated with the merge condition.</returns>
    public abstract IEnumerable<QueryParameter> GetParameters();

    /// <summary>
    /// Gets the tokens representing the merge condition.
    /// </summary>
    /// <param name="parent">The parent token.</param>
    /// <returns>The tokens representing the merge condition.</returns>
    public abstract IEnumerable<Token> GetTokens(Token? parent);

    public IEnumerable<ColumnValue> GetColumns()
    {
        if (Condition != null)
        {
            foreach (var item in Condition.GetColumns())
            {
                yield return item;
            }
        }
    }
}
