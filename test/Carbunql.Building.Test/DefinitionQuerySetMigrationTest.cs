using Carbunql.Analysis;
using System.Text;
using Xunit.Abstractions;

namespace Carbunql.Building.Test;

public class DefinitionQuerySetMigrationTest
{
	public DefinitionQuerySetMigrationTest(ITestOutputHelper output)
	{
		Output = output;
	}

	private ITestOutputHelper Output { get; set; }

	private string GetQueryText(DefinitionQuerySet queryset)
	{
		var sb = new StringBuilder();

		if (queryset.CreateTableQuery != null)
		{
			sb.AppendLine(queryset.CreateTableQuery.ToCommand().CommandText);
			sb.AppendLine(";");
		}

		foreach (var item in queryset.AlterTableQueries)
		{
			sb.AppendLine(item.ToCommand().CommandText);
			sb.AppendLine(";");

		}
		foreach (var item in queryset.AlterIndexQueries)
		{
			sb.AppendLine(item.ToCommand().CommandText);
			sb.AppendLine(";");
		}

		Output.WriteLine(sb.ToString());

		return sb.ToString();
	}

	[Fact]
	public void AddColumn()
	{
		var expect = @"CREATE TABLE child_table (
    child_id SERIAL PRIMARY KEY,
    child_name VARCHAR(100) NOT NULL UNIQUE,
    parent_id INT NOT NULL,
    value INT NOT NULL CHECK (value >= 0),
    remarks TEXT DEFAULT ''
)
";

		var actual = @"CREATE TABLE child_table (
    child_id SERIAL PRIMARY KEY
)
";

		var result = @"ALTER TABLE child_table
    ADD COLUMN child_name VARCHAR(100) NOT NULL
;
ALTER TABLE child_table
    ADD COLUMN parent_id INT NOT NULL
;
ALTER TABLE child_table
    ADD COLUMN value INT NOT NULL
;
ALTER TABLE child_table
    ADD COLUMN remarks TEXT DEFAULT ''
;
ALTER TABLE child_table
    ADD UNIQUE (child_name)
;
ALTER TABLE child_table
    ADD CHECK (value >= 0)
;
";

		var expectQuerySet = DefinitionQuerySetParser.Parse(expect);
		var actualQuerySet = DefinitionQuerySetParser.Parse(actual);

		var queryset = actualQuerySet.GenerateMigrationQuery(expectQuerySet);

		var sql = GetQueryText(queryset);

		Assert.Equal(result, sql, true, true, true);
	}

	[Fact]
	public void DropColumn()
	{
		var expect = @"CREATE TABLE child_table (
    child_id SERIAL PRIMARY KEY
)
";

		var actual = @"CREATE TABLE child_table (
    child_id SERIAL PRIMARY KEY,
    child_name VARCHAR(100) NOT NULL UNIQUE,
    parent_id INT NOT NULL,
    value INT NOT NULL CHECK (value >= 0),
    remarks TEXT DEFAULT ''
)
";

		var result = @"ALTER TABLE child_table
    DROP COLUMN child_name
;
ALTER TABLE child_table
    DROP COLUMN parent_id
;
ALTER TABLE child_table
    DROP COLUMN value
;
ALTER TABLE child_table
    DROP COLUMN remarks
;
";

		var expectQuerySet = DefinitionQuerySetParser.Parse(expect);
		var actualQuerySet = DefinitionQuerySetParser.Parse(actual);

		var queryset = actualQuerySet.GenerateMigrationQuery(expectQuerySet);
		var sql = GetQueryText(queryset);
		Assert.Equal(result, sql, true, true, true);
	}

	[Fact]
	public void TypeChange()
	{
		var expect = @"CREATE TABLE child_table (
    child_id SERIAL PRIMARY KEY,
    child_name VARCHAR(200) NOT NULL UNIQUE,
    parent_id BIGINT NOT NULL,
    value INT NOT NULL CHECK (value >= 0),
    remarks TEXT DEFAULT ''
)
";

		var actual = @"CREATE TABLE child_table (
    child_id SERIAL PRIMARY KEY,
    child_name VARCHAR(100) NOT NULL UNIQUE,
    parent_id INT NOT NULL,
    value INT NOT NULL CHECK (value >= 0),
    remarks TEXT DEFAULT ''
)
";

		var result = @"ALTER TABLE child_table
    ALTER COLUMN child_name TYPE VARCHAR(200)
;
ALTER TABLE child_table
    ALTER COLUMN parent_id TYPE BIGINT
;
";

		var expectQuerySet = DefinitionQuerySetParser.Parse(expect);
		var actualQuerySet = DefinitionQuerySetParser.Parse(actual);

		var queryset = actualQuerySet.GenerateMigrationQuery(expectQuerySet);
		var sql = GetQueryText(queryset);
		Assert.Equal(result, sql, true, true, true);
	}

	[Fact]
	public void NullableChange()
	{
		var expect = @"CREATE TABLE child_table (
    child_id SERIAL PRIMARY KEY,
    child_name VARCHAR(100) NOT NULL UNIQUE,
    parent_id INT NOT NULL,
    value INT NOT NULL CHECK (value >= 0),
    remarks TEXT DEFAULT ''
)
";

		var actual = @"CREATE TABLE child_table (
    child_id SERIAL PRIMARY KEY,
    child_name VARCHAR(100) NOT NULL UNIQUE,
    parent_id INT,
    value INT NOT NULL CHECK (value >= 0),
    remarks TEXT NOT NULL DEFAULT ''
)
";

		var result = @"ALTER TABLE child_table
    ALTER COLUMN parent_id SET NOT NULL
;
ALTER TABLE child_table
    ALTER COLUMN remarks DROP NOT NULL
