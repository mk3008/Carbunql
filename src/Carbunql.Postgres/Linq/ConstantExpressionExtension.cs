using Carbunql.Analysis.Parser;
using Carbunql.Clauses;
using Carbunql.Values;
using System.Linq.Expressions;

namespace Carbunql.Postgres.Linq;

internal static class ConstantExpressionExtension
{
    internal static ValueBase ToValue(this ConstantExpression exp)
    {
        var value = exp.Value;
        if (value == null) return ValueParser.Parse("null");
        if (value is DateTime d) return d.ToValue();
        if (value is string s) return ValueParser.Parse($"'{value}'");
        if (value is char c) return ValueParser.Parse($"'{value}'");
        return new LiteralValue(value.ToString());
    }
}
