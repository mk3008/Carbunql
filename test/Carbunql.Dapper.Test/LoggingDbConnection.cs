using Microsoft.Extensions.Logging;
using System.Data;

namespace Carbunql.Dapper.Test;

public class LoggingDbConnection : IDbConnection
{
	public LoggingDbConnection(IDbConnection connection, ILogger logger)
	{
		Connection = connection;
		Logger = logger;
	}

	private ILogger Logger { get; init; }

	private IDbConnection Connection { get; init; }

	public LogLevel LogLevel { get; set; } = LogLevel.Information;

	#region "implements interface"
#pragma warning disable CS8767
	public string ConnectionString { get => Connection.ConnectionString; set => Connection.ConnectionString = value; }
#pragma warning restore CS8767

	public int ConnectionTimeout => Connection.ConnectionTimeout;

	public string Database => Connection.Database;

	public ConnectionState State => Connection.State;

	public IDbTransaction BeginTransaction()
	{
		var trn = Connection.BeginTransaction();
		var ltrn = new LoggingDbTransaction(trn, Logger) { LogLevel = LogLevel };
		return ltrn;
	}

	public IDbTransaction BeginTransaction(IsolationLevel il)
	{
		return Connection.BeginTransaction(il);
	}

	public void ChangeDatabase(string databaseName)
	{
		Connection.ChangeDatabase(databaseName);
	}

	public void Close()
	{
		Connection.Close();
	}

	public IDbCommand CreateCommand()
	{
		var cmd = Connection.CreateCommand();
		var logcmd = new LoggingDbCommand(cmd, Logger)
		{
			LogLevel = LogLevel
		};
		return logcmd;
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
	}

	public void Open()
	{
		Connection.Open();
	}

	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
	{
		Logger.Log(logLevel, eventId, state, exception, formatter);
	}

	public bool IsEnabled(LogLevel logLevel)
	{
		return Logger.IsEnabled(logLevel);
	}

	public IDisposable? BeginScope<TState>(TState state) where TState : notnull
	{
		return Logger.BeginScope(state);
	}
	#endregion
}
