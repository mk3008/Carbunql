using QueryBuilderByLinq.Analysis;
using Xunit.Abstractions;
using static QueryBuilderByLinq.Sql;

namespace QueryBuilderByLinq.Test.Analysis;

public class SelectColumnInfoParserTest
{
	private readonly QueryCommandMonitor Monitor;

	public SelectColumnInfoParserTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
		Output = output;
	}

	private ITestOutputHelper Output { get; set; }

	[Fact]
	public void SelectColumn()
	{
		var query = from a in FromTable<table_a>()
					select new { a.a_id };

		Monitor.Log(query);

		var items = SelectColumnInfoParser.Parse(query.Expression);
		Assert.Single(items);
		Assert.Equal("a_id", items[0].Alias);
		Assert.Equal("a.a_id", items[0].Value.ToText());
	}

	[Fact]
	public void SelectColumns()
	{
		var query = from a in FromTable<table_a>()
					select new
					{
						a.a_id,
						a.text,
						a.value
					};

		Monitor.Log(query);

		var items = SelectColumnInfoParser.Parse(query.Expression);
		Assert.Equal(3, items.Count);
		Assert.Equal("a_id", items[0].Alias);
		Assert.Equal("a.a_id", items[0].Value.ToText());
		Assert.Equal("text", items[1].Alias);
		Assert.Equal("a.text", items[1].Value.ToText());
		Assert.Equal("value", items[2].Alias);
		Assert.Equal("a.value", items[2].Value.ToText());
	}

	[Fact]
	public void Alias()
	{
		var query = from a in FromTable<table_a>()
					select new
					{
						ID = a.a_id,
						TEXT = a.text,
						VALUE = a.value
					};

		Monitor.Log(query);

		var items = SelectColumnInfoParser.Parse(query.Expression);
		Assert.Equal(3, items.Count);
		Assert.Equal("ID", items[0].Alias);
		Assert.Equal("a.a_id", items[0].Value.ToText());
		Assert.Equal("TEXT", items[1].Alias);
		Assert.Equal("a.text", items[1].Value.ToText());
		Assert.Equal("VALUE", items[2].Alias);
		Assert.Equal("a.value", items[2].Value.ToText());
	}

	[Fact]
	public void Join()
	{
		var query = from a in FromTable<table_a>()
					from b in CrossJoinTable<table_a>()
					where a.a_id == 1 && b.text == "test"
					select new
					{
						a.a_id,
						b.text
					};

		Monitor.Log(query);

		var items = SelectColumnInfoParser.Parse(query.Expression);
		Assert.Equal(2, items.Count);
		Assert.Equal("a_id", items[0].Alias);
		Assert.Equal("a.a_id", items[0].Value.ToText());
		Assert.Equal("text", items[1].Alias);
		Assert.Equal("b.text", items[1].Value.ToText());
	}

	public record struct table_a(int a_id, string text, int value);
}