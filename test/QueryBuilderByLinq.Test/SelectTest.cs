using Carbunql;
using Xunit.Abstractions;
using static QueryBuilderByLinq.Sql;

namespace QueryBuilderByLinq.Test;

public class SelectTest
{
	private readonly QueryCommandMonitor Monitor;

	public SelectTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
		Output = output;
	}

	private ITestOutputHelper Output { get; set; }

	[Fact]
	public void SelectScalar()
	{
		var query = from a in From<table_a>() select a.a_id;
		var sq = query.ToQueryAsPostgres();

		Monitor.Log(sq);

		var sql = @"
SELECT
    a.a_id
FROM
    table_a AS a";

		Assert.Equal(8, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void SelectAll()
	{
		var query = from a in From<table_a>() select a;
		var sq = query.ToQueryAsPostgres();

		Monitor.Log(sq);

		var sql = @"
SELECT
    a.a_id,
    a.text,
    a.value
FROM
    table_a AS a";

		Assert.Equal(16, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void SelectColums()
	{
		var query = from a in From<table_a>()
					select new
					{
						a.a_id,
						a.text
					};
		var sq = query.ToQueryAsPostgres();

		Monitor.Log(sq);

		var sql = @"
SELECT
    a.a_id,
    a.text
FROM
    table_a AS a";

		Assert.Equal(12, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}


	[Fact]
	public void SelectAliasColums()
	{
		var query = from a in From<table_a>()
					select new
					{
						ID = a.a_id,
						Text = a.text
					};
		var sq = query.ToQueryAsPostgres();

		Monitor.Log(sq);

		var sql = @"
SELECT
    a.a_id AS ID,
    a.text AS Text
FROM
    table_a AS a";

		Assert.Equal(16, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void GreatestTest()
	{
		var query = from a in From<table_a>()
					select new
					{
						val = Greatest(a.a_id, a.value)
					};
		var sq = query.ToQueryAsPostgres();

		Monitor.Log(sq);

		var sql = @"
SELECT
    GREATEST(a.a_id, a.value) AS val
FROM
    table_a AS a";

		Assert.Equal(17, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void LeastTest()
	{
		var query = from a in From<table_a>()
					select new
					{
						val = Least(a.a_id, a.value)
					};
		var sq = query.ToQueryAsPostgres();

		Monitor.Log(sq);

		var sql = @"
SELECT
    LEAST(a.a_id, a.value) AS val
FROM
    table_a AS a";

		Assert.Equal(17, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void RownumberTest()
	{
		var query = from a in From<table_a>()
					select new
					{
						val = RowNumber()
					};
		var sq = query.ToQueryAsPostgres();

		Monitor.Log(sq);

		var sql = @"
SELECT
    ROW_NUMBER() AS val
FROM
    table_a AS a";

		Assert.Equal(10, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void RownumberTest_OrderBy()
	{
		var query = from a in From<table_a>()
					select new
					{
						val = RowNumber(new { a.text, a.a_id })
					};
		var sq = query.ToQueryAsPostgres();

		Monitor.Log(sq);

		var sql = @"
SELECT
    ROW_NUMBER() OVER(
        ORDER BY
            a.text,
            a.a_id
    ) AS val
FROM
    table_a AS a";

		Assert.Equal(21, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void RownumberTest_PartitionBy()
	{
		var query = from a in From<table_a>()
					select new
					{
						val = RowNumber(new { a.text, a.a_id }, null)
					};
		var sq = query.ToQueryAsPostgres();

		Monitor.Log(sq);

		var sql = @"
SELECT
    ROW_NUMBER() OVER(
        PARTITION BY
            a.text,
            a.a_id
    ) AS val
FROM
    table_a AS a";

		Assert.Equal(21, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void RownumberTest_PartitionByOrderBy()
	{
		var query = from a in From<table_a>()
					select new
					{
						val = RowNumber(new { a.text }, new { a.value, a.a_id })
					};
		var sq = query.ToQueryAsPostgres();

		Monitor.Log(sq);

		var sql = @"
SELECT
    ROW_NUMBER() OVER(
        PARTITION BY
            a.text
        ORDER BY
            a.value,
            a.a_id
    ) AS val
FROM
    table_a AS a";

		Assert.Equal(25, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	public record struct table_a(int a_id, string text, int value);
}