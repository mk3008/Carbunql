using Carbunql.Extensions;
using System.Data;

namespace Carbunql;

public interface IQueryCommandable : IQueryCommand
{
	IDictionary<string, object?> GetParameters();
}

public static class IQueryCommandableExtension
{
	public static QueryCommand ToCommand(this IQueryCommandable source)
	{
		var builder = new CommandTextBuilder();
		return new QueryCommand(builder.Execute(source), source.GetParameters());
	}

	public static QueryCommand ToCommand(this IQueryCommandable source, CommandTextBuilder builder)
	{
		return new QueryCommand(builder.Execute(source), source.GetParameters());
	}

	public static QueryCommand ToOneLineCommand(this IQueryCommandable source)
	{
		return new QueryCommand(source.GetTokens().ToText(), source.GetParameters());
	}

	public static IDbCommand ToDbCommand(this IQueryCommandable source, IDbConnection cn)
	{
		var c = source.ToCommand();
		var cmd = cn.CreateCommand();
		cmd.CommandText = c.CommandText;
		foreach (var item in c.Parameters)
		{
			var p = cmd.CreateParameter();
			p.ParameterName = item.Key;
			p.Value = item.Value;
			cmd.Parameters.Add(p);
		}
		return cmd;
	}
}