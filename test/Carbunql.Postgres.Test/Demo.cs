using Carbunql.Building;
using Carbunql.Clauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Carbunql.Postgres.Test;

public class Demo
{
	private readonly QueryCommandMonitor Monitor;

	public Demo(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
		Output = output;
	}

	private ITestOutputHelper Output { get; set; }

	[Fact]
	public void Test()
	{
		// Define an empty select query
		SelectQuery sq = new SelectQuery();

		// Specifies the table to select.
		// Note: Make sure the SQL table alias name and variable name are the same
		(FromClause from, table_a a) = sq.FromAs<table_a>("a");

		// Write the table join expression.
		// Combined expressions can be written in a type-safe manner.
		// Note: Make sure that the join destination table alias name and return value variable name are the same.
		table_b b = from.InnerJoinAs<table_b>("b").On(b => a.a_id == b.a_id);

		// Describe the columns to select.
		// If you want to get all columns, use the SelectAll method
		sq.SelectAll(() => a);

		// Use the Select method to select a specific column.
		// You can also give it an alias using the As method.
		sq.Select(() => b.b_id).As("table_b_key");

		// A similar description can be made for the Where clause.
		sq.Where(() => a.a_id == 1);

		// Get SQL string
		string sql = sq.ToCommand().CommandText;

		Output.WriteLine(sql);
	}

	public record struct table_a(int a_id, string text, int value);

	public record struct table_b(int a_id, int b_id, string text, int value);
}
