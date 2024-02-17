using Xunit.Abstractions;

namespace Carbunql.Analysis.Test;

public class CreateTableParseTest
{
	private readonly QueryCommandMonitor Monitor;

	public CreateTableParseTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
	}

	[Fact]
	public void Default()
	{
		var text = @"CREATE TABLE employees (
    employee_id SERIAL PRIMARY KEY,
    employee_name VARCHAR(100) NOT NULL,
    email VARCHAR(100) UNIQUE,
    hire_date DATE NOT NULL,
    salary DECIMAL CHECK (salary >= 0),
    department_id INT,
    CONSTRAINT fk_department_id FOREIGN KEY (department_id) REFERENCES departments (department_id)
)";
		var v = CreateTableQueryParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(49, lst.Count);
		Assert.Equal(text, v.ToCommand().CommandText, true, true, true);
	}

	[Fact]
	public void CreateTableAsSelectQuery()
	{
		var text = @"CREATE TABLE temp_table
AS
SELECT
    col1,
    col2
FROM
    table_a";
		var v = CreateTableQueryParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(9, lst.Count);
		Assert.Equal(text, v.ToCommand().CommandText, true, true, true);
	}

	[Fact]
	public void CreateTempoaryTableAsSelectQuery()
	{
		var text = @"CREATE TEMPORARY TABLE temp_table
AS
SELECT
    col1,
    col2
FROM
    table_a";
		var v = CreateTableQueryParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(9, lst.Count);
		Assert.Equal(text, v.ToCommand().CommandText, true, true, true);
	}
}