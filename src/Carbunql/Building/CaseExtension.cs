using Carbunql.Analysis.Parser;
using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Building;

/// <summary>
/// Static class providing extension methods for constructing CASE expressions.
/// </summary>
public static class CaseExtension
{
    /// <summary>
    /// Adds a WHEN clause to the CASE expression with the specified condition as a string.
    /// </summary>
    /// <param name="source">The CASE expression.</param>
    /// <param name="condition">The condition for the WHEN clause.</param>
    /// <returns>The created WHEN expression.</returns>
    public static WhenExpression When(this CaseExpression source, string condition)
    {
        var w = new WhenExpression(ValueParser.Parse(condition), new LiteralValue("null"));
        source.WhenExpressions.Add(w);
        return w;
    }

    /// <summary>
    /// Adds a WHEN clause to the CASE expression with the specified condition.
    /// </summary>
    /// <param name="source">The CASE expression.</param>
    /// <param name="condition">The condition for the WHEN clause.</param>
    /// <returns>The created WHEN expression.</returns>
    public static WhenExpression When(this CaseExpression source, ValueBase condition)
    {
        var w = new WhenExpression(condition, new LiteralValue("null"));
        source.WhenExpressions.Add(w);
        return w;
    }

    /// <summary>
    /// Adds a WHEN clause to the CASE expression using a condition builder function.
    /// </summary>
    /// <param name="source">The CASE expression.</param>
    /// <param name="builder">The function to build the condition.</param>
    /// <returns>The created WHEN expression.</returns>
    public static WhenExpression When(this CaseExpression source, Func<ValueBase> builder)
    {
        var w = new WhenExpression(builder(), new LiteralValue("null"));
        source.WhenExpressions.Add(w);
        return w;
    }

    /// <summary>
    /// Sets the THEN value of the WHEN expression to the specified string value.
    /// </summary>
    /// <param name="source">The WHEN expression.</param>
    /// <param name="value">The string value for the THEN clause.</param>
    public static void Then(this WhenExpression source, string value)
    {
        source.SetValue(ValueParser.Parse(value));
    }

    /// <summary>
    /// Sets the THEN value of the WHEN expression.
    /// </summary>
    /// <param name="source">The WHEN expression.</param>
    /// <param name="value">The value for the THEN clause.</param>
    public static void Then(this WhenExpression source, ValueBase value)
    {
        source.SetValue(value);
    }

    /// <summary>
    /// Adds an ELSE clause to the CASE expression with the specified string value.
    /// </summary>
    /// <param name="source">The CASE expression.</param>
    /// <param name="value">The string value for the ELSE clause.</param>
    public static void Else(this CaseExpression source, string value)
    {
        var w = new WhenExpression(ValueParser.Parse(value));
        source.WhenExpressions.Add(w);
    }

    /// <summary>
    /// Adds an ELSE clause to the CASE expression.
    /// </summary>
    /// <param name="source">The CASE expression.</param>
    /// <param name="value">The value for the ELSE clause.</param>
    public static void Else(this CaseExpression source, ValueBase value)
    {
        var w = new WhenExpression(value);
        source.WhenExpressions.Add(w);
    }
}
