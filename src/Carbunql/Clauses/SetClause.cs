using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carbunql.Clauses;

public class SetClause : QueryCommandCollection<ValueBase>, IQueryCommand
{
    public override IEnumerable<Token> GetTokens(Token? parent)
    {
        Token clause = GetClauseToken(parent);
        yield return clause;

        foreach (var item in base.GetTokens(clause)) yield return item;
    }

    private Token GetClauseToken(Token? parent)
    {
        return Token.Reserved(this, parent, "set");
    }
}