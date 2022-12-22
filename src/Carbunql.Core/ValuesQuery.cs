using Carbunql.Core.Clauses;

namespace Carbunql.Core;

public class ValuesQuery : QueryBase, IQueryCommandable
{
    public ValuesClause? ValuesClause { get; set; }

    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        if (ValuesClause == null) throw new InvalidProgramException();
        foreach (var item in ValuesClause.GetTokens(parent)) yield return item;
    }
}