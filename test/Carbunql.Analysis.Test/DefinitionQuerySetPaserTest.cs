using System.Text;
using Xunit.Abstractions;

namespace Carbunql.Analysis.Test;

public class DefinitionQuerySetPaserTest
{
	public DefinitionQuerySetPaserTest(ITestOutputHelper output)
	{
		Output = output;
	}

	private ITestOutputHelper Output { get; set; }

	[Fact]
	public void Default()
	{
		var text = @"CREATE TABLE child_table (
    child_id SERIAL PRIMARY KEY,
    child_name VARCHAR(100) NOT NULL,
    parent_id INT NOT NULL,
    remarks text DEFAULT ''
)
;
ALTER TABLE child_table
	ADD CONSTRAINT fk_parent_id FOREIGN KEY (parent_id) REFERENCES parent_table (parent_id) ON DELETE CASCADE
;
CREATE UNIQUE INDEX idx_child_name ON child_table (
    child_name
)
;
";
		var v = DefinitionQuerySetParser.Parse(text);
		var sb = new StringBuilder();

		sb.AppendLine(v.CreateTableQuery.ToCommand().CommandText);
		sb.AppendLine(";");
		foreach (var item in v.AlterTableQueries)
		{
			sb.AppendLine(item.ToCommand().CommandText);
			sb.AppendLine(";");

		}
		foreach (var item in v.CreateIndexQueries)
		{
			sb.AppendLine(item.ToCommand().CommandText);
			sb.AppendLine(";");
		}

		Output.WriteLine(sb.ToString());

		Assert.Equal(text, sb.ToString(), true, true, true);
	}

	[Fact]
	public void AlterColumn()
	{
		var text = @"CREATE TABLE child_table (
    child_id SERIAL,
    child_name VARCHAR(100),
    parent_id INT,
    remarks text
)
;
ALTER TABLE child_table
	ADD PRIMARY KEY (child_id)
;
ALTER TABLE child_table
	ADD UNIQUE (child_name)
;
ALTER TABLE child_table
	ALTER COLUMN child_name SET NOT NULL
;
ALTER TABLE child_table
	ALTER COLUMN parent_id SET NOT NULL
;
ALTER TABLE child_table
	ALTER COLUMN remarks SET NOT NULL
;
ALTER TABLE child_table
	ALTER COLUMN remarks SET DEFAULT ''
;
ALTER TABLE child_table
	ADD CONSTRAINT fk_parent_id FOREIGN KEY (parent_id) REFERENCES parent_table (parent_id) ON DELETE CASCADE
;
CREATE UNIQUE INDEX idx_child_name ON child_table (
    child_name
)
;
";
		var v = DefinitionQuerySetParser.Parse(text);
		var sb = new StringBuilder();

		sb.AppendLine(v.CreateTableQuery.ToCommand().CommandText);
		sb.AppendLine(";");
		foreach (var item in v.AlterTableQueries)
		{
			sb.AppendLine(item.ToCommand().CommandText);
			sb.AppendLine(";");

		}
		foreach (var item in v.CreateIndexQueries)
		{
			sb.AppendLine(item.ToCommand().CommandText);
			sb.AppendLine(";");
		}

		Output.WriteLine(sb.ToString());

		Assert.Equal(text, sb.ToString(), true, true, true);
	}

	[Fact]
	public void AlterColumnMany()
	{
		var text = @"CREATE TABLE child_table (
    child_id SERIAL,
    child_name VARCHAR(100),
    parent_id INT,
    remarks text
)
;
ALTER TABLE child_table
    ADD PRIMARY key (child_id),
    ADD UNIQUE (child_name),
    ALTER COLUMN child_name SET NOT NULL,
    ALTER COLUMN parent_id SET NOT NULL,
    ALTER COLUMN remarks SET NOT NULL,
    ALTER COLUMN remarks SET DEFAULT '',
    ADD CONSTRAINT fk_parent_id FOREIGN KEY (parent_id) REFERENCES parent_table (parent_id) ON DELETE CASCADE
;
CREATE UNIQUE INDEX idx_child_name ON child_table (
    child_name
)
;
";
		var v = DefinitionQuerySetParser.Parse(text);
		var sb = new StringBuilder();

		sb.AppendLine(v.CreateTableQuery.ToCommand().CommandText);
		sb.AppendLine(";");
		foreach (var item in v.AlterTableQueries)
		{
			sb.AppendLine(item.ToCommand().CommandText);
			sb.AppendLine(";");

		}
		foreach (var item in v.CreateIndexQueries)
		{
			sb.AppendLine(item.ToCommand().CommandText);
			sb.AppendLine(";");
		}

		Output.WriteLine(sb.ToString());

		Assert.Equal(text, sb.ToString(), true, true, true);
	}
}