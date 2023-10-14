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
	public void SelectAliasColums()
	{
		var query = from b in From<table_b>()
					join a in From<table_a>() on b.a_id equals a.a_id
					select new
					{
						a.a_id,
						b.b_id,
						b.text,
						b.value
					};
		var sq = query.Expression.ToQueryAsPostgres();

		Monitor.Log(sq);

		var sql = @"
SELECT
    a.a_id,
    b.b_id,
    b.text,
    b.value
FROM
    table_b AS b
    INNER JOIN table_a AS a ON b.a_id = a.a_id";

		Assert.Equal(32, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	public record struct table_a(int a_id, string text, int value);
	public record struct table_b(int a_id, int b_id, string text, int value);
}
