using Carbunql.Analysis;
using Xunit.Abstractions;


namespace Carbunql.Building.Test;

public class InsertTest
{
    private readonly QueryCommandMonitor Monitor;

    public InsertTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void InsertQuery()
    {
        var sql = "select a.id, a.value as v from table as a";
        var q = QueryParser.Parse(sql);

        var cmd = q.ToInsertCommand("new_table");

        var text = "INSERT INTO new_table(id, v)\r\nSELECT\r\n    a.id,\r\n    a.value AS v\r\nFROM\r\n    table AS a";

        Assert.Equal(text, cmd.CommandText);
    }

    [Fact]
    public void InsertQuery_Values()
    {
        var sql = "values (1, 'a'), (2, 'b')";
        var q = QueryParser.Parse(sql);

        var cmd = q.ToInsertCommand("new_table");

        var text = "INSERT INTO new_table\r\nVALUES\r\n    (1, 'a'),\r\n    (2, 'b')";

        Assert.Equal(text, cmd.CommandText);
    }
}