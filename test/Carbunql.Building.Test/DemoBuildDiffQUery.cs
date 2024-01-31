using Carbunql.Analysis;
using Carbunql.Clauses;
using Carbunql.Values;
using Xunit.Abstractions;

namespace Carbunql.Building.Test;

public class DemoBuildDiffQuery
{
	public DemoBuildDiffQuery(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
		Output = output;
	}

	private readonly QueryCommandMonitor Monitor;

	private readonly ITestOutputHelper Output;

	private void DebugPrint(QueryCommand cmd)
	{
		if (cmd.Parameters.Any())
		{
			Output.WriteLine("/*");
			foreach (var prm in cmd.Parameters)
			{
				Output.WriteLine($"    {prm.ParameterName} = {prm.Value}");
			}
			Output.WriteLine("*/");
		}
		Output.WriteLine(cmd.CommandText);
	}

	[Fact]
	public void Build()
	{
		var expectSql = @"
with
	v1 (id, value1, value2, value3) as (
		values
		( 1, null::int, null::int, null::int),
		( 2,         1, null::int, null::int),
		( 3, null::int,         1, null::int),
		( 4, null::int, null::int,         1),

		(11, null::int, null::int, null::int),
		(12, null::int, null::int, null::int),
		(13, null::int, null::int, null::int),
		(14, null::int, null::int, null::int),
		(15, null::int, null::int, null::int),
		(16, null::int, null::int, null::int),

		(21,         0,         0,         0),
		(22,         0,         0,         0),
		(23,         0,         0,         0),
		(24,         0,         0,         0),
		(25,         0,         0,         0),
		(26,         0,         0,         0),

		(99, null::int, null::int, null::int)
)
select 
	id, 
	value1, 
	value2, 
	value3 
from 
	v1";
		var actualSql = @"
with
	v2 (id, value1, value2, value3) as (
		values
		( 1, null::int, null::int, null::int),
		( 2,         1, null::int, null::int),
		( 3, null::int,         1, null::int),
		( 4, null::int, null::int,         1),

		(11,         1, null::int, null::int),
		(12,         1,         1, null::int),
		(13,         1,         1,         1),
		(14, null::int,         1, null::int),
		(15, null::int,         1,         1),
		(16, null::int, null::int,         1),

		(21,         1,         0,         0),
		(22,         1,         1,         0),
		(23,         1,         1,         1),
		(24,         0,         1,         0),
		(25,         0,         1,         1),
		(26,         0,         0,         1),

		(88, null::int, null::int, null::int)
)
select 
	id, 
	value1, 
	value2, 
	value3 
from 
	v2";
		var keys = new[] { "id" };

		var expectsq = new SelectQuery(expectSql);
		var actualsq = new SelectQuery(actualSql);

		var sq = DiffQueryBuilder.Execute(expectsq, actualsq, keys);

		DebugPrint(sq.ToCommand());
	}

}