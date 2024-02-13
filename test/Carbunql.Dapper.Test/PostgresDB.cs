using Microsoft.Extensions.Logging;
using Npgsql;
using Testcontainers.PostgreSql;

namespace Carbunql.Dapper.Test;

/*
https://testcontainers.com/guides/getting-started-with-testcontainers-for-dotnet/
 */

public class PostgresDB : IAsyncLifetime
{
	private readonly PostgreSqlContainer Container = new PostgreSqlBuilder().WithImage("postgres:15-alpine").Build();

	public Task InitializeAsync()
	{
		return Container.StartAsync();
	}

	public Task DisposeAsync()
	{
		return Container.DisposeAsync().AsTask();
	}

	public LoggingDbConnection ConnectionOpenAsNew(ILogger logger)
	{
		var cn = new NpgsqlConnection(Container.GetConnectionString());
		var lcn = new LoggingDbConnection(cn, logger);
		lcn.Open();
		return lcn;
	}
}
