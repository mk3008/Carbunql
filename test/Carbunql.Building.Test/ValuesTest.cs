using Xunit.Abstractions;

namespace Carbunql.Building.Test;

public class ValuesTest
{
    private readonly QueryCommandMonitor Monitor;

    public ValuesTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
        Output = output;
    }

    private ITestOutputHelper Output { get; set; }

    [Fact]
    public void ListArray()
    {
        var vals = new List<string[]>() { new[] { "1", "value1" }, new[] { "2", "value2" }, new[] { "3", "value3" } };

        var q = new ValuesQuery(vals);

        Monitor.Log(q);

        var lst = q.GetTokens().ToList();
        Assert.Equal(18, lst.Count());
    }

    [Fact]
    public void ListList()
    {
        var vals = new List<List<string>>() { new() { "1", "value1" }, new() { "2", "value2" }, new() { "3", "value3" } };

        var q = new ValuesQuery(vals);

        Monitor.Log(q);

        var lst = q.GetTokens().ToList();
        Assert.Equal(18, lst.Count());
    }

    [Fact]
    public void ListParameter()
    {
        var vals = new List<string[]>() { new[] { "1", "value1" }, new[] { "2", "value2" }, new[] { "3", "value3" } };

        var q = new ValuesQuery(vals, ":");

        foreach (var item in q.GetParameters())
        {
            Output.WriteLine($"{item.ParameterName} : {item.Value}");
        }

        Monitor.Log(q);

        Assert.Equal(6, q.GetParameters().ToList().Count);

        var lst = q.GetTokens().ToList();
        Assert.Equal(18, lst.Count());
    }

    [Fact]
    public void Arrary()
    {
        var vals = new string[,] { { "1", "value1" }, { "2", "value2" }, { "3", "value3" } };

        var q = new ValuesQuery(vals);

        Monitor.Log(q);

        var lst = q.GetTokens().ToList();
        Assert.Equal(18, lst.Count());
    }

    [Fact]
    public void ArrayParameter()
    {
        var vals = new string[,] { { "1", "value1" }, { "2", "value2" }, { "3", "value3" } };

        var q = new ValuesQuery(vals, ":");

        foreach (var item in q.GetParameters())
        {
            Output.WriteLine($"{item.ParameterName} : {item.Value}");
        }

        Monitor.Log(q);

        Assert.Equal(6, q.GetParameters().ToList().Count);

        var lst = q.GetTokens().ToList();
        Assert.Equal(18, lst.Count());
    }

    public class Model
    {
        public int Id { get; set; }
        public string? Value { get; set; }
    }

    [Fact]
    public void ListObejct()
    {
        var vals = new List<Model>() { new Model { Id = 1, Value = "value1" }, new Model { Id = 2, Value = "value2" }, new Model { Id = 3, Value = "value3" } };

        var q = new ValuesQuery(vals);

        Monitor.Log(q);

        var lst = q.GetTokens().ToList();
        Assert.Equal(18, lst.Count());
    }

    [Fact]
    public void ListObejctParameter()
    {
        var vals = new List<Model>() { new Model { Id = 1, Value = "value1" }, new Model { Id = 2, Value = "value2" }, new Model { Id = 3, Value = null } };

        var q = new ValuesQuery(vals, ":");

        foreach (var item in q.GetParameters())
        {
            Output.WriteLine($"{item.ParameterName} : {item.Value}");
        }

        Monitor.Log(q);

        Assert.Equal(6, q.GetParameters().ToList().Count);

        var lst = q.GetTokens().ToList();
        Assert.Equal(18, lst.Count());
    }

    [Fact]
    public void ToSelectQuery()
    {
        var vals = new string[,] { { "1", "value1" }, { "2", "value2" }, { "3", "value3" } };

        var q = new ValuesQuery(vals, ":");

        var sq = q.ToSelectQuery(new[] { "id", "value" });
        Monitor.Log(sq);

        var lst = sq.GetTokens().ToList();
        Assert.Equal(36, lst.Count());
    }

    [Fact]
    public void ToCTE()
    {
        var vals = new string[,] { { "1", "value1" }, { "2", "value2" }, { "3", "value3" } };

        var q = new ValuesQuery(vals, ":");

        var sq = new SelectQuery();
        var cte = sq.With(q, new[] { "id", "value" }).As("cte");
        var f = sq.From(cte);
        sq.Select(f);

        Monitor.Log(sq);

        var lst = sq.GetTokens().ToList();
        Assert.Equal(38, lst.Count());
    }
}