using Carbunql;
using Xunit.Abstractions;
using static QueryBuilderByLinq.Sql;

namespace QueryBuilderByLinq.Test;

public class SubQueryTest
{
	private readonly QueryCommandMonitor Monitor;

	public SubQueryTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
		Output = output;
	}

	private ITestOutputHelper Output { get; set; }

	[Fact]
	public void DefaultTest()
	{
		var subq = from a in From<table_a>() select new { ID = a.a_id, Text = a.text };
		var query = from x in From(subq) select new { x.ID, x.Text };

		var sq = query.ToQueryAsPostgres();

		Monitor.Log(sq);

		var sql = @"
SELECT
    x.ID,
    x.Text
FROM
    (
        SELECT
            a.a_id AS ID,
            a.text AS Text
        FROM
            table_a AS a
    ) AS x";

		Assert.Equal(29, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	public record struct table_a(int a_id, string text, int value);
}