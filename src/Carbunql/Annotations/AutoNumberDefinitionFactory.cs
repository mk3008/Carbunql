using Carbunql.Analysis.Parser;
using Carbunql.Clauses;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Annotations;

public static class AutoNumberDefinitionFactory
{
    public static bool TryCreate<T>([MaybeNullWhen(false)] out ValueBase autoNumberDefinition)
    {
        autoNumberDefinition = default;

        var atr = (TableAttribute?)Attribute.GetCustomAttribute(typeof(T), typeof(TableAttribute));
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
