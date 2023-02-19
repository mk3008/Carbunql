using Carbunql.Analysis;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Xunit.Abstractions;


namespace Carbunql.Building.Test;

public class SelectQueryGeneratorTest
{
    public SelectQueryGeneratorTest(ITestOutputHelper output)
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
                Output.WriteLine($"    {prm.Key} = {prm.Value}");
            }
            Output.WriteLine("*/");
        }
        Output.WriteLine(cmd.CommandText);
    }

    private class Model
    {
        public int ModelID { get; set; }
        public string ModelName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    [Fact]
    public void ObjectToSelectQuery()
    {
        var m = new Model() { ModelID = 2, ModelName = "test" };

        var sq = SelectQueryGenerator.FromItem(m);
        DebugPrint(sq.ToCommand());

        var lst = sq.GetTokens().ToList();

        Assert.Equal(32, lst.Count());
        Assert.Equal(":ModelID0", lst[16].Text);
    }

    [Fact]
    public void ObjectToSelectQuery_LowerSnakeCase()
    {
        var m = new Model() { ModelID = 2, ModelName = "test" };
        var sq = SelectQueryGenerator.FromItem(m, "@", StringFormatter.ToLowerSnakeCase);
        DebugPrint(sq.ToCommand());

        var lst = sq.GetTokens().ToList();

        Assert.Equal(32, lst.Count());
        Assert.Equal("@model_id0", lst[16].Text);
    }

    [Fact]
    public void ListToSelectQuery()
    {
        var models = new List<Model>
        {
            new Model() { ModelID = 1, ModelName = "abc" },
            new Model() { ModelID = 2, ModelName = "test" },
            new Model() { ModelID = 3, ModelName = "xyz" }
        };

        var sq = SelectQueryGenerator.FromList(models, "@", StringFormatter.ToLowerSnakeCase);
        DebugPrint(sq.ToCommand());

        var lst = sq.GetTokens().ToList();

        Assert.Equal(48, lst.Count());
        Assert.Equal("@description2", lst[36].Text);
    }
}