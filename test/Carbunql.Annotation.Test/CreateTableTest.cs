using Carbunql.Annotations;
using Xunit.Abstractions;

namespace Carbunql.Annotation.Test;

public class CreateTableTest
{
    public CreateTableTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
        Output = output;
    }
    private QueryCommandMonitor Monitor { get; }

    private ITestOutputHelper Output { get; }

    [Fact]
    public void NullableStringIsNotDbNullable()
    {
        // Even if the property type is defined as string?, the DB type does not allow NULL.
        var nullable = typeof(table_row).GetProperty(nameof(table_row.nullable_string_value))!.IsDbNullable();
        Assert.False(nullable);
    }

    [Fact]
    public void ClauseTest()
    {
        var clause = TableDefinitionClauseFactory.Create<table_row>();

        var tokens = clause.GetTokens(null);

        Monitor.Log(tokens);

        Assert.Equal(74, tokens.Count());
    }

    [Fact]
    public void TableDefinitionClauseFactoryTest()
    {
        var clause = TableDefinitionClauseFactory.Create<table_row>();
        var q = new CreateTableQuery(clause);

        Monitor.Log(q);

        var expect = @"CREATE TABLE table_row (
    table_row_id bigserial NOT NULL,
    string_value text NOT NULL,
    nullable_string_value text NOT NULL,
    int_value integer NOT NULL,
    nullabe_int_value integer,
    long_value bigint NOT NULL,
    nullable_long_value bigint,
    decimal_value numeric NOT NULL,
    nullable_decimal_value numeric,
    date_value timestamp NOT NULL,
    nullable_date_value timestamp,
    bool_value boolean NOT NULL,
    nullable_bool_value boolean,
    readonly_value text NOT NULL,
    writeonly_value text NOT NULL,
    snake_case_test text NOT NULL,
    manual_mapping_column VARCHAR(20) NOT NULL,
    CONSTRAINT table_row_pkey PRIMARY KEY (table_row_id)
)";

        Assert.Equal(expect, q.ToText(), true, true, true);
    }

    [Fact]
    public void CreateTableQueryFactoryTest()
    {
        var q = CreateTableQueryFactory.Create<table_row>();

        Monitor.Log(q);

        var expect = @"CREATE TABLE table_row (
    table_row_id bigserial NOT NULL,
    string_value text NOT NULL,
    nullable_string_value text NOT NULL,
    int_value integer NOT NULL,
    nullabe_int_value integer,
    long_value bigint NOT NULL,
    nullable_long_value bigint,
    decimal_value numeric NOT NULL,
    nullable_decimal_value numeric,
    date_value timestamp NOT NULL,
    nullable_date_value timestamp,
    bool_value boolean NOT NULL,
    nullable_bool_value boolean,
    readonly_value text NOT NULL,
    writeonly_value text NOT NULL,
    snake_case_test text NOT NULL,
    manual_mapping_column VARCHAR(20) NOT NULL,
    CONSTRAINT table_row_pkey PRIMARY KEY (table_row_id)
)";

        Assert.Equal(expect, q.ToText(), true, true, true);
    }

    public record struct table_row
    {
        public long table_row_id { get; set; }

        public string string_value { get; set; }

        public string? nullable_string_value { get; set; }

        public int int_value { get; set; }

        public int? nullabe_int_value { get; set; }

        public long long_value { get; set; }

        public long? nullable_long_value { get; set; }

        public decimal decimal_value { get; set; }

        public decimal? nullable_decimal_value { get; set; }

        public DateTime date_value { get; set; }

        public DateTime? nullable_date_value { get; set; }

        public bool bool_value { get; set; }

        public bool? nullable_bool_value { get; set; }

        public string readonly_value { get; }

        public string writeonly_value { set; private get; }

        public string SnakeCaseTest { get; set; }

        [Column(ColumnName = "manual_mapping_column", ColumnType = "varchar(20)")]
        public string manual_mapping_value { get; set; }

        [IgnoreMapping]
        public string ignore_mapping_value { get; set; }
    }
}