using Carbunql.TypeSafe.Dialect;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace Carbunql.TypeSafe.Test;

public class SqlDialectTest(DependencySetupFixture fixture, ITestOutputHelper output) : IClassFixture<DependencySetupFixture>
{
    private readonly DependencySetupFixture Fixture = fixture;
    private readonly ITestOutputHelper Output = output;
    private readonly ServiceProvider ServiceProvider = fixture.ServiceProvider;

    [Fact]
    public void DefaultTest()
    {
        var id = 1;
        var value = 10.5;

        var query = Sql.From()
            .Select(() => new
            {
                parameter_symbol = id,
                now_command = DateTime.Now,
                timestamp_command = Sql.CurrentTimestamp,
                coalesce_command = id == 1 ? true : false,
                modulo_command = value % 10,
                truncate_command = Math.Truncate(value),
                floor_command = Math.Floor(value),
                ceiling_command = Math.Ceiling(value),
                round_command = Math.Round(value),
                round_command_arg = Math.Round(value, 1),
                cast_test = id * value,
            });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"/*
  :id = 1
  :value = 10.5
*/
SELECT
    :id AS parameter_symbol,
    NOW() AS now_command,
    current_timestamp AS timestamp_command,
    CASE
        WHEN :id = 1 THEN True
        ELSE False
    END AS coalesce_command,
    :value % 10 AS modulo_command,
    TRUNC(:value) AS truncate_command,
    FLOOR(:value) AS floor_command,
    CEIL(:value) AS ceiling_command,
    ROUND(:value) AS round_command,
    ROUND(:value, 1) AS round_command_arg,
    CAST(:id AS double precision) * :value AS cast_test";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void BuilderTest_Manual()
    {
        var id = 1;
        var value = 10.5;

        var query = new FluentSelectQueryBuilder(new SQLiteTranspiler()).From()
            .Select(() => new
            {
                parameter_symbol = id,
                now_command = DateTime.Now,
                timestamp_command = Sql.CurrentTimestamp,
                coalesce_command = id == 1 ? true : false,
                modulo_command = value % 10,
                truncate_command = Math.Truncate(value),
                floor_command = Math.Floor(value),
                ceiling_command = Math.Ceiling(value),
                round_command = Math.Round(value),
                round_command_arg = Math.Round(value, 1),
                cast_test = id * value,
            });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"/*
  @id = 1
  @value = 10.5
*/
SELECT
    @id AS parameter_symbol,
    DATETIME('now') AS now_command,
    current_timestamp AS timestamp_command,
    CASE
        WHEN @id = 1 THEN True
        ELSE False
    END AS coalesce_command,
    @value % 10 AS modulo_command,
    CAST(@value AS integer) AS truncate_command,
    FLOOR(@value) AS floor_command,
    CEIL(@value) AS ceiling_command,
    ROUND(@value) AS round_command,
    ROUND(@value, 1) AS round_command_arg,
    CAST(@id AS real) * @value AS cast_test";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void BuilderTest_DependencyInjection()
    {
        var id = 1;
        var value = 10.5;
        var builder = ServiceProvider.GetService<FluentSelectQueryBuilder>()!;
        var query = builder.From()
            .Select(() => new
            {
                parameter_symbol = id,
                now_command = DateTime.Now,
                timestamp_command = Sql.CurrentTimestamp,
                coalesce_command = id == 1 ? true : false,
                modulo_command = value % 10,
                truncate_command = Math.Truncate(value),
                floor_command = Math.Floor(value),
                ceiling_command = Math.Ceiling(value),
                round_command = Math.Round(value),
                round_command_arg = Math.Round(value, 1),
                cast_test = id * value,
            });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"/*
  @id = 1
  @value = 10.5
*/
SELECT
    @id AS parameter_symbol,
    DATETIME('now') AS now_command,
    current_timestamp AS timestamp_command,
    CASE
        WHEN @id = 1 THEN True
        ELSE False
    END AS coalesce_command,
    @value % 10 AS modulo_command,
    CAST(@value AS integer) AS truncate_command,
    FLOOR(@value) AS floor_command,
    CEIL(@value) AS ceiling_command,
    ROUND(@value) AS round_command,
    ROUND(@value, 1) AS round_command_arg,
    CAST(@id AS real) * @value AS cast_test";

        Assert.Equal(expect, actual, true, true, true);
    }
}


public class DependencySetupFixture : IDisposable
{
    public ServiceProvider ServiceProvider { get; private set; }

    public DependencySetupFixture()
    {
        var serviceCollection = new ServiceCollection()
            .AddSingleton<ISqlTranspiler, SQLiteTranspiler>()
            .AddTransient<FluentSelectQueryBuilder>();

        ServiceProvider = serviceCollection.BuildServiceProvider();
    }

    public void Dispose()
    {
        ServiceProvider.Dispose();
    }
}