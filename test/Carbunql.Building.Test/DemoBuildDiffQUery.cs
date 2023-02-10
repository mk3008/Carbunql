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
				Output.WriteLine($"    {prm.Key} = {prm.Value}");
			}
			Output.WriteLine("*/");
		}
		Output.WriteLine(cmd.CommandText);
	}

	[Fact]
	public void Build()
	{
		var sql1 = "select id, name, value1, value2, value3 from system_a.table";
		var sql2 = "select id, name, value1, value2, value3 from system_b.table";

		var builder = new DiffQueryBuilder();

		var sq = builder.Execute(sql1, sql2, new[] { "id", "name" });

		DebugPrint(sq.ToCommand());
	}
}