;
";

		var expectQuerySet = DefinitionQuerySetParser.Parse(expect);
		var actualQuerySet = DefinitionQuerySetParser.Parse(actual);

		var queryset = actualQuerySet.GenerateMigrationQuery(expectQuerySet);
		var sql = GetQueryText(queryset);
		Assert.Equal(result, sql, true, true, true);
	}

	[Fact]
	public void DefaultChange()
	{
		var expect = @"CREATE TABLE child_table (
    child_id SERIAL PRIMARY KEY,
    child_name VARCHAR(100) NOT NULL UNIQUE,
    parent_id INT NOT NULL DEFAULT 0,
    value INT NOT NULL CHECK (value >= 0),
    remarks TEXT NOT NULL
)
";

		var actual = @"CREATE TABLE child_table (
    child_id SERIAL PRIMARY KEY,
    child_name VARCHAR(100) NOT NULL UNIQUE,
    parent_id INT NOT NULL ,
    value INT NOT NULL CHECK (value >= 0),
    remarks TEXT NOT NULL DEFAULT ''
)
";

		var result = @"ALTER TABLE child_table
    ALTER COLUMN parent_id SET DEFAULT 0
;
ALTER TABLE child_table
    ALTER COLUMN remarks DROP DEFAULT
;
";

		var expectQuerySet = DefinitionQuerySetParser.Parse(expect);
		var actualQuerySet = DefinitionQuerySetParser.Parse(actual);

		var queryset = actualQuerySet.GenerateMigrationQuery(expectQuerySet);
		var sql = GetQueryText(queryset);
		Assert.Equal(result, sql, true, true, true);
	}

	[Fact]
	public void AddConstraint()
	{
		var expect = @"CREATE TABLE child_table (
    child_id SERIAL PRIMARY KEY,
    child_name VARCHAR(100) NOT NULL,
    parent_id INT NOT NULL,
    value INT NOT NULL CHECK (value >= 0),
    remarks TEXT DEFAULT ''
)
;
ALTER TABLE child_table
    ADD CONSTRAINT child_name_unique UNIQUE (child_name)
;
";

		var actual = @"CREATE TABLE child_table (
    child_id SERIAL PRIMARY KEY,
    child_name VARCHAR(100) NOT NULL,
    parent_id INT NOT NULL,
    value INT NOT NULL CHECK (value >= 0),
    remarks TEXT DEFAULT ''
)
";

		var result = @"ALTER TABLE child_table
    ADD CONSTRAINT child_name_unique UNIQUE (child_name)
;
";

		var expectQuerySet = DefinitionQuerySetParser.Parse(expect);
		var actualQuerySet = DefinitionQuerySetParser.Parse(actual);

		var queryset = actualQuerySet.GenerateMigrationQuery(expectQuerySet);
		var sql = GetQueryText(queryset);
		Assert.Equal(result, sql, true, true, true);
	}

	[Fact]
	public void DropConstraint()
	{
		var expect = @"CREATE TABLE child_table (
    child_id SERIAL PRIMARY KEY,
    child_name VARCHAR(100) NOT NULL,
    parent_id INT NOT NULL,
    value INT NOT NULL CHECK (value >= 0),
    remarks TEXT DEFAULT ''
)
";

		var actual = @"CREATE TABLE child_table (
    child_id SERIAL PRIMARY KEY,
    child_name VARCHAR(100) NOT NULL,
    parent_id INT NOT NULL,
    value INT NOT NULL CHECK (value >= 0),
    remarks TEXT DEFAULT ''
)
;
ALTER TABLE child_table
    ADD CONSTRAINT child_name_unique UNIQUE (child_name)
;
";

		var result = @"ALTER TABLE child_table
    DROP CONSTRAINT child_name_unique
;
";

		var expectQuerySet = DefinitionQuerySetParser.Parse(expect);
		var actualQuerySet = DefinitionQuerySetParser.Parse(actual);

		var queryset = actualQuerySet.GenerateMigrationQuery(expectQuerySet);
		var sql = GetQueryText(queryset);
		Assert.Equal(result, sql, true, true, true);
	}

	[Fact]
	public void CrateIndex()
	{
		var expect = @"CREATE TABLE public.child_table (
    parent_id int4 NOT NULL
)
;
CREATE INDEX idx_parent_id ON public.child_table (parent_id)
;
";

		var actual = @"CREATE TABLE public.child_table (
    parent_id int4 NOT NULL
)
";

		var result = @"CREATE INDEX idx_parent_id ON public.child_table (
    parent_id
)
;
";

		var expectQuerySet = DefinitionQuerySetParser.Parse(expect);
		var actualQuerySet = DefinitionQuerySetParser.Parse(actual);

		var queryset = actualQuerySet.GenerateMigrationQuery(expectQuerySet);
		var sql = GetQueryText(queryset);
		Assert.Equal(result, sql, true, true, true);
	}

	[Fact]
	public void DropIndex()
	{
		var expect = @"CREATE TABLE public.child_table (
    parent_id int4 NOT NULL
)
;
";

		var actual = @"CREATE TABLE public.child_table (
    parent_id int4 NOT NULL
)
;
CREATE INDEX idx_parent_id ON public.child_table (parent_id)
;
";

		var result = @"DROP INDEX idx_parent_id
;
";

		var expectQuerySet = DefinitionQuerySetParser.Parse(expect);
		var actualQuerySet = DefinitionQuerySetParser.Parse(actual);

		var queryset = actualQuerySet.GenerateMigrationQuery(expectQuerySet);
		var sql = GetQueryText(queryset);
		Assert.Equal(result, sql, true, true, true);
	}
}