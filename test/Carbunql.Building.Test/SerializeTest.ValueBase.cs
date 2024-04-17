using Carbunql.Clauses;
using Carbunql.Values;
using MessagePack;

namespace Carbunql.Building.Test;

public partial class SerializeTest
{
    [Fact]
    public void LiteralValue()
    {
        var sq = new LiteralValue(1);

        var json = MessagePackSerializer.Serialize(sq);
        Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

        var actual = MessagePackSerializer.Deserialize<LiteralValue>(json);
        Output.WriteLine(actual.ToText());

        Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
    }

    [Fact]
    public void OperatableValue()
    {
        var sq = new LiteralValue(1);
        sq.Equal("2");

        var json = MessagePackSerializer.Serialize(sq);
        Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

        var actual = MessagePackSerializer.Deserialize<LiteralValue>(json);
        Output.WriteLine(actual.ToText());

        Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
    }

    [Fact]
    public void AsArgument()
    {
        var sq = new AsArgument(new LiteralValue("'0123'"), new LiteralValue("integer"));

        var json = MessagePackSerializer.Serialize(sq);
        Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

        var actual = MessagePackSerializer.Deserialize<AsArgument>(json);
        Output.WriteLine(actual.ToText());

        Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
    }

    [Fact]
    public void BetweenExpression()
    {
        var sq = new BetweenClause(new LiteralValue(5), new LiteralValue(1), new LiteralValue(10), false);

        var json = MessagePackSerializer.Serialize(sq);
        Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

        var actual = MessagePackSerializer.Deserialize<BetweenClause>(json);
        Output.WriteLine(actual.ToText());

        Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
    }

    [Fact]
    public void BracketValue()
    {
        var sq = new BracketValue(new LiteralValue(1));

        var json = MessagePackSerializer.Serialize(sq);
        Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

        var actual = MessagePackSerializer.Deserialize<BracketValue>(json);
        Output.WriteLine(actual.ToText());

        Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
    }

    [Fact]
    public void CaseExpressions()
    {
        var sq = new CaseExpression(new LiteralValue(1));
        sq.WhenExpressions.Add(new WhenExpression(new LiteralValue(2), new LiteralValue(3)));
        sq.Else(new LiteralValue(9));

        var json = MessagePackSerializer.Serialize(sq);
        Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

        var actual = MessagePackSerializer.Deserialize<CaseExpression>(json);
        Output.WriteLine(actual.ToText());

        Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
    }

    [Fact]
    public void CaseWhenExpressions()
    {
        var sq = new CaseExpression();
        sq.WhenExpressions.Add(new WhenExpression(new LiteralValue(true), new LiteralValue(1)));
        sq.Else(new LiteralValue(2));

        var json = MessagePackSerializer.Serialize(sq);
        Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

        var actual = MessagePackSerializer.Deserialize<CaseExpression>(json);
        Output.WriteLine(actual.ToText());

        Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
    }

    [Fact]
    public void CastValue()
    {
        var sq = new CastValue(new LiteralValue("'0'"), "::", new LiteralValue("integer"));

        var json = MessagePackSerializer.Serialize(sq);
        Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

        var actual = MessagePackSerializer.Deserialize<CastValue>(json);
        Output.WriteLine(actual.ToText());

        Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
    }

    [Fact]
    public void ColumnValue()
    {
        var sq = new ColumnValue("table", "column");

        var json = MessagePackSerializer.Serialize(sq);
        Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

        var actual = MessagePackSerializer.Deserialize<ColumnValue>(json);
        Output.WriteLine(actual.ToText());

        Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
    }

    [Fact]
    public void FromArgument()
    {
        var sq = new FromArgument(new LiteralValue("year"), new LiteralValue("current_timestamp"));

        var json = MessagePackSerializer.Serialize(sq);
        Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

        var actual = MessagePackSerializer.Deserialize<FromArgument>(json);
        Output.WriteLine(actual.ToText());

        Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
    }

    [Fact]
    public void FunctionValue()
    {
        var sq = new FunctionValue("sum", "1");

        var json = MessagePackSerializer.Serialize(sq);
        Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

        var actual = MessagePackSerializer.Deserialize<FunctionValue>(json);
        Output.WriteLine(actual.ToText());

        Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
    }

    [Fact]
    public void InExpression()
    {
        var lst = new ValueCollection
        {
            new LiteralValue(2),
            new LiteralValue(3)
        };

        var sq = new InClause(new LiteralValue(1), lst);

        var json = MessagePackSerializer.Serialize(sq);
        Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

        var actual = MessagePackSerializer.Deserialize<InClause>(json);
        Output.WriteLine(actual.ToText());

        Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
    }

    [Fact]
    public void NegativeValue()
    {
        var sq = new NegativeValue(new LiteralValue(true));

        var json = MessagePackSerializer.Serialize(sq);
        Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

        var actual = MessagePackSerializer.Deserialize<NegativeValue>(json);
        Output.WriteLine(actual.ToText());

        Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
    }

    [Fact]
    public void ParameterValue()
    {
        var sq = new ParameterValue("@id", 1);

        var json = MessagePackSerializer.Serialize(sq);
        Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

        var actual = MessagePackSerializer.Deserialize<ParameterValue>(json);
        Output.WriteLine(actual.ToText());

        Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
    }

    [Fact]
    public void QueryContainer()
    {
        var sq = new QueryContainer(new SelectQuery("select 1"));

        var json = MessagePackSerializer.Serialize(sq);
        Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

        var actual = MessagePackSerializer.Deserialize<QueryContainer>(json);
        Output.WriteLine(actual.ToText());

        Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
    }

    [Fact]
    public void WindowFunction()
    {
        var win = new OverClause();
        win.Partition(new ColumnValue("shop_id"));
        win.Order(new SortableItem(new ColumnValue("order_id")));

        var sq = new FunctionValue("row_number", win);

        var json = MessagePackSerializer.Serialize(sq);
        Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

        var actual = MessagePackSerializer.Deserialize<FunctionValue>(json);
        Output.WriteLine(actual.ToText());

        Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
    }

    [Fact]
    public void ValueCollection()
    {
        var sq = new ValueCollection(new[] { "a", "b", "c" });

        var json = MessagePackSerializer.Serialize(sq);
        Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

        var actual = MessagePackSerializer.Deserialize<ValueCollection>(json);
        Output.WriteLine(actual.ToText());

        Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
    }
}
