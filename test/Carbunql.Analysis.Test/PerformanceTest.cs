using Xunit.Abstractions;

namespace Carbunql.Analysis.Test;

public class PerformanceTest
{
    private readonly QueryCommandMonitor Monitor;

    public PerformanceTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void Case1()
    {
        var text = @"SELECT View1.OrderDate,View1.Email,SUM(View1.TotalPayments) FROM dbo.View1 WHERE (View1.OrderStatus = 'Completed') GROUP BY View1.OrderDate,View1.Email HAVING (SUM(View1.TotalPayments) > 75)";
        var item = QueryParser.Parse(text);
        Monitor.Log(item);
    }

    [Fact]
    public void Case2()
    {
        var text = @"select
    ' comment('')comment ' comment /* prefix /* nest */ sufix */,
    a.id::text as id,
    '1'::int as v1,
    1::text as v2,
    (1+1)::text as v3,
    to_char(a.col1, 'yyyy')::int as v4,
    3.1415::numeric(8,2) as v5,
    1 + 1 = 2 v6,
    1 + 1 = 2 and 2 + 2 = 4 and 3 + 3 = 6 v7,
    1 + 1 = 2 or  2 * 2 = 2 or  3 + 3 = 3 v8
from a";
        var item = QueryParser.Parse(text);
        Monitor.Log(item);
    }

    [Fact]
    public void Case3()
    {
        var text = @"select
    a.val1 || a.val2 as t1,
    case when 1=1 then '1' else '2' end || 'a' as t2,
    a.txt1 || case when 1=1 then 1 else 2 end as t3,
    case when true then true else false end as t4
from
    a";
        var item = QueryParser.Parse(text);
        Monitor.Log(item);
    }
}
