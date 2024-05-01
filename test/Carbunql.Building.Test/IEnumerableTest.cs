using Xunit.Abstractions;

namespace Carbunql.Building.Test;

public class IEnumerableTest
{
    private readonly QueryCommandMonitor Monitor;

    public IEnumerableTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
        Output = output;
    }

    private ITestOutputHelper Output { get; set; }

    [Fact]
    public void ToValuesQuery()
    {
        var results = new List<ApiResult>()
        {
            new (){ ResultId = 1 , ResultText = "a", ResultValue = 10, ResultDate = new DateTime(2000,1,1), ResultBool= true },
            new (){ ResultId = 2 , ResultText = "b", ResultValue = 20, ResultDate = new DateTime(2010,10,10), ResultBool= false },
            new (){ ResultId = 3 , ResultText = null, ResultValue = null, ResultDate = null, ResultBool= null},
        };

        var q = results.ToValuesQuery();

        var actual = q.ToText(true);
        Output.WriteLine(actual);

        var expect = @"/*
  r0c0 = 1
  r0c1 = 'a'
  r0c2 = 10
  r0c3 = 2000/01/01 0:00:00
  r0c4 = True
  r1c0 = 2
  r1c1 = 'b'
  r1c2 = 20
  r1c3 = 2010/10/10 0:00:00
  r1c4 = False
  r2c0 = 3
  r2c1 is NULL
  r2c2 is NULL
  r2c3 is NULL
  r2c4 is NULL
*/
VALUES
    (:r0c0, :r0c1, :r0c2, :r0c3, :r0c4),
    (:r1c0, :r1c1, :r1c2, :r1c3, :r1c4),
    (:r2c0, :r2c1, :r2c2, :r2c3, :r2c4)";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void ToSelectQuery()
    {
        var results = new List<ApiResult>()
        {
            new (){ ResultId = 1 , ResultText = "a", ResultValue = 10, ResultDate = new DateTime(2000,1,1), ResultBool= true },
            new (){ ResultId = 2 , ResultText = "b", ResultValue = 20, ResultDate = new DateTime(2010,10,10), ResultBool= false },
            new (){ ResultId = 3 , ResultText = null, ResultValue = null, ResultDate = null, ResultBool= null},
        };

        var q = results.ToSelectQuery();

        var actual = q.ToText(true);
        Output.WriteLine(actual);

        var expect = @"/*
  r0c0 = 1
  r0c1 = 'a'
  r0c2 = 10
  r0c3 = 2000/01/01 0:00:00
  r0c4 = True
  r1c0 = 2
  r1c1 = 'b'
  r1c2 = 20
  r1c3 = 2010/10/10 0:00:00
  r1c4 = False
  r2c0 = 3
  r2c1 is NULL
  r2c2 is NULL
  r2c3 is NULL
  r2c4 is NULL
*/
SELECT
    v.result_id,
    v.result_text,
    v.result_value,
    v.result_date,
    v.result_bool
FROM
    (
        VALUES
            (:r0c0, :r0c1, :r0c2, :r0c3, :r0c4),
            (:r1c0, :r1c1, :r1c2, :r1c3, :r1c4),
            (:r2c0, :r2c1, :r2c2, :r2c3, :r2c4)
    ) AS v (
        result_id, result_text, result_value, result_date, result_bool
    )";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void ToSelectQuery_50k()
    {
        var results = GenerateDummyResults(50000);

        var q = results.ToSelectQuery();

        var actual = q.ToText(false);
        Output.WriteLine(actual);
    }

    [Fact]
    public void AnonymousTypeTest_ToSelectQuery()
    {
        var students = new[]
        {
            new { Name = "Alice", Age = 20 },
            new { Name = "Bob", Age = 22 },
            new { Name = "Charlie", Age = 21 }
        };

        var q = students.ToSelectQuery();

        var actual = q.ToText(false);
        Output.WriteLine(actual);

        var expect = @"SELECT
    v.name,
    v.age
FROM
    (
        VALUES
            (:r0c0, :r0c1),
            (:r1c0, :r1c1),
            (:r2c0, :r2c1)
    ) AS v (
        name, age
    )";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void AnonymousTypeTest_ToValuesQuery()
    {
        var students = new[]
        {
            new { Name = "Alice", Age = 20 },
            new { Name = "Bob", Age = 22 },
            new { Name = "Charlie", Age = 21 }
        };

        var q = students.ToValuesQuery();

        var actual = q.ToText(false);
        Output.WriteLine(actual);

        var expect = @"VALUES
    (:r0c0, :r0c1),
    (:r1c0, :r1c1),
    (:r2c0, :r2c1)";

        Assert.Equal(expect, actual, true, true, true);
    }

    private static List<ApiResult> GenerateDummyResults(int count)
    {
        var dummyResults = new List<ApiResult>();

        for (int i = 0; i < count; i++)
        {
            var result = new ApiResult
            {
                ResultId = i + 1,
                ResultText = "dummy",
                ResultValue = i * 10,
                ResultDate = DateTime.Now.AddDays(i),
                ResultBool = i % 2 == 0
            };

            dummyResults.Add(result);
        }

        return dummyResults;
    }

    public class ApiResult
    {
        public int ResultId { get; set; }
        public string? ResultText { get; set; }
        public long? ResultValue { get; set; }
        public DateTime? ResultDate { get; set; }
        public bool? ResultBool { get; set; }
    }
}
