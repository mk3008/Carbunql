namespace Carbunql.Clauses;

/// <summary>
/// Represents a "WHEN MATCHED THEN DO NOTHING" or "WHEN NOT MATCHED THEN DO NOTHING" condition in a "MERGE" SQL statement.
/// </summary>
public class MergeWhenNothing : MergeCondition
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MergeWhenNothing"/> class.
    /// </summary>
    public MergeWhenNothing()
    {
    }

    /// <summary>
    /// Gets or sets a value indicating whether this condition is a match condition.
    /// </summary>
    public bool IsMatchCondition { get; set; }

    /// <inheritdoc/>
    public override IEnumerable<QueryParameter> GetParameters()
    {
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
        var txt = (IsMatchCondition) ? "when matched" : "when not matched";
        var t = Token.Reserved(this, parent, txt);
        yield return t;
        foreach (var item in GetConditionTokens(t)) yield return item;
        yield return Token.Reserved(this, parent, "then");
        yield return Token.Reserved(this, parent, "do nothing");
    }
}
