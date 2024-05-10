namespace Carbunql.Extensions;

/// <summary>
/// Provides extension methods for the Materialized enum.
/// </summary>
public static class MaterializedTypeExtension
{
    /// <summary>
    /// Converts the Materialized enum value to command text.
    /// </summary>
    /// <param name="source">The Materialized enum value.</param>
    /// <returns>A string representing the command text for the Materialized enum value.</returns>
    public static string ToCommandText(this Materialized source)
    {
        switch (source)
        {
            case Materialized.Undefined:
                return string.Empty;
            case Materialized.Materialized:
                return "materialized";
            case Materialized.NotMaterialized:
                return "not materialized";
        }
        throw new NotSupportedException();
    }
}
