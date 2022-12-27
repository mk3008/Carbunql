namespace Carbunql.Values;

public class InlineQuery : QueryContainer
{
    public InlineQuery(IQueryCommandable query) : base(query)
    {
    }
}