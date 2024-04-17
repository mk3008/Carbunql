using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Carbunql.Dapper.Test;

public class UnitTestLogger : ILogger
{
    public ITestOutputHelper Output { get; set; } = null!;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        Output.WriteLine($"{DateTime.Now:HH:mm:ss.fff} [{logLevel}] {formatter(state, exception)}");
    }
}