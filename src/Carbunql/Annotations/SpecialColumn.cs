namespace Carbunql.Annotations;

/// <summary>
/// Enum representing special columns.
/// </summary>
public enum SpecialColumn
{
    /// <summary>
    /// No special column.
    /// </summary>
    None,

    /// <summary>
    /// Special column for storing the creation timestamp.
    /// </summary>
    CreateTimestamp,

    /// <summary>
    /// Special column for storing the update timestamp.
    /// </summary>
    UpdateTimestamp,

    /// <summary>
    /// Special column for storing version numbers.
    /// </summary>
    VersionNumber,

    /// <summary>
    /// Special column representing a parent relation.
    /// </summary>
    ParentRelation
}
