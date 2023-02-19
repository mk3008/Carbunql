using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Building;

public static class ValueCollectionExtension
{
    public static ValueCollection Add(this ValueCollection source, string value)
    {
        source.Add(new LiteralValue(value));
        return source;
    }
}
