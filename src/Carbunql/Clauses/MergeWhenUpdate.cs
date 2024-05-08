namespace Carbunql.Clauses;

/// <summary>
/// Represents a "WHEN MATCHED THEN UPDATE" condition in a "MERGE" SQL statement.
/// </summary>
public class MergeWhenUpdate : MergeCondition
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MergeWhenUpdate"/> class.
    /// </summary>
    /// <param name="query">The update query.</param>
    public MergeWhenUpdate(MergeUpdateQuery query)
    {
        Query = query;
    }

    /// <summary>
    /// Gets the update query.
    /// </summary>
    public MergeUpdateQuery Query { get; init; }

    /// <inheritdoc/>
    public override IEnumerable<QueryParameter> GetParameters()
    {
        foreach (var item in Query.GetParameters())
        {
            yield return item;
        }
        if (Condition != null)
        {
            foreach (var item in Condition.GetParameters())
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    public override IEnumerable<Token> GetTokens(Token? parent)
    {
        var t = Token.Reserved(this, parent, "when matched");
        yield return t;
        foreach (var item in GetConditionTokens(t)) yield return item;
        yield return Token.Reserved(this, parent, "then");
        foreach (var item in Query.GetTokens(t)) yield return item;
    }
}
