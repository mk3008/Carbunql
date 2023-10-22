using Carbunql;
using Xunit.Abstractions;
using static QueryBuilderByLinq.Sql;

namespace QueryBuilderByLinq.Test;

public class CommonTableTest
{
	private readonly QueryCommandMonitor Monitor;

	public CommonTableTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
		Output = output;
	}

	private ITestOutputHelper Output { get; set; }

	[Fact]
	public void DefaultTest()
	{
		var subq = from a in FromTable<table_a>() select new { ID = a.a_id, Text = a.text };

		var query = from cte in CommonTable(subq)
					from b in FromTable<table_a>(nameof(cte))
					where b.a_id == 1
					select b;

		var sq = query.ToQueryAsPostgres();

		Monitor.Log(sq);

		var sql = @"
WITH
    cte AS (
        SELECT
            a.a_id AS ID,
            a.text AS Text
        FROM
            table_a AS a
    )
SELECT 
	b.ID
	, b.Text
FROM
    cte AS b
WHERE
    b.a_id =  1";

		Assert.Equal(39, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	public record struct table_a(int a_id, string text, int value);
}