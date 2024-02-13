using Microsoft.Extensions.Logging;
using System.Data;

namespace Carbunql.Dapper.Test;

public class LoggingDbTransaction : IDbTransaction
{
	public LoggingDbTransaction(IDbTransaction transaction, ILogger logger)
	{
		DbTransaction = transaction;
		Logger = logger;
		Logger.Log(LogLevel, nameof(Connection.BeginTransaction));
	}

	private ILogger Logger { get; init; }

	private IDbTransaction DbTransaction { get; init; }

	public LogLevel LogLevel { get; set; } = LogLevel.Information;

	public IDbConnection? Connection => DbTransaction.Connection;

	public IsolationLevel IsolationLevel => DbTransaction.IsolationLevel;

	public void Commit()
	{
		DbTransaction.Commit();
		Logger.Log(LogLevel, nameof(Commit));
	}

	public void Rollback()
	{
		DbTransaction.Rollback();
		Logger.Log(LogLevel, nameof(Rollback));
	}

	public void Dispose()
	{
		DbTransaction.Dispose();
	}
}
