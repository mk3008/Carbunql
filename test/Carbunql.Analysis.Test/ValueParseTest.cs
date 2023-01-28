using Carbunql.Analysis.Parser;
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

		//Assert.Equal("col", v.GetCommandText());
		//Assert.Equal("col", v.GetDefaultName());
	}

	[Fact]
	public void TableColumn()
	{
		var text = "tbl.col";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		//Assert.Equal("tbl.col", v.GetCommandText());
		//Assert.Equal("col", v.GetDefaultName());
	}

	[Fact]
	public void Numeric()
	{
		var text = "3.14";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		//Assert.Equal("3.14", v.GetCommandText());
	}

	[Fact]
	public void Numeric_Negative()
	{
		var text = "-1";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		Assert.IsType<LiteralValue>(v);

		//Assert.Equal("3.14", v.GetCommandText());
	}

	[Fact]
	public void Text()
	{
		var text = "'abc''s'";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		//Assert.Equal("'abc''s'", v.GetCommandText());
	}

	[Fact]
	public void BooleanTrue()
	{
		var text = "true";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		//Assert.Equal("true", v.GetCommandText());
	}

	[Fact]
	public void BooleanFalse()
	{
		var text = "false";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		//Assert.Equal("false", v.GetCommandText());
	}

	[Fact]
	public void Expression()
	{
		var text = "1*3.14";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

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

		//Assert.Equal("tbl.col1 * tbl.col2", v.GetCommandText());
		//Assert.Equal("", v.GetDefaultName());
	}

	[Fact]
	public void Expression3()
	{
		var text = "(1+1)*2";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		//Assert.Equal("(1 + 1) * 2", v.GetCommandText());
	}

	[Fact]
	public void Not()
	{
		var text = "not true";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		//Assert.Equal("not true", v.GetCommandText());
	}

	[Fact]
	public void Not2()
	{
		var text = "not (1 + 1 = 1)";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		//Assert.Equal("not (1 + 1 = 1)", v.GetCommandText());
	}

	[Fact]
	public void Function()
	{
		var text = "sum(tbl.col+    tbl.col2)";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		//Assert.Equal("sum(tbl.col + tbl.col2)", v.GetCommandText());
	}

	[Fact]
	public void Function_2()
	{
		var text = "concat('a', 'b')";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		Assert.IsType<FunctionValue>(v);
		var lv = (FunctionValue)v;
		Assert.Equal("concat", lv.Name);
		Assert.NotNull(lv.Argument);
	}

	[Fact]
	public void Function_3()
	{
		var text = "concat('a',     '/')";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		Assert.IsType<FunctionValue>(v);
		var lv = (FunctionValue)v;
		Assert.Equal("concat", lv.Name);
		Assert.NotNull(lv.Argument);
	}

	[Fact]
	public void WindowFunction()
	{
		var text = "row_number() over(partition by tbl.col, tbl.col2 order by tbl.col3, tbl.col4)";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		//Assert.Equal("row_number() over(partition by tbl.col, tbl.col2 order by tbl.col3, tbl.col4)", v.GetCommandText());
	}

	[Fact]
	public void WindowFunction2()
	{
		var text = "row_number() over(order by tbl.col, tbl.col2)";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(14, lst.Count);

		//Assert.Equal("row_number() over(order by tbl.col, tbl.col2)", v.GetCommandText());
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

		//Assert.Equal("case when tbl.col1 = 1 then 10 when tbl.col2 = 2 then 20 else 30 end", v.GetCommandText());
	}

	[Fact]
	public void InlineQuery()
	{
		var text = "(select a.val from table_a a where a.id = b.table_a_id)";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		Assert.IsType<InlineQuery>(v);
	}

	[Fact]
	public void ExistsExpression()
	{
		var text = "exists (select * from table_a a where a.id = b.table_a_id)";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		Assert.IsType<ExistsExpression>(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(17, lst.Count);
	}

	[Fact]
	public void ExistsExpression_Upper()
	{
		var text = "EXISTS (select * from table_a a where a.id = b.table_a_id)";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		Assert.IsType<ExistsExpression>(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(17, lst.Count);
	}

	[Fact]
	public void InExpression()
	{
		var text = "'a' in (select a.id from table_a)";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);

		Assert.IsType<InExpression>(v);
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

		Assert.IsType<FunctionValue>(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(8, lst.Count);
	}

	[Fact]
	public void Between()
	{
		var text = "a.id between 1 and 2";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);
	}

	[Fact]
	public void Like()
	{
		var text = "a.name like 'test%'";
		var v = ValueParser.Parse(text);
		Monitor.Log(v);
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
}