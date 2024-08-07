﻿using Carbunql.Analysis;
using Xunit.Abstractions;

namespace Carbunql.Building.Test;

public class DefinitionQuerySetNormalizeTest
{
    public DefinitionQuerySetNormalizeTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private ITestOutputHelper Output { get; set; }

    [Fact]
    public void UnknownNameConstraint()
    {
        var text = @"CREATE TABLE child_table (
    child_id SERIAL PRIMARY KEY,
    child_name VARCHAR(100) NOT NULL UNIQUE,
    parent_id INT NOT NULL,
    value INT NOT NULL CHECK (value >= 0),
    remarks TEXT DEFAULT ''
)
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
	ADD PRIMARY KEY (child_id)
;
ALTER TABLE child_table
	ADD UNIQUE (child_name)
;
ALTER TABLE child_table
	ADD CHECK (value >= 0)
;
";

        var v = DefinitionQuerySetParser.Parse(text);
        var normal = v.ToNormalize(doMergeAltarTablerQuery: false);

        var sql = normal.ToText();
        Output.WriteLine(sql);
        Assert.Equal(expect, sql, true, true, true);
    }

    [Fact]
    public void NamedConstraint()
    {
        var text = @"CREATE TABLE public.child_table (
    child_id serial4 NOT NULL,
    child_name varchar(100) NOT NULL,
    parent_id int4 NOT NULL,
    value int4 NOT NULL,
    remarks text NULL DEFAULT ''::text,
    CONSTRAINT child_table_pkey PRIMARY KEY (child_id),
	CONSTRAINT child_table_child_name_key UNIQUE (child_name),
    CONSTRAINT child_table_value_check CHECK (value >= 0)
)
;
";

        var expect = @"CREATE TABLE public.child_table (
    child_id serial4 NOT NULL,
    child_name VARCHAR(100) NOT NULL,
    parent_id int4 NOT NULL,
    value int4 NOT NULL,
    remarks text NOT NULL DEFAULT ''::text
)
;
ALTER TABLE public.child_table
	ADD CONSTRAINT child_table_pkey PRIMARY KEY (child_id)
;
ALTER TABLE public.child_table
	ADD CONSTRAINT child_table_child_name_key UNIQUE (child_name)
;
ALTER TABLE public.child_table
	ADD CONSTRAINT child_table_value_check CHECK (value >= 0)
;
";

        var v = DefinitionQuerySetParser.Parse(text);
        var normal = v.ToNormalize(doMergeAltarTablerQuery: false);

        var sql = normal.ToText();
        Output.WriteLine(sql);
        Assert.Equal(expect, sql, true, true, true);
    }

    [Fact]
    public void IntegrateNotNull()
    {
        var text = @"CREATE TABLE public.child_table (
    child_id serial4 PRIMARY KEY,
    child_name varchar(100),
    parent_id int4,
    value int4,
    remarks text DEFAULT ''::text
)
;
ALTER TABLE public.child_table
	ALTER COLUMN child_id SET NOT NULL,
	ALTER COLUMN child_name SET NOT NULL,
	ALTER COLUMN parent_id SET NOT NULL,
	ALTER COLUMN value SET NOT NULL,
	ALTER COLUMN remarks SET NOT NULL
;
";

        var expect = @"CREATE TABLE public.child_table (
    child_id serial4 NOT NULL,
    child_name VARCHAR(100) NOT NULL,
    parent_id int4 NOT NULL,
    value int4 NOT NULL,
    remarks text NOT NULL DEFAULT ''::text
)
;
ALTER TABLE public.child_table
    ADD PRIMARY KEY (child_id)
;
";

        var v = DefinitionQuerySetParser.Parse(text);
        var normal = v.ToNormalize();

        var sql = normal.ToText();
        Output.WriteLine(sql);
        Assert.Equal(expect, sql, true, true, true);
    }

    [Fact]
    public void IntegrateAddColumn()
    {
        var text = @"CREATE TABLE public.child_table (
    child_id serial4 PRIMARY KEY
)
;
ALTER TABLE public.child_table
	ADD COLUMN child_name varchar(100) not null,
	ADD COLUMN parent_id int4 not null,
	ADD COLUMN value int4 not null,
	ADD COLUMN remarks text not null DEFAULT ''::text
;
";

        var expect = @"CREATE TABLE public.child_table (
    child_id serial4,
    child_name VARCHAR(100) NOT NULL,
    parent_id int4 NOT NULL,
    value int4 NOT NULL,
    remarks text NOT NULL DEFAULT ''::text
)
;
ALTER TABLE public.child_table
    ADD PRIMARY KEY (child_id)
