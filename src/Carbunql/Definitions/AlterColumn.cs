using Carbunql.Clauses;
using Carbunql.Tables;

namespace Carbunql.Definitions;

public interface IAlterCommand : IQueryCommandable
{
}

public class AddColumnCommand : IAlterCommand
{
	public AddColumnCommand(ColumnDefinition definition)
	{
		Definition = definition;
	}

	public ColumnDefinition Definition { get; set; }

	public IEnumerable<CommonTable> GetCommonTables()
	{
		yield break;
	}

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		yield break;
	}

	public IEnumerable<QueryParameter> GetParameters()
	{
		yield break;
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		yield break;
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		yield return new Token(this, parent, "add", isReserved: true);
		foreach (var item in Definition.GetTokens(parent))
		{
			yield return item;
		}
	}
}

public class DropColumnCommand : IAlterCommand
{
	public DropColumnCommand(string columnName)
	{
		ColumnName = columnName;
	}

	public string ColumnName { get; set; }

	public IEnumerable<CommonTable> GetCommonTables()
	{
		yield break;
	}

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		yield break;
	}

	public IEnumerable<QueryParameter> GetParameters()
	{
		yield break;
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		yield break;
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		yield return new Token(this, parent, "drop", isReserved: true);
		yield return new Token(this, parent, "column", isReserved: true);
		yield return new Token(this, parent, ColumnName);
	}
}

public class SetDefaultCommand : IAlterCommand
{
	public SetDefaultCommand(string columnName, string defaultValue)
	{
		ColumnName = columnName;
		DefaultValue = defaultValue;
	}

	public string ColumnName { get; set; }

	public string DefaultValue { get; set; }

	public IEnumerable<CommonTable> GetCommonTables()
	{
		yield break;
	}

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		yield break;
	}

	public IEnumerable<QueryParameter> GetParameters()
	{
		yield break;
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		yield break;
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		yield return new Token(this, parent, "alter column", isReserved: true);
		yield return new Token(this, parent, ColumnName);
		yield return new Token(this, parent, "set", isReserved: true);
		yield return new Token(this, parent, "default", isReserved: true);
		yield return new Token(this, parent, DefaultValue);
	}
}

public class DropDefaultCommand : IAlterCommand
{
	public DropDefaultCommand(string columnName)
	{
		ColumnName = columnName;
	}

	public string ColumnName { get; set; }

	public IEnumerable<CommonTable> GetCommonTables()
	{
		yield break;
	}

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		yield break;
	}

	public IEnumerable<QueryParameter> GetParameters()
	{
		yield break;
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		yield break;
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		yield return new Token(this, parent, "alter column", isReserved: true);
		yield return new Token(this, parent, ColumnName);
		yield return new Token(this, parent, "drop", isReserved: true);
		yield return new Token(this, parent, "default", isReserved: true);
	}
}

public class SetNotNullCommand : IAlterCommand
{
	public SetNotNullCommand(string columnName)
	{
		ColumnName = columnName;
	}

	public string ColumnName { get; set; }

	public IEnumerable<CommonTable> GetCommonTables()
	{
		yield break;
	}

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		yield break;
	}

	public IEnumerable<QueryParameter> GetParameters()
	{
		yield break;
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		yield break;
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		yield return new Token(this, parent, "alter column", isReserved: true);
		yield return new Token(this, parent, ColumnName);
		yield return new Token(this, parent, "set", isReserved: true);
		yield return new Token(this, parent, "not null", isReserved: true);
	}
}

public class DropNotNullCommand : IAlterCommand
{
	public DropNotNullCommand(string columnName)
	{
		ColumnName = columnName;
	}

	public string ColumnName { get; set; }

	public IEnumerable<CommonTable> GetCommonTables()
	{
		yield break;
	}

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		yield break;
	}

	public IEnumerable<QueryParameter> GetParameters()
	{
		yield break;
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		yield break;
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		yield return new Token(this, parent, "alter column", isReserved: true);
		yield return new Token(this, parent, ColumnName);
		yield return new Token(this, parent, "drop", isReserved: true);
		yield return new Token(this, parent, "not null", isReserved: true);
	}
}

public class ChangeColumnTypeCommand : IAlterCommand
{
	public ChangeColumnTypeCommand(string columnName, string columnType)
	{
		ColumnName = columnName;
		ColumnType = columnType;
	}

	public string ColumnName { get; set; }

	public string ColumnType { get; set; }

	public IEnumerable<CommonTable> GetCommonTables()
	{
		yield break;
	}

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		yield break;
	}

	public IEnumerable<QueryParameter> GetParameters()
	{
		yield break;
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		yield break;
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		yield return new Token(this, parent, "alter column", isReserved: true);
		yield return new Token(this, parent, ColumnName);
		yield return new Token(this, parent, "type", isReserved: true);
		yield return new Token(this, parent, ColumnType);
	}
}

public class AddConstraintCommand : IAlterCommand
{
	public AddConstraintCommand(IConstraint constraint)
	{
		Constraint = constraint;
	}

	public IConstraint Constraint { get; set; }

	public IEnumerable<CommonTable> GetCommonTables()
	{
		yield break;
	}

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		yield break;
	}

	public IEnumerable<QueryParameter> GetParameters()
	{
		yield break;
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		yield break;
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		yield return new Token(this, parent, "add", isReserved: true);
		foreach (var item in Constraint.GetTokens(parent))
		{
			yield return item;
		}
	}
}

public class DropConstraintCommand : IAlterCommand
{
	public DropConstraintCommand(string constraintName)
	{
		ConstraintName = constraintName;
	}

	public string ConstraintName { get; set; }

	public IEnumerable<CommonTable> GetCommonTables()
	{
		yield break;
	}

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		yield break;
	}

	public IEnumerable<QueryParameter> GetParameters()
	{
		yield break;
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		yield break;
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		yield return new Token(this, parent, "drop", isReserved: true);
		yield return new Token(this, parent, "constraint", isReserved: true);
		yield return new Token(this, parent, ConstraintName);
	}
}

public class RenameTableCommand : IAlterCommand
{
	public RenameTableCommand(string newTableName)
	{
		NewTableName = newTableName;
	}

	public string NewTableName { get; set; }

	public IEnumerable<CommonTable> GetCommonTables()
	{
		yield break;
	}

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		yield break;
	}

	public IEnumerable<QueryParameter> GetParameters()
	{
		yield break;
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		yield break;
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		yield return new Token(this, parent, "rename", isReserved: true);
		yield return new Token(this, parent, "to", isReserved: true);
		yield return new Token(this, parent, NewTableName);
	}
}

public class RenameColumnCommand : IAlterCommand
{
	public RenameColumnCommand(string oldColumnName, string newColumnName)
	{
		OldColumnName = oldColumnName;
		NewColumnName = newColumnName;
	}

	public string OldColumnName { get; set; }

	public string NewColumnName { get; set; }

	public IEnumerable<CommonTable> GetCommonTables()
	{
		yield break;
	}

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		yield break;
	}

	public IEnumerable<QueryParameter> GetParameters()
	{
		yield break;
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		yield break;
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		yield return new Token(this, parent, "rename", isReserved: true);
		yield return new Token(this, parent, "column", isReserved: true);
		yield return new Token(this, parent, OldColumnName);
		yield return new Token(this, parent, "to", isReserved: true);
		yield return new Token(this, parent, NewColumnName);
	}
}