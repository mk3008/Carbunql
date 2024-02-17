﻿using Carbunql.Definitions;
using Carbunql.Extensions;

namespace Carbunql.Analysis.Parser;

public static class ColumnDefinitionParser
{
	public static ColumnDefinition Parse(string text)
	{
		var r = new SqlTokenReader(text);
		var q = Parse(r);
		return q;
	}

	public static ColumnDefinition Parse(ITokenReader r)
	{
		var columnName = r.Read();
		var columnType = ValueParser.Parse(r);

		var c = new ColumnDefinition(columnName, columnType);

		var token = r.Peek();
		if (token.IsEqualNoCase("not null"))
		{
			r.Read();
			c.IsNullable = false;
			token = r.Peek();
		}
		else
		{
			c.IsNullable = true;
		}

		if (token.IsEqualNoCase("default"))
		{
			r.Read();
			c.DefaultValueDefinition = ValueParser.Parse(r);
			token = r.Peek();
		}

		if (token.IsEqualNoCase("primary key"))
		{
			r.Read();
			c.IsPrimaryKey = true;
			token = r.Peek();
		}

		if (token.IsEqualNoCase("unique"))
		{
			r.Read();
			c.IsUniqueKey = true;
			token = r.Peek();
		}

		if (token.IsEqualNoCase("check"))
		{
			r.Read();
			c.CheckDefinition = ValueParser.Parse(r);
			token = r.Peek();
		}

		if (token == "," || token == ")") return c;
		throw new NotSupportedException($"Token : {token}");
	}
}