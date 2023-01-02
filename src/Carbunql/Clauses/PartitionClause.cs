namespace Carbunql.Clauses;

public class PartitionClause : QueryCommandCollection<ValueBase>, IQueryCommand
{
    public PartitionClause() : base()
    {
    }

    public PartitionClause(List<ValueBase> collection) : base(collection)
    {
    }

    public override IEnumerable<Token> GetTokens(Token? parent)
    {
        if (!Items.Any()) yield break;

        var clause = Token.Reserved(this, parent, "partition by");
        yield return clause;

        foreach (var item in base.GetTokens(clause)) yield return item;
    }
}