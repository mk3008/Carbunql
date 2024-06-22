using Carbunql.Analysis.Parser;
using Carbunql.TypeSafe.Building;
using System.Linq.Expressions;

namespace Carbunql.TypeSafe.Extensions;

internal static class ConditionalExpressionExtension
{
    internal static string ToValue(this ConditionalExpression cnd, BuilderEngine engine)
    {
        var test = cnd.Test.ToValue(engine);
        var ifTrue = cnd.IfTrue.ToValue(engine);
        var ifFalse = cnd.IfFalse.ToValue(engine);

        if (string.IsNullOrEmpty(ifFalse))
        {
            throw new ArgumentException("The IfFalse expression cannot be null or empty.", nameof(cnd.IfFalse));
        }

        // When case statements are nested, check if there is an alternative in the when clause
        if (ifFalse.TrimStart().StartsWith("case ", StringComparison.OrdinalIgnoreCase))
        {
            var caseExpression = CaseExpressionParser.Parse(ifFalse);
            if (caseExpression.CaseCondition is null)
            {
                // Replace with when clause
                var we = WhenExpressionParser.Parse($"when {test} then {ifTrue}");
                caseExpression.WhenExpressions.Insert(0, we);
                return caseExpression.ToText();
            }
            else
            {
                return $"case when {test} then {ifTrue} else {ifFalse} end";
            }
        }
        else
        {
            return $"case when {test} then {ifTrue} else {ifFalse} end";
        }
    }
}
