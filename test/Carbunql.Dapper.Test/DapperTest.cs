using Xunit.Abstractions;

namespace Carbunql.Dapper.Test;

public class LoadTest : IClassFixture<PostgresDB>
{
	public LoadTest(PostgresDB postgresDB, ITestOutputHelper output)
	{
		PostgresDB = postgresDB;
		Logger = new UnitTestLogger() { Output = output };
	}

	private readonly PostgresDB PostgresDB;

	private readonly UnitTestLogger Logger;

	[Fact]
	public void ExecuteReader_NoParamter()
	{
		using var cn = PostgresDB.ConnectionOpenAsNew(Logger);

		var sql = @"with
data_ds(c1, c2) as (
	values
	(1,2)
	, (3,4)
)
select
	*
from
	data_ds
";

		var sq = new SelectQuery(sql);
		using var r = cn.ExecuteReader(sq);
		var cnt = 0;
		while (r.Read())
		{
			cnt++;
		}
		Assert.Equal(2, cnt);
	}

	[Fact]
	public void ExecuteReader_HasParamter()
	{
		using var cn = PostgresDB.ConnectionOpenAsNew(Logger);

		var sql = @"with
data_ds(c1, c2) as (
	values
	(1,1)
	, (1,2)
	, (2,1)
	, (2,2)
)
select
	*
from
	data_ds
where
	c1 = :val
";

		var sq = new SelectQuery(sql);
		sq.AddParameter(":val", 1);

		using var r = cn.ExecuteReader(sq);
		var cnt = 0;
		while (r.Read())
		{
			cnt++;
		}
		Assert.Equal(2, cnt);
	}
}
