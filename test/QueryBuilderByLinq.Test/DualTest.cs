using Carbunql;
using Xunit.Abstractions;
using static QueryBuilderByLinq.Sql;

namespace QueryBuilderByLinq.Test;

public class DualTest
{
	private readonly QueryCommandMonitor Monitor;

	public DualTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
		Output = output;
	}

	private ITestOutputHelper Output { get; set; }

	[Fact]
	public void ConstValue()
	{
		var query = from a in Dual()
					select new
					{
						v1 = 1,
						v2 = "a",
						v3 = new DateTime(2001, 2, 3),
						v4 = DateTime.Now,
						v5 = true,
						v6 = 3.14,
						v7 = 1 + 2 + 3
					};
		var sq = query.ToQueryAsPostgres();

		Monitor.Log(sq);

		var sql = @"
SELECT
    1 AS v1,
    'a' AS v2,
    CAST('2001/02/03 0:00:00' AS timestamp) AS v3,
    current_timestamp AS v4,
    True AS v5,
    3.14 AS v6,
    6 AS v7";

		Assert.Equal(33, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	public record struct table_a(int a_id, string text, int value);
}