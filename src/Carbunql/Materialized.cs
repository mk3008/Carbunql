namespace Carbunql;

/// <summary>
/// Enumerates the materialization states.
/// </summary>
public enum Materialized
{
    /// <summary>
    /// Indicates an undefined materialization state.
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// Indicates that the entity is materialized.
    /// </summary>
    Materialized = 1,

    /// <summary>
    /// Indicates that the entity is not materialized.
    /// </summary>
    NotMaterialized = 2,
}