;
";

        var v = DefinitionQuerySetParser.Parse(text);
        var normal = v.ToNormalize();

        var sql = normal.ToText();
        Output.WriteLine(sql);
        Assert.Equal(expect, sql, true, true, true);
    }

    [Fact]
    public void IntegrateDropColumn()
    {
        var text = @"CREATE TABLE public.child_table (
    child_id serial4 PRIMARY KEY,
    child_name varchar(100),
    parent_id int4,
    value int4,
    remarks text DEFAULT ''::text
)
;
ALTER TABLE public.child_table
	DROP COLUMN remarks
;
";

        var expect = @"CREATE TABLE public.child_table (
    child_id serial4,
    child_name VARCHAR(100),
    parent_id int4,
    value int4
)
;
ALTER TABLE public.child_table
    ADD PRIMARY KEY (child_id)
;
";

        var v = DefinitionQuerySetParser.Parse(text);
        var normal = v.ToNormalize();

        var sql = normal.ToText();
        Output.WriteLine(sql);
        Assert.Equal(expect, sql, true, true, true);
    }

    [Fact]
    public void SetDefault()
    {
        var text = @"CREATE TABLE public.child_table (
    child_id serial4 PRIMARY KEY,
    child_name varchar(100),
    parent_id int4,
    value int4,
    remarks text
)
;
ALTER TABLE public.child_table
	ALTER COLUMN remarks SET DEFAULT ''
;
";

        var expect = @"CREATE TABLE public.child_table (
    child_id serial4,
    child_name VARCHAR(100),
    parent_id int4,
    value int4,
    remarks text DEFAULT ''
)
;
ALTER TABLE public.child_table
    ADD PRIMARY KEY (child_id)
;
";

        var v = DefinitionQuerySetParser.Parse(text);
        var normal = v.ToNormalize();

        var sql = normal.ToText();
        Output.WriteLine(sql);
        Assert.Equal(expect, sql, true, true, true);
    }

    [Fact]
    public void DropDefault()
    {
        var text = @"CREATE TABLE public.child_table (
    child_id serial4 PRIMARY KEY,
    child_name varchar(100),
    parent_id int4,
    value int4,
    remarks text DEFAULT ''
)
;
ALTER TABLE public.child_table
	ALTER COLUMN remarks DROP DEFAULT
;
";

        var expect = @"CREATE TABLE public.child_table (
    child_id serial4,
    child_name VARCHAR(100),
    parent_id int4,
    value int4,
    remarks text
)
;
ALTER TABLE public.child_table
    ADD PRIMARY KEY (child_id)
;
";

        var v = DefinitionQuerySetParser.Parse(text);
        var normal = v.ToNormalize();

        var sql = normal.ToText();
        Output.WriteLine(sql);
        Assert.Equal(expect, sql, true, true, true);
    }

    [Fact]
    public void CraeteUniqueIndex()
    {
        var text = @"CREATE TABLE public.child_table (
    child_id serial4 PRIMARY KEY,
    child_name varchar(100)
)
;
CREATE UNIQUE INDEX idx_child_name ON public.child_table (
    child_name
);
";

        var expect = @"CREATE TABLE public.child_table (
    child_id serial4,
    child_name VARCHAR(100)
)
;
ALTER TABLE public.child_table
    ADD PRIMARY KEY (child_id)
;
CREATE UNIQUE INDEX idx_child_name ON public.child_table (
    child_name
)
;
";

        var v = DefinitionQuerySetParser.Parse(text);
        var normal = v.ToNormalize();

        var sql = normal.ToText();
        Output.WriteLine(sql);
        Assert.Equal(expect, sql, true, true, true);
    }

    [Fact]
    public void AlterTableUnique()
    {
        var text = @"CREATE TABLE public.child_table (
    child_id serial4 PRIMARY KEY,
    child_name varchar(100)
)
;
ALTER TABLE public.child_table
ADD CONSTRAINT idx_child_name UNIQUE (child_name)
;
";

        var expect = @"CREATE TABLE public.child_table (
    child_id serial4,
    child_name VARCHAR(100)
)
;
ALTER TABLE public.child_table
    ADD PRIMARY KEY (child_id),
    ADD CONSTRAINT idx_child_name UNIQUE (child_name)
;
";

        var v = DefinitionQuerySetParser.Parse(text);
        var normal = v.ToNormalize();

        var sql = normal.ToText();
        Output.WriteLine(sql);
        Assert.Equal(expect, sql, true, true, true);
    }
}