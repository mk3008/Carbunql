using Carbunql.Analysis;
using Xunit.Abstractions;


namespace Carbunql.Building.Test;

public class CreateTableTest
{
    private readonly QueryCommandMonitor Monitor;

    public CreateTableTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void CreateTable()
    {
        var sql = "select a.id, a.value from table as a";
        var q = SelectQueryParser.Parse(sql);

        var cmd = q.ToCreateTableCommand("new_table");

        var ddl = "CREATE TABLE new_table\r\nAS\r\nSELECT\r\n    a.id,\r\n    a.value\r\nFROM\r\n    table AS a";

        Assert.Equal(ddl, cmd.CommandText);
    }

    [Fact]
    public void CreateTemporaryTable()
    {
        var sql = "select a.id, a.value from table as a";
        var q = SelectQueryParser.Parse(sql);

        var cmd = q.ToCreateTableCommand("new_table", isTemporary: true);

        var ddl = "CREATE TEMPORARY TABLE new_table\r\nAS\r\nSELECT\r\n    a.id,\r\n    a.value\r\nFROM\r\n    table AS a";

        Assert.Equal(ddl, cmd.CommandText);
    }
}