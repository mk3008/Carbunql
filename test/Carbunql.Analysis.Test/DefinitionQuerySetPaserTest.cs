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
    child_name VARCHAR(100),
    parent_id INT
)
;
ALTER TABLE child_table ADD CONSTRAINT fk_parent_id FOREIGN KEY (parent_id) REFERENCES parent_table (parent_id) ON DELETE CASCADE
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

	//ALTER TABLE employees ADD CONSTRAINT employees_department_id_fkey FOREIGN KEY (department_id) REFERENCES departments(department_id) ON DELETE CASCADE;
}