using Carbunql;
using Xunit.Abstractions;
using static QueryBuilderByLinq.Sql;

namespace QueryBuilderByLinq.Test;

public class JoinTest
{
	private readonly QueryCommandMonitor Monitor;

	public JoinTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
		Output = output;
	}

	private ITestOutputHelper Output { get; set; }

	[Fact]
	public void InnerJoin()
	{
		var query = from b in From<table_b>()
					from a in InnerJoin<table_a>(a => b.b_id == a.a_id)
					select new
					{
						a.a_id,
						b.b_id,
						b.text,
						b.value
					};
		var sq = query.ToQueryAsPostgres();

		Monitor.Log(sq);

		var sql = @"
SELECT
    a.a_id,
    b.b_id,
    b.text,
    b.value
FROM
    table_b AS b
    INNER JOIN table_a AS a ON b.b_id = a.a_id";

		Assert.Equal(32, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void InnerJoinWhere()
	{
		var query = from b in From<table_b>()
					from a in InnerJoin<table_a>(a => b.b_id == a.a_id)
					where a.a_id == 1
					select new
					{
						a.a_id,
						b.b_id,
						b.text,
						b.value
					};
		var sq = query.ToQueryAsPostgres();

		Monitor.Log(sq);

		var sql = @"
SELECT
    a.a_id,
    b.b_id,
    b.text,
    b.value
FROM
    table_b AS b
    INNER JOIN table_a AS a ON b.b_id = a.a_id
WHERE
    a.a_id = 1";

		Assert.Equal(38, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void LeftJoin()
	{
		var query = from b in From<table_b>()
					from a in LeftJoin<table_a>(a => b.b_id == a.a_id)
					select new
					{
						a.a_id,
						b.b_id,
						b.text,
						b.value
					};
		var sq = query.ToQueryAsPostgres();

		Monitor.Log(sq);

		var sql = @"
SELECT
    a.a_id,
    b.b_id,
    b.text,
    b.value
FROM
    table_b AS b
    LEFT JOIN table_a AS a ON b.b_id = a.a_id";

		Assert.Equal(32, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void Relations()
	{
		var query = from d in From<table_d>()
					from c in InnerJoin<table_c>(x => d.c_id == x.c_id)
					from b in InnerJoin<table_b>(x => c.b_id == x.b_id)
					from a in LeftJoin<table_a>(x => b.a_id == x.a_id)
					select new
					{
						a.a_id,
						b.b_id,
						c.c_id,
						d.d_id
					};
		var sq = query.ToQueryAsPostgres();

		Monitor.Log(sq);

		var sql = @"
SELECT
    a.a_id,
    b.b_id,
    c.c_id,
    d.d_id
FROM
    table_d AS d
    INNER JOIN table_c AS c ON d.c_id = c.c_id
    INNER JOIN table_b AS b ON c.b_id = b.b_id
    LEFT JOIN table_a AS a ON b.a_id = a.a_id";

		Assert.Equal(56, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void RelationsAll()
	{
		var query = from d in From<table_d>()
					from c in InnerJoin<table_c>(x => d.c_id == x.c_id)
					from b in InnerJoin<table_b>(x => c.b_id == x.b_id)
					from a in LeftJoin<table_a>(x => b.a_id == x.a_id)
					select new
					{
						a,
						b,
						c,
						d
					};
		var sq = query.ToQueryAsPostgres();

		Monitor.Log(sq);

		var sql = @"
SELECT
    a.a_id,
    a.text,
    a.value,
    b.a_id,
    b.b_id,
    b.text,
    b.value,
    c.b_id,
    c.c_id,
    c.text,
    c.value,
    d.c_id,
    d.d_id,
    d.text,
    d.value
FROM
    table_d AS d
    INNER JOIN table_c AS c ON d.c_id = c.c_id
    INNER JOIN table_b AS b ON c.b_id = b.b_id
    LEFT JOIN table_a AS a ON b.a_id = a.a_id";

		Assert.Equal(100, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void RelationsAllWhere()
	{
		var query = from d in From<table_d>()
					from c in InnerJoin<table_c>(x => d.c_id == x.c_id)
					from b in InnerJoin<table_b>(x => c.b_id == x.b_id)
					from a in LeftJoin<table_a>(x => b.a_id == x.a_id)
					where (a.a_id == 1 && b.b_id == 2) || c.text == "text" || d.value != 10
					select new
					{
						a,
						b,
						c,
						d
					};
		var sq = query.ToQueryAsPostgres();

		Monitor.Log(sq);

		var sql = @"
SELECT
    a.a_id,
    a.text,
    a.value,
    b.a_id,
    b.b_id,
    b.text,
    b.value,
    c.b_id,
    c.c_id,
    c.text,
    c.value,
    d.c_id,
    d.d_id,
    d.text,
    d.value
FROM
    table_d AS d
    INNER JOIN table_c AS c ON d.c_id = c.c_id
    INNER JOIN table_b AS b ON c.b_id = b.b_id
    LEFT JOIN table_a AS a ON b.a_id = a.a_id
WHERE
    ((a.a_id = 1 AND b.b_id = 2) OR c.text = 'text' OR d.value <> 10)";

		Assert.Equal(128, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	//	[Fact]
	//	public void CrossJoin()
	//	{
	//		var query = from b in From<table_b>()
	//					from a in CrossJoin<table_a>(a => true)
	//					select new
	//					{
	//						a.a_id,
	//						b.b_id,
	//						b.text,
	//						b.value
	//					};
	//		var sq = query.Expression.ToQueryAsPostgres();

	//		Monitor.Log(sq);

	//		var sql = @"
	//SELECT
	//    a.a_id,
	//    b.b_id,
	//    b.text,
	//    b.value
	//FROM
	//    table_b AS b
	//    CROSS JOIN table_a AS a";

	//		Assert.Equal(24, sq.GetTokens().ToList().Count);
	//		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	//	}

	public record table_a(int a_id, string text, int value);
	public record table_b(int a_id, int b_id, string text, int value);
	public record table_c(int b_id, int c_id, string text, int value);
	public record table_d(int c_id, int d_id, string text, int value);
}
