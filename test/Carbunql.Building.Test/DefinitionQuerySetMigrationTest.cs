using Xunit.Abstractions;

namespace Carbunql.Building.Test;

public class DefinitionQuerySetMigrationTest
{
    public DefinitionQuerySetMigrationTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private ITestOutputHelper Output { get; set; }

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
    ADD COLUMN child_name VARCHAR(100) NOT NULL,
    ADD COLUMN parent_id INT NOT NULL,
    ADD COLUMN value INT NOT NULL,
    ADD COLUMN remarks TEXT DEFAULT '',
    ADD UNIQUE (child_name),
    ADD CHECK (value >= 0)
;
";

        var queryset = MigrationQueryBuilder.Execute(expect, actual);

        var sql = queryset.ToText();
        Output.WriteLine(sql);
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
    DROP COLUMN child_name,
    DROP COLUMN parent_id,
    DROP COLUMN value,
    DROP COLUMN remarks
;
";

        var queryset = MigrationQueryBuilder.Execute(expect, actual);

        var sql = queryset.ToText();
        Output.WriteLine(sql);
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
    ALTER COLUMN child_name TYPE VARCHAR(200),
    ALTER COLUMN parent_id TYPE BIGINT
;
";

        var queryset = MigrationQueryBuilder.Execute(expect, actual);

        var sql = queryset.ToText();
        Output.WriteLine(sql);
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
    ALTER COLUMN parent_id SET NOT NULL,
    ALTER COLUMN remarks DROP NOT NULL
;
";

        var queryset = MigrationQueryBuilder.Execute(expect, actual);

        var sql = queryset.ToText();
        Output.WriteLine(sql);
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
    ALTER COLUMN parent_id SET DEFAULT 0,
    ALTER COLUMN remarks DROP DEFAULT
;
";

        var queryset = MigrationQueryBuilder.Execute(expect, actual);

        var sql = queryset.ToText();
        Output.WriteLine(sql);
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

        var queryset = MigrationQueryBuilder.Execute(expect, actual);

        var sql = queryset.ToText();
        Output.WriteLine(sql);
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

        var queryset = MigrationQueryBuilder.Execute(expect, actual);

        var sql = queryset.ToText();
        Output.WriteLine(sql);
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

        var queryset = MigrationQueryBuilder.Execute(expect, actual);

        var sql = queryset.ToText();
        Output.WriteLine(sql);
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

        var queryset = MigrationQueryBuilder.Execute(expect, actual);

        var sql = queryset.ToText();
        Output.WriteLine(sql);
        Assert.Equal(result, sql, true, true, true);
    }

    [Fact]
    public void DropTable()
    {
        var expect = @"CREATE TABLE child_table (
    child_id SERIAL PRIMARY KEY
)
";

        var actual = @"CREATE TABLE child_table_2 (
    child_id SERIAL PRIMARY KEY,
    child_name VARCHAR(100) NOT NULL UNIQUE,
    parent_id INT NOT NULL,
    value INT NOT NULL CHECK (value >= 0),
    remarks TEXT DEFAULT ''
)
";

        var result1 = @"CREATE TABLE child_table (
    child_id SERIAL
)
;
ALTER TABLE child_table
    ADD PRIMARY KEY (child_id)
;
";

        var result2 = @"DROP TABLE child_table_2
;
CREATE TABLE child_table (
    child_id SERIAL
)
;
ALTER TABLE child_table
    ADD PRIMARY KEY (child_id)
;
";

        var queryset = MigrationQueryBuilder.Execute(expect, actual);

        var sql = queryset.ToText();
        Output.WriteLine(sql);
        Assert.Equal(result1, sql, true, true, true);

        sql = queryset.ToText(includeDropTableQuery: true);
        Output.WriteLine(sql);
        Assert.Equal(result2, sql, true, true, true);
    }
}