using Carbunql.Building;
using Carbunql.Values;
using Xunit.Abstractions;

namespace Carbunql.DynamicQuery.Test;

public class UnitTest1
{
	public UnitTest1(ITestOutputHelper output)
	{
		Output = output;
	}

	private readonly ITestOutputHelper Output;

	[Theory]
	[InlineData(1)]
	public void AddCondition(int? parameter)
	{
		//sql parse
		var sq = new SelectQuery(@"select a.a_id, a.val from table as a");

		//get FROM table
		var a = sq.FromClause!.Root;

		//add WHERE clause
		if (parameter != null)
		{
			sq.Where(a, "val").Equal(":prm");
			sq.Parameters.Add(new QueryParameter(":prm", parameter));
		}

		var printer = new DebugPrinter(Output);
		var actual = printer.Write(sq);
		var expect =
@"/*
    :prm = 1
*/
SELECT
    a.a_id,
    a.val
FROM
    table AS a
WHERE
    a.val = :prm
".Replace("\r\n", "\n");

		Assert.Equal(expect, actual);
	}

	[Theory]
	[InlineData(1)]
	public void AddCondition_ValueParse(int? parameter)
	{
		//sql parse
		var sq = new SelectQuery(@"select a.a_id, a.val from table as a");

		//get FROM table
		var a = sq.FromClause!.Root;

		//add WHERE clause
		if (parameter != null)
		{
			sq.Where("a.val = :prm");
			sq.Parameters.Add(new QueryParameter(":prm", parameter));
		}

		var printer = new DebugPrinter(Output);
		var actual = printer.Write(sq);
		var expect =
@"/*
    :prm = 1
*/
SELECT
    a.a_id,
    a.val
FROM
    table AS a
WHERE
    a.val = :prm
".Replace("\r\n", "\n");

		Assert.Equal(expect, actual);
	}

	[Fact]
	public void AddConditions()
	{
		//sql parse
		var sq = new SelectQuery(@"select a.a_id, a.val1, a.val2 from table as a");

		//get FROM table
		var a = sq.FromClause!.Root;

		//add WHERE clause
		sq.Where(a, "val1").Equal(":prm1");
		sq.Where(a, "val2").Equal(":prm2");

		var printer = new DebugPrinter(Output);
		var actual = printer.Write(sq);
		var expect =
@"SELECT
    a.a_id,
    a.val1,
    a.val2
FROM
    table AS a
WHERE
    a.val1 = :prm1
    AND a.val2 = :prm2
".Replace("\r\n", "\n");

		Assert.Equal(expect, actual);
	}

	[Theory]
	[InlineData(1, 2)]
	public void AddOrCondition(int prm1, int prm2)
	{
		//sql parse
		var sq = new SelectQuery(@"select a.a_id, a.val from table as a");

		//get FROM table
		var a = sq.FromClause!.Root;

		//add WHERE clause
		sq.Where(() =>
		{
			sq.Parameters.Add(new QueryParameter(":prm1", prm1));
			sq.Parameters.Add(new QueryParameter(":prm2", prm2));

			//(a.val = :prm1)
			var v1 = new ColumnValue(a, "val").Equal(":prm1").ToGroup();
			//(a.val = :prm2)
			var v2 = new ColumnValue(a, "val").Equal(":prm2").ToGroup();

			//((a.val = :prm1) or (a.val = :prm2))
			return v1.Or(v2).ToGroup();
		});

		var printer = new DebugPrinter(Output);
		var actual = printer.Write(sq);
		var expect =
@"/*
    :prm1 = 1
    :prm2 = 2
*/
SELECT
    a.a_id,
    a.val
FROM
    table AS a
WHERE
    ((a.val = :prm1) OR (a.val = :prm2))
".Replace("\r\n", "\n");

		Assert.Equal(expect, actual);
	}

	[Theory]
	[InlineData("a_id", "val3")]
	public void SelectColumnFilter(params string[] columns)
	{
		//sql parse
		var sq = new SelectQuery(@"select a.a_id, a.val1, a.val2, a.val2 as val3 from table as a");

		//get FROM table
		var a = sq.FromClause!.Root;

		//get not contains columns
		var lst = sq.SelectClause!.Where(x => !columns.Contains(x.Alias)).ToList();

		//remove columns
		foreach (var item in lst) sq.SelectClause!.Remove(item);

		var printer = new DebugPrinter(Output);
		var actual = printer.Write(sq);
		var expect =
@"SELECT
    a.a_id,
    a.val2 AS val3
FROM
    table AS a
".Replace("\r\n", "\n");

		Assert.Equal(expect, actual);
	}

	[Theory]
	[InlineData("a_id", "val3")]
	public void SelectColumnFilter_Remove(params string[] columns)
	{
		//sql parse
		var sq = new SelectQuery(@"select a.a_id, a.val1, a.val2, a.val2 as val3 from table as a");

		//filter in
		sq.SelectClause!.FilterInColumns(columns);

		var printer = new DebugPrinter(Output);
		var actual = printer.Write(sq);
		var expect =
@"SELECT
    a.a_id,
    a.val2 AS val3
FROM
    table AS a
".Replace("\r\n", "\n");

		Assert.Equal(expect, actual);
	}

	[Fact()]
	public void AddOutJoinFilter()
	{
		//sql parse
		var sq = new SelectQuery(@"select a.a_id, a.val from table as a");

		//get FROM table
		var a = sq.FromClause!.Root;

		//add left-join, and get joined table
		var b = sq.FromClause!.LeftJoin("data").As("b").On(a, "a_id");

		//add WHERE clause
		sq.Where(b, "a_id").IsNull();

		var printer = new DebugPrinter(Output);
		var actual = printer.Write(sq);
		var expect =
@"SELECT
    a.a_id,
    a.val
FROM
    table AS a
    LEFT JOIN data AS b ON a.a_id = b.a_id
WHERE
    b.a_id IS null
".Replace("\r\n", "\n");

		Assert.Equal(expect, actual);
	}

	[Fact()]
	public void AddConnditionToCommonTableExpression()
	{
		//sql parse
		var sq = new SelectQuery(@"with
cte as (
	select a_id, val from table
)
select
	a.a_id,
	a.val
from
	datasource as a");

		var cte = sq.WithClause!.Where(x => x.Alias == "cte").First();

		// cte SelectQuery
		var cteq = cte.GetSelectQuery();

		//get FROM table
		var a = cteq.FromClause!.Root;

		//add WHERE clause
		cteq.Where(a, "val").Equal(":prm");
		cteq.Parameters.Add(new QueryParameter(":prm", 1));

		var printer = new DebugPrinter(Output);
		var actual = printer.Write(sq);
		var expect =
@"/*
    :prm = 1
*/
WITH
    cte AS (
        SELECT
            a_id,
            val
        FROM
            table
        WHERE
            table.val = :prm
    )
SELECT
    a.a_id,
    a.val
FROM
    datasource AS a
".Replace("\r\n", "\n");

		Assert.Equal(expect, actual);
	}
}