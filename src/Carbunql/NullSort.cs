namespace Carbunql;

/// <summary>
/// Enumerates options for sorting null values.
/// </summary>
public enum NullSort
{
    /// <summary>
    /// Indicates that null sorting is undefined.
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// Indicates that null values should appear first when sorting.
    /// </summary>
    First = 1,

    /// <summary>
    /// Indicates that null values should appear last when sorting.
    /// </summary>
    Last = 2,
}
