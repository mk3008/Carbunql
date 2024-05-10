namespace Carbunql.Extensions;

/// <summary>
/// Provides extension methods for the NullSort enum.
/// </summary>
public static class NullSortTypeExtension
{
    /// <summary>
    /// Converts the NullSort enum value to command text.
    /// </summary>
    /// <param name="source">The NullSort enum value.</param>
    /// <returns>A string representing the command text for the NullSort enum value.</returns>
    public static string ToCommandText(this NullSort source)
    {
        switch (source)
        {
            case NullSort.Undefined:
                return string.Empty;
            case NullSort.First:
                return "nulls first";
            case NullSort.Last:
                return "nulls last";
        }
        throw new NotSupportedException();
    }
}
