using Carbunql;
using Xunit.Abstractions;
using static QueryBuilderByLinq.Sql;

namespace QueryBuilderByLinq.Test;

public class WhereTest
{
	private readonly QueryCommandMonitor Monitor;

	public WhereTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
		Output = output;
	}

	private ITestOutputHelper Output { get; set; }

	//	[Fact]
	//	public void SelectColumnWhere()
	//	{
	//		var query = from a in From<table_a>() where a.a_id == 1 select a.text;
	//		var exp = query.Expression;
	//		SelectQuery sq = exp.ToQueryAsPostgres();

	//		Monitor.Log(sq);

	//		var sql = @"
	//SELECT
	//    a.text
	//FROM
	//    table_a AS a
	//WHERE
	//    a.a_id = 1";

	//		Assert.Equal(14, sq.GetTokens().ToList().Count);
	//		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	//	}

	[Fact]
	public void DefaultTest()
	{
		var query = from a in Table<table_a>()
					where a.a_id == 1
					select a;
		SelectQuery sq = query.ToQueryAsPostgres();

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

	[Fact]
	public void AndTest()
	{
		var query = from a in Table<table_a>()
					where a.a_id == 1 && a.text == "test"
					select a;
		SelectQuery sq = query.ToQueryAsPostgres();

		Monitor.Log(sq);

		var sql = @"
SELECT
    a.a_id,
    a.text,
    a.value
FROM
    table_a AS a
WHERE
    (a.a_id = 1 AND a.text = 'test')";

		Assert.Equal(30, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void OrTest()
	{
		var query = from a in Table<table_a>()
					where a.a_id == 1 || a.text == "test"
					select a;
		SelectQuery sq = query.ToQueryAsPostgres();

		Monitor.Log(sq);

		var sql = @"
SELECT
    a.a_id,
    a.text,
    a.value
FROM
    table_a AS a
WHERE
    (a.a_id = 1 OR a.text = 'test')";

		Assert.Equal(30, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void ParameterTest()
	{
		var id = 1;
		var query = from a in Table<table_a>()
					where a.a_id == id
					select a;
		SelectQuery sq = query.ToQueryAsPostgres();

		Monitor.Log(sq);

		var sql = @"
/*
  :member_id = 1
*/
SELECT
    a.a_id,
    a.text,
    a.value
FROM
    table_a AS a
WHERE
    a.a_id = :member_id";

		Assert.Equal(22, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void AnyTest()
	{
		var query = from a in Table<table_a>()
					where new[] { 1, 2, 3 }.Contains(a.a_id)
					select a;
		SelectQuery sq = query.ToQueryAsPostgres();

		Monitor.Log(sq);

		var sql = @"
SELECT
    a.a_id,
    a.text,
    a.value
FROM
    table_a AS a
WHERE
    a.a_id = ANY(ARRAY[1, 2, 3])";

		Assert.Equal(32, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void AnyParameterTest()
	{
		var ids = new[] { 1, 2, 3 };
		var query = from a in Table<table_a>()
					where ids.Contains(a.a_id)
					select a;
		SelectQuery sq = query.ToQueryAsPostgres();

		Monitor.Log(sq);

		var sql = @"
/*
  :member_ids = System.Int32[]
*/
SELECT
    a.a_id,
    a.text,
    a.value
FROM
    table_a AS a
WHERE
    a.a_id = ANY(:member_ids)";

		Assert.Equal(25, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	public record struct table_a(int a_id, string text, int value);
}