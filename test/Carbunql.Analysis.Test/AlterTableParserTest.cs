using Xunit.Abstractions;

namespace Carbunql.Analysis.Test;

public class AlterTableParserTest
{
	private readonly QueryCommandMonitor Monitor;

	public AlterTableParserTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
	}

	[Fact]
	public void AddColumn()
	{
		var text = @"ALTER TABLE table_name
	ADD COLUMN column_name datatype";
		var v = AlterTableQueryParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(6, lst.Count);
		Assert.Equal(text, v.ToCommand().CommandText, true, true, true);
	}

	[Fact]
	public void AddColumn_omit()
	{
		var text = @"ALTER TABLE table_name
	ADD column_name datatype";
		var v = AlterTableQueryParser.Parse(text);
		Monitor.Log(v);

		var expect = @"ALTER TABLE table_name
	ADD COLUMN column_name datatype";

		var lst = v.GetTokens().ToList();
		Assert.Equal(6, lst.Count);
		Assert.Equal(expect, v.ToCommand().CommandText, true, true, true);
	}

	[Fact]
	public void DropColumn()
	{
		var text = @"ALTER TABLE table_name
	DROP COLUMN column_name";
		var v = AlterTableQueryParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(5, lst.Count);
		Assert.Equal(text, v.ToCommand().CommandText, true, true, true);
	}

	[Fact]
	public void SetDefault()
	{
		var text = @"ALTER TABLE table_name
	ALTER COLUMN column_name SET DEFAULT default_value";
		var v = AlterTableQueryParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(7, lst.Count);
		Assert.Equal(text, v.ToCommand().CommandText, true, true, true);
	}

	[Fact]
	public void ColumnType()
	{
		var text = @"ALTER TABLE table_name
	ALTER COLUMN column_name TYPE new_datatype";
		var v = AlterTableQueryParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(6, lst.Count);
		Assert.Equal(text, v.ToCommand().CommandText, true, true, true);
	}

	[Fact]
	public void AddConstraint()
	{
		var text = @"ALTER TABLE table_name
	ADD CONSTRAINT constraint_name UNIQUE (column_name)";
		var v = AlterTableQueryParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(9, lst.Count);
		Assert.Equal(text, v.ToCommand().CommandText, true, true, true);
	}

	[Fact]
	public void DropConstraint()
	{
		var text = @"ALTER TABLE table_name
	DROP CONSTRAINT constraint_name";
		var v = AlterTableQueryParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(5, lst.Count);
		Assert.Equal(text, v.ToCommand().CommandText, true, true, true);
	}

	[Fact]
	public void RenameColumn()
	{
		var text = @"ALTER TABLE table_name
	RENAME COLUMN old_column_name TO new_column_name";
		var v = AlterTableQueryParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(7, lst.Count);
		Assert.Equal(text, v.ToCommand().CommandText, true, true, true);
	}
}