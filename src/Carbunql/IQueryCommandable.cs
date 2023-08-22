using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;
using Cysharp.Text;
using System.Data;

namespace Carbunql;

[MessagePack.Union(0, typeof(FromClause))]
[MessagePack.Union(1, typeof(HavingClause))]
[MessagePack.Union(2, typeof(LimitClause))]
[MessagePack.Union(3, typeof(MergeClause))]
[MessagePack.Union(4, typeof(MergeCondition))]
[MessagePack.Union(5, typeof(Relation))]
[MessagePack.Union(6, typeof(SelectableItem))]
[MessagePack.Union(7, typeof(SelectableTable))]
[MessagePack.Union(8, typeof(SortableItem))]
[MessagePack.Union(9, typeof(TableBase))]
[MessagePack.Union(10, typeof(UsingClause))]
[MessagePack.Union(11, typeof(ValueBase))]
[MessagePack.Union(12, typeof(WhereClause))]
[MessagePack.Union(13, typeof(CreateTableQuery))]
[MessagePack.Union(14, typeof(DeleteQuery))]
[MessagePack.Union(15, typeof(InsertQuery))]
[MessagePack.Union(16, typeof(MergeInsertQuery))]
[MessagePack.Union(17, typeof(MergeQuery))]
[MessagePack.Union(18, typeof(MergeUpdateQuery))]
[MessagePack.Union(19, typeof(OperatableQuery))]
[MessagePack.Union(20, typeof(UpdateQuery))]
[MessagePack.Union(21, typeof(IReadQuery))]
[MessagePack.Union(22, typeof(ReadQuery))]
[MessagePack.Union(23, typeof(SelectQuery))]
[MessagePack.Union(24, typeof(ValuesQuery))]
public interface IQueryCommandable : IQueryCommand
{
	IDictionary<string, object?> GetParameters();

	IEnumerable<SelectQuery> GetInternalQueries();

	IEnumerable<PhysicalTable> GetPhysicalTables();
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

	public static string ToText(this IQueryCommandable source)
	{
		var sb = ZString.CreateStringBuilder();
		if (source.GetParameters().Any())
		{
			sb.AppendLine("/*");
			foreach (var item in source.GetParameters())
			{
				if (item.Value == null)
				{
					sb.AppendLine($"  {item.Key} is NULL");
				}
				else if (item.Value.GetType() == typeof(string))
				{
					sb.AppendLine($"  {item.Key} = '{item.Value}'");
				}
				else
				{
					sb.AppendLine($"  {item.Key} = {item.Value}");
				}
			}
			sb.AppendLine("*/");
		}
		sb.AppendLine(source.ToCommand().CommandText);

		return sb.ToString();
	}
}