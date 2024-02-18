using Carbunql.Clauses;
using Carbunql.Tables;
using Carbunql.Values;
using MessagePack;

namespace Carbunql.Building.Test;

public partial class SerializeTest
{
	[Fact]
	public void SelectableItem()
	{
		var sq = new SelectableItem(new LiteralValue(1), "v1");

		var json = MessagePackSerializer.Serialize(sq);
		Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

		var actual = MessagePackSerializer.Deserialize<SelectableItem>(json);
		Output.WriteLine(actual.ToText());

		Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
	}

	[Fact]
	public void SelectClause()
	{
		var sq = new SelectClause
		{
			new SelectableItem(new LiteralValue(1), "v1"),
			new SelectableItem(new ColumnValue("t", "c"), "v2"),
		};

		var json = MessagePackSerializer.Serialize(sq);
		Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

		var actual = MessagePackSerializer.Deserialize<SelectClause>(json);
		Output.WriteLine(actual.ToText());

		Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
	}

	[Fact]
	public void SelectableTable()
	{
		var sq = new SelectableTable(new PhysicalTable("table_a"), "a");

		var json = MessagePackSerializer.Serialize(sq);
		Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

		var actual = MessagePackSerializer.Deserialize<SelectableTable>(json);
		Output.WriteLine(actual.ToText());

		Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
	}

	[Fact]
	public void Relation()
	{
		var sq = new Relation(new SelectableTable(new PhysicalTable("table_a"), "a"), "inner join");

		var json = MessagePackSerializer.Serialize(sq);
		Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

		var actual = MessagePackSerializer.Deserialize<Relation>(json);
		Output.WriteLine(actual.ToText());

		Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
	}

	[Fact]
	public void FromClause()
	{
		var sq = new FromClause(new SelectableTable(new PhysicalTable("table_a"), "a"));
		sq.InnerJoin(new SelectableTable(new PhysicalTable("table_b"), "b")).On(sq, "id");

		var json = MessagePackSerializer.Serialize(sq);
		Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

		var actual = MessagePackSerializer.Deserialize<FromClause>(json);
		Output.WriteLine(actual.ToText());

		Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
	}

	[Fact]
	public void WhereClause()
	{
		var sq = new WhereClause(new LiteralValue(true));

		var json = MessagePackSerializer.Serialize(sq);
		Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

		var actual = MessagePackSerializer.Deserialize<WhereClause>(json);
		Output.WriteLine(actual.ToText());

		Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
	}

	[Fact]
	public void GroupClause()
	{
		var sq = new GroupClause
		{
			new LiteralValue(true)
		};

		var json = MessagePackSerializer.Serialize(sq);
		Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

		var actual = MessagePackSerializer.Deserialize<GroupClause>(json);
		Output.WriteLine(actual.ToText());

		Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
	}

	[Fact]
	public void HavingClause()
	{
		var sq = new HavingClause(new LiteralValue(true));

		var json = MessagePackSerializer.Serialize(sq);
		Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

		var actual = MessagePackSerializer.Deserialize<HavingClause>(json);
		Output.WriteLine(actual.ToText());

		Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
	}

	[Fact]
	public void OrderClause()
	{
		var sq = new OrderClause
		{
			new LiteralValue(true)
		};

		var json = MessagePackSerializer.Serialize(sq);
		Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

		var actual = MessagePackSerializer.Deserialize<OrderClause>(json);
		Output.WriteLine(actual.ToText());

		Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
	}

	[Fact]
	public void CommonTable()
	{
		var sq = new CommonTable(new VirtualTable(new SelectQuery("select 1")), "a", new ValueCollection(new[] { "v1" }));

		var json = MessagePackSerializer.Serialize(sq);
		Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

		var actual = MessagePackSerializer.Deserialize<CommonTable>(json);
		Output.WriteLine(actual.ToText());

		Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
	}

	[Fact]
	public void LimitClause()
	{
		var sq = new LimitClause("10");

		var json = MessagePackSerializer.Serialize(sq);
		Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

		var actual = MessagePackSerializer.Deserialize<LimitClause>(json);
		Output.WriteLine(actual.ToText());

		Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
	}

	[Fact]
	public void SortableItem()
	{
		var sq = new SortableItem(new LiteralValue(1), isAscending: false, nullSort: NullSort.Last);

		var json = MessagePackSerializer.Serialize(sq);
		Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

		var actual = MessagePackSerializer.Deserialize<SortableItem>(json);
		Output.WriteLine(actual.ToText());

		Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
	}

	[Fact]
	public void WithClause()
	{
		var sq = new WithClause()
		{
			new CommonTable(new VirtualTable(new SelectQuery("select 1")), "a", new ValueCollection(new[] { "v1" }))
		};

		var json = MessagePackSerializer.Serialize(sq);
		Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

		var actual = MessagePackSerializer.Deserialize<WithClause>(json);
		Output.WriteLine(actual.ToText());

		Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
	}
}
