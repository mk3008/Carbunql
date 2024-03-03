using Carbunql.Definitions;
using Carbunql.Extensions;

namespace Carbunql.Analysis.Parser;

public class AlterCommandParser
{
	public static IAlterCommand Parse(string text)
	{
		var r = new SqlTokenReader(text);
		var q = Parse(r);
		return q;
	}

	public static IAlterCommand Parse(ITokenReader r)
	{
		var token = r.Peek();
		if (token.IsEqualNoCase("add"))
		{
			return ParseAsAddCommand(r);
		}

		if (token.IsEqualNoCase("drop"))
		{
			return ParseAdDropCommand(r);
		}

		if (token.IsEqualNoCase("alter column"))
		{
			return ParseAsAlterColumnCommand(r);
		}

		if (token.IsEqualNoCase("rename"))
		{
			return ParseAsRenameCommand(r);
		}
		throw new NotSupportedException($"Token:{token}");
	}

	private static IAlterCommand ParseAsAddCommand(ITokenReader r)
	{
		r.Read("add");
		var token = r.Peek();

		if (token.IsEqualNoCase(new[] { "constraint", "primary key", "unique" }))
		{
			var constraint = ConstraintParser.Parse(r);
			return new AddConstraintCommand(constraint);
		}
		else if (token.IsEqualNoCase("column"))
		{
			var definition = ColumnDefinitionParser.Parse(r);
			return new AddColumnCommand(definition);
		}
		else
		{
			//Consider "add column" with "column" omitted.
			var definition = ColumnDefinitionParser.Parse(r);
			return new AddColumnCommand(definition);
		}

		//throw new NotSupportedException($"Token:{token}");
	}

	private static IAlterCommand ParseAdDropCommand(ITokenReader r)
	{
		r.Read("drop");
		var target = r.Read();
		if (target.IsEqualNoCase("column"))
		{
			var name = r.Read();
			return new DropColumnCommand(name);
		}
		if (target.IsEqualNoCase("constraint"))
		{
			var name = r.Read();
			return new DropConstraintCommand(name);
		}
		throw new NotSupportedException();
	}

	private static IAlterCommand ParseAsAlterColumnCommand(ITokenReader r)
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
				return new SetDefaultCommand(column, value);
			}
			if (token.IsEqualNoCase("not null"))
			{
				return new SetNotNullCommand(column);
			}
			throw new NotSupportedException();
		}
		if (token.IsEqualNoCase("drop"))
		{
			token = r.Read();
			if (token.IsEqualNoCase("default"))
			{
				return new DropDefaultCommand(column);
			}
			if (token.IsEqualNoCase("not null"))
			{
				return new DropNotNullCommand(column);
			}
			throw new NotSupportedException();
		}
		if (token.IsEqualNoCase("type"))
		{
			var columnType = ValueParser.Parse(r);
			return new ChangeColumnTypeCommand(column, columnType);
		}
		throw new NotSupportedException();
	}

	private static IAlterCommand ParseAsRenameCommand(ITokenReader r)
	{
		r.Read("rename");
		var token = r.Read();
		if (token.IsEqualNoCase("column"))
		{
			//rename column
			var oldName = r.Read();
			r.Read("to");
			var newName = r.Read();
			return new RenameColumnCommand(oldName, newName);
		}
		else
		{
			//rename table
			return new RenameTableCommand(token);
		}
	}
}
