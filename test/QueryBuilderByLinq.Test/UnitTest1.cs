using Carbunql;
using Xunit.Abstractions;

namespace QueryBuilderByLinq.Test;

public class UnitTest1
{
	private readonly QueryCommandMonitor Monitor;

	public UnitTest1(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
		Output = output;
	}

	private ITestOutputHelper Output { get; set; }

	[Fact]
	public void SelectFrom()
	{
		var query = from a in new List<table_a>().AsQueryable() select a.a_id;
		var sq = query.Expression.ToQueryAsPostgres();

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
	public void SelectAllFrom()
	{
		var query = from a in new List<table_a>().AsQueryable() select a;
		var sq = query.Expression.ToQueryAsPostgres();

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
	public void SelectColumsFrom()
	{
		var query = from a in new List<table_a>().AsQueryable()
					select new
					{
						a.a_id,
						a.text
					};
		var sq = query.Expression.ToQueryAsPostgres();

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
	public void SelectAliasColumsFrom()
	{
		var query = from a in new List<table_a>().AsQueryable()
					select new
					{
						ID = a.a_id,
						Text = a.text
					};
		var sq = query.Expression.ToQueryAsPostgres();

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
	public void SelectFromWhere()
	{
		var query = from a in new List<table_a>().AsQueryable() where a.a_id == 1 select a.text;
		var exp = query.Expression;
		SelectQuery sq = exp.ToQueryAsPostgres();

		Monitor.Log(sq);

		var sql = @"
SELECT
    a.text
FROM
    table_a AS a
WHERE
    a.a_id = 1";

		Assert.Equal(14, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void SelectAllWhere()
	{
		var query = from a in new List<table_a>().AsQueryable() where a.a_id == 1 select a;
		var exp = query.Expression;
		SelectQuery sq = exp.ToQueryAsPostgres();

		Monitor.Log(sq);

		var sql = @"
SELECT
    a.a_id,
    a.text,
    a.value
FROM
    table_a AS a
WHERE
    a.a_id = 1";

		Assert.Equal(22, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	public record struct table_a(int a_id, string text, int value);
}