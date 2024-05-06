using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses an IN clause from SQL text or token streams.
/// </summary>
public static class InClauseParser
{
    /// <summary>
    /// Parses an IN clause from SQL text.
    /// </summary>
    /// <param name="value">The value to check for inclusion.</param>
    /// <param name="argument">The SQL text containing the IN clause.</param>
    /// <returns>The parsed IN clause.</returns>
    public static InClause Parse(ValueBase value, string argument)
    {
        var r = new SqlTokenReader(argument);
        return Parse(value, r, false);
    }

    /// <summary>
    /// Parses an IN clause from the token stream.
    /// </summary>
    /// <param name="value">The value to check for inclusion.</param>
    /// <param name="r">The token reader.</param>
    /// <param name="isNegative">Specifies whether the IN clause is negated.</param>
    /// <returns>The parsed IN clause.</returns>
    public static InClause Parse(ValueBase value, ITokenReader r, bool isNegative)
    {
        r.Read("in");

        using var ir = new BracketInnerTokenReader(r);

        var first = ir.Peek() ?? throw new NotSupportedException();
        if (first.IsEqualNoCase("select"))
        {
            // Subquery
            var iq = new InlineQuery(SelectQueryParser.Parse(ir));
            return new InClause(value, iq, isNegative);
        }
        else
        {
            // Value collection
            var bv = new BracketValue(ValueCollectionParser.Parse(ir));
            return new InClause(value, bv, isNegative);
        }
    }
}
