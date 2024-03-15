using Carbunql.Analysis.Parser;
using Carbunql.Definitions;
using Carbunql.Extensions;

namespace Carbunql.Analysis;

public static class ConstraintParser
{
	//public static IConstraint Parse(string text)
	//{
	//	var r = new SqlTokenReader(text);
	//	var q = Parse(r);
	//	return q;
	//}

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
			return new PrimaryKeyConstraint(t)
			{
				ConstraintName = name,
				ColumnNames = columns
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

		throw new NotSupportedException($"Token : {token}");
	}
}
