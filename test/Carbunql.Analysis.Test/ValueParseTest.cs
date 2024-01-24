using Carbunql.Analysis.Parser;
using Carbunql.Clauses;
using Carbunql.Values;
using Xunit.Abstractions;

namespace Carbunql.Analysis.Test;

public class ValueParserTest
{
	private readonly QueryCommandMonitor Monitor;

	public ValueParserTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
	}

	[Fact]
	public void Column()
	{
		var text = "col";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Single(lst);
	}

	[Fact]
	public void TableColumn()
	{
		var text = "tbl.col";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(3, lst.Count);
	}

	[Fact]
	public void Numeric()
	{
		var text = "3.14";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Single(lst);
	}

	[Fact]
	public void Numeric_Negative()
	{
		var text = "-1";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		Assert.IsType<LiteralValue>(v);

		var lst = v.GetTokens().ToList();
		Assert.Single(lst);
	}

	[Fact]
	public void Text()
	{
		var text = "'abc''s'";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Single(lst);
	}

	[Fact]
	public void BooleanTrue()
	{
		var text = "true";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Single(lst);
	}

	[Fact]
	public void BooleanFalse()
	{
		var text = "false";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Single(lst);
	}

	[Fact]
	public void Expression()
	{
		var text = "1*3.14";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(3, lst.Count);

		Assert.IsType<LiteralValue>(v);
		var lv = (LiteralValue)v;
		Assert.Equal("1", lv.CommandText);
		Assert.NotNull(v.OperatableValue);
		Assert.Equal("*", v.OperatableValue!.Operator);
		Assert.IsType<LiteralValue>(v.OperatableValue!.Value);
		lv = (LiteralValue)v.OperatableValue!.Value;
		Assert.Equal("3.14", lv.CommandText);
	}

	[Fact]
	public void Expression_Negative()
	{
		var text = "-1*3.14";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(3, lst.Count);

		Assert.IsType<LiteralValue>(v);
		var lv = (LiteralValue)v;
		Assert.Equal("-1", lv.CommandText);
		Assert.NotNull(v.OperatableValue);
		Assert.Equal("*", v.OperatableValue!.Operator);
		Assert.IsType<LiteralValue>(v.OperatableValue!.Value);
		lv = (LiteralValue)v.OperatableValue!.Value;
		Assert.Equal("3.14", lv.CommandText);
	}

	[Fact]
	public void Expression2()
	{
		var text = "tbl.col1 *   tbl.col2";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(7, lst.Count);
	}

	[Fact]
	public void Expression3()
	{
		var text = "(1+1)*2";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(7, lst.Count);
	}

	[Fact]
	public void Expression4()
	{
		var text = "1%1";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(3, lst.Count);
	}

	[Fact]
	public void Expression5()
	{
		var text = "a.val=:parameter";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(5, lst.Count);
	}

	[Fact]
	public void Regex()
	{
		var text = "'a' !~* 'a'";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(3, lst.Count);
	}

	[Fact]
	public void Not()
	{
		var text = "not true";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(2, lst.Count);
	}

	[Fact]
	public void Not2()
	{
		var text = "not (1 + 1 = 1)";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(8, lst.Count);
	}

	[Fact]
	public void WithoutTimeZone()
	{
		//Postgres
		var text = "now() without time zone";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(4, lst.Count);

		Assert.Equal("now() without time zone", v.ToText());
	}

	[Fact]
	public void AtTimeZone()
	{
		var text = "now() at time zone 'Asia/Tokyo'";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(5, lst.Count);
		Assert.Equal("now() at time zone 'Asia/Tokyo'", v.ToText());
	}

	[Fact]
	public void In()
	{
		var text = "1 in (1)";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(5, lst.Count);
	}

	[Fact]
	public void NotIn()
	{
		var text = "1 not in (1)";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(6, lst.Count);
	}

	[Fact]
	public void Function()
	{
		var text = "sum(tbl.col+    tbl.col2)";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(10, lst.Count);
	}

	[Fact]
	public void Function_2()
	{
		var text = "concat('a', 'b')";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(6, lst.Count);

		Assert.IsType<FunctionValue>(v);
		var lv = (FunctionValue)v;
		Assert.Equal("concat", lv.Name);
		Assert.NotNull(lv.Arguments);
	}

	[Fact]
	public void Function_3()
	{
		var text = "concat('a',     '/')";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(6, lst.Count);

		Assert.IsType<FunctionValue>(v);
		var lv = (FunctionValue)v;
		Assert.Equal("concat", lv.Name);
		Assert.NotNull(lv.Arguments);
	}

	[Fact]
	public void Function_Nest()
	{
		var text = "coalesce(min(1), 1)";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(9, lst.Count);
	}

	[Fact]
	public void WindowFunction_PartitionByOrderBy()
	{
		var text = "row_number() over(partition by tbl.col, tbl.col2 order by tbl.col3, tbl.col4)";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(22, lst.Count);
	}

	[Fact]
	public void WindowFunction_OrderBy()
	{
		var text = "row_number() over(order by tbl.col, tbl.col2)";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(14, lst.Count);
	}

	[Fact]
	public void WindowFunction_Argument()
	{
		var text = "sum(d.tax) over (partition by d.tax_rate)";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(13, lst.Count);
	}

	[Fact]
	public void WindowFunction_Filter()
	{
		var text = "sum(value) filter (where v.name = 'a')";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(13, lst.Count);
	}

	[Fact]
	public void CaseExpression()
	{
		var text = "case tbl.col when 1 then 10 when 2 then 20 else 30 end";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(15, lst.Count);
	}

	[Fact]
	public void CaseExpression_Upper()
	{
		var text = "CASE tbl.col WHEN 1 THEN 10 WHEN 2 THEN 20 ELSE 30 END";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(15, lst.Count);
	}

	[Fact]
	public void CaseWhenExpression()
	{
		var text = "case when tbl.col1 = 1 then 10 when tbl.col2 = 2 then 20 else 30 end";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(20, lst.Count);
	}

	[Fact]
	public void InlineQuery()
	{
		var text = "(select a.val from table_a a where a.id = b.table_a_id)";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(18, lst.Count);

		Assert.IsType<InlineQuery>(v);
	}

	[Fact]
	public void ExistsExpression()
	{
		var text = "exists (select * from table_a a where a.id = b.table_a_id)";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(17, lst.Count);

		Assert.IsType<ExistsExpression>(v);
	}

	[Fact]
	public void ExistsExpression_Upper()
	{
		var text = "EXISTS (select * from table_a a where a.id = b.table_a_id)";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(17, lst.Count);

		Assert.IsType<ExistsExpression>(v);
	}

	[Fact]
	public void InExpression()
	{
		var text = "'a' in (select a.id from table_a)";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		Assert.IsType<InClause>(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(10, lst.Count);
	}

	[Fact]
	public void InFunction()
	{
		var text = "in (1, 2, 3)";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		Assert.IsType<FunctionValue>(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(8, lst.Count);
	}

	[Fact]
	public void InFunction_Upper()
	{
		var text = "IN (1, 2, 3)";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(8, lst.Count);

		Assert.IsType<FunctionValue>(v);
	}

	[Fact]
	public void Between()
	{
		var text = "a.id between 1 and 2";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(7, lst.Count);
	}

	[Fact]
	public void NotBetween()
	{
		var text = "a.id not between 1 and 2";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(8, lst.Count);
	}

	[Fact]
	public void Like()
	{
		var text = "a.name like 'test%'";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(5, lst.Count);
	}

	[Fact]
	public void NotLike()
	{
		var text = "a.name not like 'test%'";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(6, lst.Count);
	}

	[Fact]
	public void Bracket()
	{
		var text = "(1)";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(3, lst.Count);
	}

	[Fact]
	public void Bracket_Nest()
	{
		var text = "((1))";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(5, lst.Count);
	}

	[Fact]
	public void Sufix()
	{
		var text = "'2000-01-01'::timestamp";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(3, lst.Count);
	}

	[Fact]
	public void Parameter()
	{
		var text = ":val,";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Single(lst);
		Assert.IsType<ParameterValue>(lst.First().Sender);
	}

	[Fact]
	public void Parameter_SQLServer()
	{
		var text = "@val,";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Single(lst);
		Assert.IsType<ParameterValue>(lst.First().Sender);
	}

	[Fact]
	public void Parameter_Nameless()
	{
		var text = "?,";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Single(lst);
		Assert.IsType<ParameterValue>(lst.First().Sender);
	}

	[Fact]
	public void SufixBracket()
	{
		var text = "'3.14'::numeric(8,2)";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(8, lst.Count);
	}

	[Fact]
	public void Extract()
	{
		var text = "extract(month from '2020-05-01'::date)";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(8, lst.Count);
	}

	[Fact]
	public void Cast()
	{
		var text = "cast('3.14' as numeric(8,2))";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(11, lst.Count);
	}

	[Fact]
	public void JsonFunction_1()
	{
		var text = "'[{\"a\":\"foo\"},{\"b\":\"bar\"},{\"c\":\"baz\"}]'::json->2";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(5, lst.Count);
	}

	[Fact]
	public void JsonFunction_2()
	{
		var text = "'[1,2,3]'::json->>2";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(5, lst.Count);
	}

	[Fact]
	public void JsonFunction_3()
	{
		var text = "'{\"a\": {\"b\":{\"c\": \"foo\"}}}'::json#>'{a,b}'";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(5, lst.Count);
	}

	[Fact]
	public void JsonFunction_4()
	{
		var text = "'{\"a\":[1,2,3],\"b\":[4,5,6]}'::json#>>'{a,2}'";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(5, lst.Count);
	}

	[Fact]
	public void PostgresArray()
	{
		var text = "array[1,2]";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(6, lst.Count);
	}

	[Fact]
	public void MySqlSystemVariable()
	{
		var text = "@@session.time_zone";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(3, lst.Count);

		var expect = "@@session.time_zone";

		Assert.Equal(expect, v.ToText());
	}

	[Fact]
	public void PostgresOperator_1()
	{
		var text = "'((0,0),1)'::circle <-> '((5,0),1)'::circle";

		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(7, lst.Count);

		var expect = "'((0,0),1)'::circle <-> '((5,0),1)'::circle";

		Assert.Equal(expect, v.ToText());
	}
}