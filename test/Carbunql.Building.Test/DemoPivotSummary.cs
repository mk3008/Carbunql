using Carbunql.Analysis;
using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Values;
using Microsoft.VisualStudio.CodeCoverage;
using System;
using Xunit.Abstractions;

namespace Carbunql.Building.Test;

public class DemoPivotSummary
{
	public DemoPivotSummary(ITestOutputHelper output)
	{
		Output = output;
	}

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
		var sql = "select ym, shop_id, shop_name, sales_amout from table_a";

		var builder = new YmPivotQueryBuilder() { StartDate = new DateTime(2020, 1, 1) };
		var sq = builder.Execute(sql, "ym", new List<String> { "ym", "shop_id", "shop_name" }, "sales_amount");

		DebugPrint(sq.ToCommand());
	}
}