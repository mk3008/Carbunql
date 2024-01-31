using Carbunql.Analysis;
using Carbunql.Clauses;
using Carbunql.Values;
using Xunit.Abstractions;

namespace Carbunql.Building.Test;

public class Demo
{
	public Demo(ITestOutputHelper output)
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
	public void BuildSelectQuery()
	{
		var sq = new SelectQuery();

		// from clause
		var (from, a) = sq.From("table_a").As("a");
		var b = from.InnerJoin("table_b").As("b").On(a, "table_a_id");
		var c = from.LeftJoin("table_c").As("c").On(b, "table_b_id");

		// select clause
		sq.Select(a, "id").As("a_id");
		sq.Select(b, "table_a_id").As("b_id");

		// where clause
		sq.Where(a, "id").Equal(":id").And(b, "is_visible").True().And(c, "table_b_id").IsNull();

		// parameter
		sq.Parameters.Add(new QueryParameter(":id", 1));

		var cmd = sq.ToCommand();
		DebugPrint(cmd);
		/*
			:id = 1
		*/
		/*
		SELECT
			a.id AS a_id,
			b.table_a_id AS b_id
		FROM

			table_a AS a
			INNER JOIN table_b AS b ON a.table_a_id = b.table_a_id

			LEFT JOIN table_c AS c ON b.table_b_id = c.table_b_id
		WHERE

			a.id = :id
			AND b.is_visible = true
			AND c.table_b_id IS null
		*/
	}

	[Fact]
	public void BuildSubQuery()
	{
		var sq = new SelectQuery();
		sq.From(() =>
		{
			var x = new SelectQuery();
			x.From("table_a").As("a");
			x.SelectAll();
			return x;
		}).As("b");
		sq.SelectAll();

		var cmd = sq.ToCommand();
		DebugPrint(cmd);
		/*
		SELECT
			*
		FROM
			(
				SELECT
					*
				FROM
					table_a AS a
			) AS b
		*/
	}

	[Fact]
	public void BuildConditionGroup()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.From("table_a").As("a");
		sq.SelectAll();

		sq.Where(() =>
		{
			// a.id = 1 and a.value = 2
			var c1 = new ColumnValue(a, "id").Equal(1);
			c1.And(() => new ColumnValue(a, "value").Equal(2));

			// a.value = 3 and a.value = 4
			var c2 = new ColumnValue(a, "id").Equal(3);
			c2.And(() => new ColumnValue(a, "value").Equal(4));

			// (
			//     (a.id = 1 and a.value = 2)
			//     or
			//     (a.value = 3 and a.value = 4)
			// )
			return c1.ToGroup().Or(c2.ToGroup()).ToGroup();
		});

		var cmd = sq.ToCommand();
		DebugPrint(cmd);
		/*
		SELECT
			*
		FROM
			table_a AS a
		WHERE
			((a.id = 1 AND a.value = 2) OR (a.id = 3 AND a.value = 4))
		*/
	}

	[Fact]
	public void BuildExistsCondition()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.From("table_a").As("a");
		sq.SelectAll();
		sq.Where(() =>
		{
			var x = new SelectQuery();
			var (_, b) = x.From("table_b").As("b");
			x.SelectAll();
			x.Where(b, "id").Equal(a, "id");
			return x.ToExists();
		});
		sq.Where(() =>
		{
			var x = new SelectQuery();
			var (_, b) = x.From("table_b").As("b");
			x.SelectAll();
			x.Where(b, "id").Equal(a, "id");
			return x.ToNotExists();
		});

		var cmd = sq.ToCommand();
		DebugPrint(cmd);
		/*
		SELECT
			*
		FROM
			table_a AS a
		WHERE
			EXISTS (
				SELECT
					*
				FROM
					table_b AS b
				WHERE
					b.id = a.id
			)
			AND NOT EXISTS (
				SELECT
					*
				FROM
					table_b AS b
				WHERE
					b.id = a.id
			)
		*/
	}

	[Fact]
	public void BuildCTEQuery()
	{
		var cq = new SelectQuery();

		// a as (select * from table_a)
		var ct_a = cq.With(() =>
		{
			var sq = new SelectQuery();
			sq.From("table_a");
			sq.SelectAll();
			return sq;
		}).As("a");

		// b as (select * from table_b)
		var ct_b = cq.With(() =>
		{
			var sq = new SelectQuery();
			sq.From("table_b");
			sq.SelectAll();
			return sq;
		}).As("b");

		// get select query
		var sq = cq.GetOrNewSelectQuery();

		// select * from a iner join b a.id = b.id
		var (from, a) = sq.From(ct_a).As("a");
		from.InnerJoin(ct_b).On(a, "id");

		sq.SelectAll();

		var cmd = cq.ToCommand();
		DebugPrint(cmd);
		/*
		WITH
			a AS (
				SELECT
					*
				FROM
					table_a
			),
			b AS (
				SELECT
					*
				FROM
					table_b
			)
		SELECT
			*
		FROM
			a
			INNER JOIN b ON a.id = b.id
		*/
	}
}