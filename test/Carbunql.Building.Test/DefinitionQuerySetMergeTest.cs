using Carbunql.Analysis;
using Xunit.Abstractions;

namespace Carbunql.Building.Test;

public class DefinitionQuerySetMergeTest
{
    public DefinitionQuerySetMergeTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private ITestOutputHelper Output { get; set; }

    [Fact]
    public void Default()
    {
        var text = @"CREATE TABLE child_table (
    child_id SERIAL,
    child_name VARCHAR(100) NOT NULL,
    parent_id INT NOT NULL,
    value INT NOT NULL,
    remarks TEXT DEFAULT ''
)
;
ALTER TABLE child_table
	ADD PRIMARY KEY (child_id)
;
ALTER TABLE child_table
	ADD UNIQUE (child_name)
;
ALTER TABLE child_table
	ADD CHECK (value >= 0)
;
";

        var expect = @"CREATE TABLE child_table (
    child_id SERIAL,
    child_name VARCHAR(100) NOT NULL,
    parent_id INT NOT NULL,
    value INT NOT NULL,
    remarks TEXT DEFAULT ''
)
;
ALTER TABLE child_table
    ADD PRIMARY KEY (child_id),
    ADD UNIQUE (child_name),
    ADD COLUMN CHECK (value >= 0)
;
";

        var v = DefinitionQuerySetParser.Parse(text);
        v.MergeAlterTableQuery();

        var sql = v.ToText();
        Output.WriteLine(sql);
        Assert.Equal(expect, sql, true, true, true);
    }

    [Fact]
    public void AlterTableOnly()
    {
        var text = @"
ALTER TABLE child_table
	ADD PRIMARY KEY (child_id)
;
ALTER TABLE child_table
	ADD UNIQUE (child_name)
;
ALTER TABLE child_table
	ADD CHECK (value >= 0)
;
";

        var expect = @"ALTER TABLE child_table
    ADD PRIMARY KEY (child_id),
    ADD UNIQUE (child_name),
    ADD COLUMN CHECK (value >= 0)
;
";

        var v = DefinitionQuerySetParser.Parse(text);
        v.MergeAlterTableQuery();

        var sql = v.ToText();
        Output.WriteLine(sql);
        Assert.Equal(expect, sql, true, true, true);
    }

    [Fact]
    public void DifferentTable()
    {
        var text = @"
ALTER TABLE child_table_1
	ADD PRIMARY KEY (child_id)
;
ALTER TABLE child_table_2
	ADD UNIQUE (child_name)
;
ALTER TABLE child_table_1
	ADD CHECK (value >= 0)
;
";

        var expect = @"ALTER TABLE child_table_1
    ADD PRIMARY KEY (child_id),
    ADD COLUMN CHECK (value >= 0)
;
ALTER TABLE child_table_2
    ADD UNIQUE (child_name)
;
";

        var v = DefinitionQuerySetParser.Parse(text);
        v.MergeAlterTableQuery();

        var sql = v.ToText();
        Output.WriteLine(sql);
        Assert.Equal(expect, sql, true, true, true);
    }
}