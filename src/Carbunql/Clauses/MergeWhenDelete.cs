namespace Carbunql.Clauses;

/// <summary>
/// Represents a "WHEN MATCHED THEN DELETE" condition in a "MERGE" SQL statement.
/// </summary>
public class MergeWhenDelete : MergeCondition
{
    /// <inheritdoc/>
    public MergeWhenDelete()
    {
    }

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
        var t = Token.Reserved(this, parent, "when matched");
        yield return t;
        foreach (var item in GetConditionTokens(t)) yield return item;
        yield return Token.Reserved(this, parent, "then");
        yield return Token.Reserved(this, parent, "delete");
    }
}
