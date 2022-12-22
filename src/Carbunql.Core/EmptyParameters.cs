using System.Collections.Immutable;

namespace Carbunql.Core;

internal static class EmptyParameters
{
    public static IDictionary<string, object?> Get() => ImmutableDictionary<string, object?>.Empty;
}