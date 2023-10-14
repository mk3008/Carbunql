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
	public void SelectAllWhere()
	{
		var query = from a in From<table_a>() where a.a_id == 1 select a;
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