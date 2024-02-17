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
	public void Postgres()
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
	public void MySql()
	{
		var text = @"CREATE TABLE employees (
    employee_id INT AUTO_INCREMENT PRIMARY KEY,
    employee_name VARCHAR(100) NOT NULL,
    email VARCHAR(100) UNIQUE,
    hire_date DATE NOT NULL,
    salary DECIMAL(10, 2) CHECK (salary >= 0),
    department_id INT,
    CONSTRAINT fk_department_id FOREIGN KEY (department_id) REFERENCES departments (department_id)
)";
		var v = CreateTableQueryParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(55, lst.Count);
		Assert.Equal(text, v.ToCommand().CommandText, true, true, true);
	}

	[Fact]
	public void SQLServer()
	{
		var text = @"CREATE TABLE employees (
    employee_id INT IDENTITY(1, 1) PRIMARY KEY,
    employee_name NVARCHAR(100) NOT NULL,
    email NVARCHAR(100) UNIQUE,
    hire_date DATE NOT NULL,
    salary DECIMAL(10, 2) CHECK (salary >= 0),
    department_id INT,
    CONSTRAINT fk_department_id FOREIGN KEY (department_id) REFERENCES departments (department_id)
)";
		var v = CreateTableQueryParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(60, lst.Count);
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