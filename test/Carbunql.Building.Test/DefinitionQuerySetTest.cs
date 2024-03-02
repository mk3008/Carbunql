using Carbunql.Analysis;
using System.Text;
using Xunit.Abstractions;

namespace Carbunql.Building.Test;

public class DefinitionQuerySetTest
{
	public DefinitionQuerySetTest(ITestOutputHelper output)
	{
		Output = output;
	}

	private ITestOutputHelper Output { get; set; }

	[Fact]
	public void Default()
	{
		var text = @"CREATE TABLE child_table (
    child_id SERIAL PRIMARY KEY,
    child_name VARCHAR(100) not null unique,
    parent_id INT not null, 
	remarks text
)
;
";

		var expect = @"CREATE TABLE child_table (
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
";

		var v = DefinitionQuerySetParser.Parse(text);
		var sb = new StringBuilder();

		var diffv = v.CreateTableQuery.ToDefinitionQuerySet();
		diffv.AlterTableQueries.AddRange(v.AlterTableQueries);
		diffv.CreateIndexQueries.AddRange(v.CreateIndexQueries);

		sb.AppendLine(diffv.CreateTableQuery.ToCommand().CommandText);
		sb.AppendLine(";");
		foreach (var item in diffv.AlterTableQueries)
		{
			sb.AppendLine(item.ToCommand().CommandText);
			sb.AppendLine(";");

		}
		foreach (var item in diffv.CreateIndexQueries)
		{
			sb.AppendLine(item.ToCommand().CommandText);
			sb.AppendLine(";");
		}

		Output.WriteLine(sb.ToString());

		Assert.Equal(expect, sb.ToString(), true, true, true);
	}
}