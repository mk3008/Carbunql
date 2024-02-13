using Microsoft.Extensions.Logging;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;

namespace Carbunql.Dapper.Test;

public class LoggingDbCommand : IDbCommand
{
	public LoggingDbCommand(IDbCommand command, ILogger logger)
	{
		DbCommand = command;
		Logger = logger;
	}

	private ILogger Logger { get; init; }

	private IDbCommand DbCommand { get; init; }

	public LogLevel LogLevel { get; set; } = LogLevel.Information;

	private string GetParameterText()
	{
		if (DbCommand.Parameters.Count == 0) return string.Empty;

		var sb = new StringBuilder();
		sb.AppendLine("/*");
		foreach (IDbDataParameter item in DbCommand.Parameters)
		{
			if (item.Value == null)
			{
				sb.AppendLine("  " + item.ParameterName + " is NULL");
			}
			else if (item.Value.GetType() == typeof(string))
			{
				sb.AppendLine($"  {item.ParameterName} = '{item.Value}'");
			}
			else
			{
				sb.AppendLine($"  {item.ParameterName} = {item.Value}");
			}
		}
		sb.AppendLine("*/");

		return sb.ToString();
	}

	private void WriteLog([CallerMemberName] string callerMethodName = "unknown")
	{
		Logger.Log(LogLevel, callerMethodName + ";\n" + GetParameterText() + CommandText + ";");
	}

	private void WriteExecuteResultLog(int rows, [CallerMemberName] string callerMethodName = "unknown")
	{
		Logger.Log(LogLevel, callerMethodName + " result : " + rows);
	}

	private void WriteExecuteScalarLog(object? value, [CallerMemberName] string callerMethodName = "unknown")
	{
		if (value == null)
		{
			Logger.Log(LogLevel, callerMethodName + " return : NULL");
		}
		else if (value.GetType() == typeof(string))
		{
			Logger.Log(LogLevel, callerMethodName + $" return : '{value}'");
		}
		else
		{
			Logger.Log(LogLevel, callerMethodName + $" return : {value}");
		}
	}

	#region "implements interface"

#pragma warning disable CS8767 // パラメーターの型における参照型の NULL 値の許容が、暗黙的に実装されるメンバーと一致しません。おそらく、NULL 値の許容の属性が原因です。
	public string CommandText { get => DbCommand.CommandText; set => DbCommand.CommandText = value; }
#pragma warning restore CS8767 // パラメーターの型における参照型の NULL 値の許容が、暗黙的に実装されるメンバーと一致しません。おそらく、NULL 値の許容の属性が原因です。

	public int CommandTimeout { get => DbCommand.CommandTimeout; set => DbCommand.CommandTimeout = value; }

	public CommandType CommandType { get => DbCommand.CommandType; set => DbCommand.CommandType = value; }

	public IDbConnection? Connection { get => DbCommand.Connection; set => DbCommand.Connection = value; }

	public IDataParameterCollection Parameters => DbCommand.Parameters;

	public IDbTransaction? Transaction { get => DbCommand.Transaction; set => DbCommand.Transaction = value; }

	public UpdateRowSource UpdatedRowSource { get => DbCommand.UpdatedRowSource; set => DbCommand.UpdatedRowSource = value; }

	public void Cancel()
	{
		DbCommand.Cancel();
	}

	public IDbDataParameter CreateParameter()
	{
		return DbCommand.CreateParameter();
	}

	public void Dispose()
	{
		DbCommand.Dispose();
	}

	public int ExecuteNonQuery()
	{
		WriteLog();
		var val = DbCommand.ExecuteNonQuery();
		WriteExecuteResultLog(val);
		return val;
	}

	public IDataReader ExecuteReader()
	{
		WriteLog();
		return DbCommand.ExecuteReader();
	}

	public IDataReader ExecuteReader(CommandBehavior behavior)
	{
		WriteLog();
		return DbCommand.ExecuteReader(behavior);
	}

	public object? ExecuteScalar()
	{
		WriteLog();
		var val = DbCommand.ExecuteScalar();
		WriteExecuteScalarLog(val);
		return val;
	}

	public void Prepare()
	{
		DbCommand.Prepare();
	}
	#endregion
}
