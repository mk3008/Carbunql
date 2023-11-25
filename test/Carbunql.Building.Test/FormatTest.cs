using Xunit.Abstractions;

namespace Carbunql.Building.Test;

public class FormatTest
{
	private readonly QueryCommandMonitor Monitor;

	public FormatTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
	}

	private string GetFormatValidateText(string text)
	{
		return text.Replace("\t", "    ").Replace("\r", "");
	}

	[Fact]
	public void Issuse280_CreateTable()
	{
		var sq = new SelectQuery("SELECT x.id FROM x");
		var q = sq.ToCreateTableQuery("test");

		Monitor.Log(q);

		var actual = q.ToText();
		var expect =
@"CREATE TEMPORARY TABLE
    test
AS
SELECT
    x.id
FROM
    x";

		Assert.Equal(GetFormatValidateText(expect), GetFormatValidateText(actual));
	}

	[Fact]
	public void Issuse280_Union()
	{
		var sq = new SelectQuery("select x.id from x union select y.id from y");

		Monitor.Log(sq);

		var actual = sq.ToText();
		var expect =
@"SELECT
    x.id
FROM
    x
UNION
SELECT
    y.id
FROM
    y";

		Assert.Equal(GetFormatValidateText(expect), GetFormatValidateText(actual));
	}

	[Fact]
	public void Issuse280_SubQuery()
	{
		var sq = new SelectQuery("select x.id from (select select y.id from y) as x");

		Monitor.Log(sq);

		var actual = sq.ToText();
		var expect =
@"SELECT
    x.id
FROM
    (
        SELECT
            y.id
        FROM
            y
    ) AS x";

		Assert.Equal(GetFormatValidateText(expect), GetFormatValidateText(actual));
	}
}
