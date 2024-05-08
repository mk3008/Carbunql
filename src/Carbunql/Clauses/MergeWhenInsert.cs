namespace Carbunql.Clauses;

/// <summary>
/// Represents a "WHEN NOT MATCHED THEN INSERT" condition in a "MERGE" SQL statement.
/// </summary>
public class MergeWhenInsert : MergeCondition
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MergeWhenInsert"/> class with the specified <see cref="MergeInsertQuery"/>.
    /// </summary>
    /// <param name="query">The <see cref="MergeInsertQuery"/> to be executed when the condition is met.</param>
    public MergeWhenInsert(MergeInsertQuery query)
    {
        Query = query;
    }

    /// <summary>
    /// Gets the <see cref="MergeInsertQuery"/> to be executed when the condition is met.
    /// </summary>
    public MergeInsertQuery Query { get; init; }

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
        var t = Token.Reserved(this, parent, "when not matched");
        yield return t;
        foreach (var item in GetConditionTokens(t)) yield return item;
        yield return Token.Reserved(this, parent, "then");
        foreach (var item in Query.GetTokens(t)) yield return item;
    }
}
