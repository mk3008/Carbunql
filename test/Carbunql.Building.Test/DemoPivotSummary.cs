﻿using Carbunql.Analysis;
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
				Output.WriteLine($"    {prm.ParameterName} = {prm.Value}");
			}
			Output.WriteLine("*/");
		}
		Output.WriteLine(cmd.CommandText);
	}

	[Fact]
	public void MonthPivot()
	{
		var sql = @"
select 
	t.ym, t.shop_id, t.sales_amount
from
	(
		values
		('2020-01-01'::date, 1, 10),
		('2020-01-01'::date, 2, 30),
		('2020-01-01'::date, 4, 50),
		('2020-02-01'::date, 1, 100),
		('2020-02-01'::date, 2, 60),
		('2020-02-01'::date, 3, 20)
	) t(ym, shop_id, sales_amount)";

		var builder = new MonthPivotQueryBuilder() { StartDate = new DateTime(2020, 1, 1) };
		var sq = builder.Execute(sql, "ym", new List<String> { "shop_id" }, "sales_amount");

		DebugPrint(sq.ToCommand());
	}

	[Fact]
	public void DatePivot()
	{
		var sql = @"
select 
	t.ymd, t.shop_id, t.sales_amount
from
	(
		values
		('2020-01-01'::date, 1, 10),
		('2020-01-01'::date, 2, 30),
		('2020-01-01'::date, 4, 50),
		('2020-01-02'::date, 1, 100),
		('2020-01-03'::date, 2, 60),
		('2020-01-03'::date, 3, 20)
	) t(ymd, shop_id, sales_amount)";

		var builder = new DatePivotQueryBuilder() { Month = new DateTime(2020, 1, 1) };
		var sq = builder.Execute(sql, "ymd", new List<String> { "shop_id" }, "sales_amount");

		DebugPrint(sq.ToCommand());
	}
}