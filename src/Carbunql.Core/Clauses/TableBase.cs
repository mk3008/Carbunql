namespace Carbunql.Core.Clauses;

public abstract class TableBase : IQueryCommand
{
    public abstract IEnumerable<Token> GetTokens(Token? parent);

    public virtual string GetDefaultName() => string.Empty;
}