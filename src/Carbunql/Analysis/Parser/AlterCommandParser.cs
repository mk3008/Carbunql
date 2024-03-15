using Carbunql.Definitions;
using Carbunql.Extensions;

namespace Carbunql.Analysis.Parser;

public class AlterCommandParser
{
	//public static IAlterCommand Parse(ITable t, string text)
	//{
	//	var r = new SqlTokenReader(text);
	//	var q = Parse(t, r);
	//	return q;
	//}

	public static IAlterCommand Parse(ITable t, ITokenReader r)
	{
		var token = r.Peek();
		if (token.IsEqualNoCase("add"))
		{
			return ParseAsAddCommand(t, r);
		}

		if (token.IsEqualNoCase("drop"))
		{
			return ParseAdDropCommand(t, r);
		}

		if (token.IsEqualNoCase("alter column"))
		{
			return ParseAsAlterColumnCommand(t, r);
		}

		if (token.IsEqualNoCase("rename"))
		{
			return ParseAsRenameCommand(t, r);
		}
		throw new NotSupportedException($"Token:{token}");
	}

	private static IAlterCommand ParseAsAddCommand(ITable t, ITokenReader r)
	{
		r.Read("add");
		var token = r.Peek();

		if (token.IsEqualNoCase(new[] { "constraint", "primary key", "unique" }))
		{
			var constraint = ConstraintParser.Parse(t, r);
			return new AddConstraintCommand(constraint);
		}
		else if (token.IsEqualNoCase("column"))
		{
			var definition = ColumnDefinitionParser.Parse(t, r);
			return new AddColumnCommand(definition);
		}
		else
		{
			//Consider "add column" with "column" omitted.
			var definition = ColumnDefinitionParser.Parse(t, r);
			return new AddColumnCommand(definition);
		}

		//throw new NotSupportedException($"Token:{token}");
	}

	private static IAlterCommand ParseAdDropCommand(ITable t, ITokenReader r)
	{
		r.Read("drop");
		var target = r.Read();
		if (target.IsEqualNoCase("column"))
		{
			var name = r.Read();
			return new DropColumnCommand(t, name);
		}
		if (target.IsEqualNoCase("constraint"))
		{
			var name = r.Read();
			return new DropConstraintCommand(t, name);
		}
		throw new NotSupportedException();
	}

	private static IAlterCommand ParseAsAlterColumnCommand(ITable t, ITokenReader r)
	{
		r.Read("alter column");
		var column = r.Read();
		var token = r.Read();
		if (token.IsEqualNoCase("set"))
		{
			token = r.Read();
			if (token.IsEqualNoCase("default"))
			{
				var value = r.Read();
				return new SetDefaultCommand(t, column, value);
			}
			if (token.IsEqualNoCase("not null"))
			{
				return new SetNotNullCommand(t, column);
			}
			throw new NotSupportedException();
		}
		if (token.IsEqualNoCase("drop"))
		{
			token = r.Read();
			if (token.IsEqualNoCase("default"))
			{
				return new DropDefaultCommand(t, column);
			}
			if (token.IsEqualNoCase("not null"))
			{
				return new DropNotNullCommand(t, column);
			}
			throw new NotSupportedException();
		}
		if (token.IsEqualNoCase("type"))
		{
			var columnType = ValueParser.Parse(r);
			return new ChangeColumnTypeCommand(t, column, columnType);
		}
		throw new NotSupportedException();
	}

	private static IAlterCommand ParseAsRenameCommand(ITable t, ITokenReader r)
	{
		r.Read("rename");
		var token = r.Read();
		if (token.IsEqualNoCase("column"))
		{
			//rename column
			var oldName = r.Read();
			r.Read("to");
			var newName = r.Read();
			return new RenameColumnCommand(t, oldName, newName);
		}
		else
		{
			//rename table
			return new RenameTableCommand(t, token);
		}
	}
}
