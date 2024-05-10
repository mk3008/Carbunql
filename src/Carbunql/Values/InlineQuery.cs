namespace Carbunql.Values;

/// <summary>
/// Represents an inline query.
/// </summary>
public class InlineQuery : QueryContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InlineQuery"/> class with the specified query.
    /// </summary>
    /// <param name="query">The query commandable.</param>
    public InlineQuery(IQueryCommandable query) : base(query)
    {
    }
}
