using Carbunql.Analysis.Parser;
using Carbunql.Clauses;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Annotations;

/// <summary>
/// Factory class for creating auto number definitions.
/// </summary>
public static class AutoNumberDefinitionFactory
{
    /// <summary>
    /// Tries to create an auto number definition for the specified type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of class for which to create the auto number definition.</typeparam>
    /// <param name="autoNumberDefinition">When this method returns, contains the auto number definition if successful, or the default value if unsuccessful.</param>
    /// <returns>True if the auto number definition is successfully created; otherwise, false.</returns>
    public static bool TryCreate<T>([MaybeNullWhen(false)] out ValueBase autoNumberDefinition)
    {
        return TryCreate(typeof(T), out autoNumberDefinition);
    }

    /// <summary>
    /// Tries to create an auto number definition for the specified type.
    /// </summary>
    /// <param name="type">The type of class for which to create the auto number definition.</param>
    /// <param name="autoNumberDefinition">When this method returns, contains the auto number definition if successful, or the default value if unsuccessful.</param>
    /// <returns>True if the auto number definition is successfully created; otherwise, false.</returns>
    public static bool TryCreate(Type type, [MaybeNullWhen(false)] out ValueBase autoNumberDefinition)
    {
        autoNumberDefinition = default;

        var atr = (TableAttribute?)Attribute.GetCustomAttribute(type, typeof(TableAttribute));
        var def = string.Empty;

        if (atr != null)
        {
            if (atr.HasAutoNumber == false) return false;
            def = atr.AutoNumberDefinition;
        }

        if (string.IsNullOrEmpty(def))
        {
            def = DbmsConfiguration.GetDefaultAutoNumberDefinitionLogic();
        }

        if (string.IsNullOrEmpty(def))
        {
            return false;
        }

        autoNumberDefinition = ValueParser.Parse(def);
        return true;
    }
}
