using System.Collections.Immutable;

namespace Carbunql;

/// <summary>
/// Provides a utility for obtaining an empty set of parameters.
/// </summary>
internal static class EmptyParameters
{
    /// <summary>
    /// Gets an empty dictionary of parameters.
    /// </summary>
    /// <returns>An empty dictionary of parameters.</returns>
    public static IDictionary<string, object?> Get() => ImmutableDictionary<string, object?>.Empty;
}
