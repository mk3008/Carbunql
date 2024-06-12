using Carbunql.Definitions;
using Carbunql.Extensions;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses a constraint from token streams.
/// </summary>
public static class ConstraintParser
{
    /// <summary>
    /// Parses a constraint from the token stream.
    /// </summary>
    /// <param name="t">The table the constraint belongs to.</param>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed constraint.</returns>
    public static IConstraint Parse(ITable t, ITokenReader r)
    {
        var token = r.Read();
        var name = string.Empty;
        if (token.IsEqualNoCase("constraint"))
        {
            name = r.Read();
            token = r.Read();
        }

        if (token.IsEqualNoCase("primary key"))
        {
            var columns = ArrayParser.Parse(r);
            return new PrimaryKeyConstraint(t, columns)
            {
                ConstraintName = name,
            };
        }

        if (token.IsEqualNoCase("unique"))
        {
            var columns = ArrayParser.Parse(r);
            return new UniqueConstraint(t)
            {
                ConstraintName = name,
                ColumnNames = columns
            };
        }

        if (token.IsEqualNoCase("check"))
        {
            var val = ValueParser.Parse(r);
            return new CheckConstraint(t)
            {
                ConstraintName = name,
                Value = val
            };
        }

        if (token.IsEqualNoCase("foreign key"))
        {
            var columns = ArrayParser.Parse(r);
            var reference = ReferenceParser.Parse(r);
            return new ForeignKeyConstraint(t)
            {
                ConstraintName = name,
                ColumnNames = columns,
                Reference = reference
            };
        }

        throw new NotSupportedException($"Unsupported token:{token}");
    }
}
