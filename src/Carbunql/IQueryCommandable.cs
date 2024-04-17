using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;
using Carbunql.Values;
using Cysharp.Text;
using MessagePack;

namespace Carbunql;

[Union(0, typeof(FromClause))]
[Union(1, typeof(HavingClause))]
[Union(2, typeof(LimitClause))]
[Union(3, typeof(MergeClause))]
[Union(4, typeof(MergeCondition))]
[Union(5, typeof(Relation))]
[Union(6, typeof(SelectableItem))]
[Union(7, typeof(SelectableTable))]
[Union(8, typeof(SortableItem))]
[Union(9, typeof(TableBase))]
[Union(10, typeof(UsingClause))]
[Union(11, typeof(ValueBase))]
[Union(12, typeof(WhereClause))]
[Union(13, typeof(CreateTableQuery))]
[Union(14, typeof(DeleteQuery))]
[Union(15, typeof(InsertQuery))]
[Union(16, typeof(MergeInsertQuery))]
[Union(17, typeof(MergeQuery))]
[Union(18, typeof(MergeUpdateQuery))]
[Union(19, typeof(OperatableQuery))]
[Union(20, typeof(UpdateQuery))]
[Union(21, typeof(IReadQuery))]
[Union(22, typeof(ReadQuery))]
[Union(23, typeof(SelectQuery))]
[Union(24, typeof(ValuesQuery))]
[Union(25, typeof(LiteralValue))]
[Union(26, typeof(AsArgument))]
[Union(27, typeof(BetweenClause))]
[Union(28, typeof(BracketValue))]
[Union(29, typeof(CaseExpression))]
[Union(30, typeof(CastValue))]
[Union(31, typeof(ColumnValue))]
[Union(32, typeof(FromArgument))]
[Union(33, typeof(FunctionValue))]
[Union(34, typeof(InClause))]
[Union(35, typeof(NegativeValue))]
[Union(36, typeof(ParameterValue))]
[Union(37, typeof(QueryContainer))]
[Union(38, typeof(ValueCollection))]
[Union(39, typeof(FunctionTable))]
[Union(40, typeof(PhysicalTable))]
[Union(41, typeof(VirtualTable))]
[Union(42, typeof(MergeWhenDelete))]
[Union(43, typeof(MergeWhenInsert))]
[Union(44, typeof(MergeWhenNothing))]
[Union(45, typeof(MergeWhenUpdate))]
public interface IQueryCommandable
{
    IEnumerable<Token> GetTokens(Token? parent);

    IEnumerable<QueryParameter> GetParameters();

    IEnumerable<SelectQuery> GetInternalQueries();

    IEnumerable<PhysicalTable> GetPhysicalTables();

    IEnumerable<CommonTable> GetCommonTables();
}

public static class IQueryCommandableExtension
{
    public static IEnumerable<Token> GetTokens(this IQueryCommandable source)
    {
        return source.GetTokens(null);
    }

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

    public static string ToText(this IQueryCommandable source, bool exportParameterInfo = true)
    {
        var cmd = source.ToCommand();
        var text = cmd.CommandText;

        if (!cmd.Parameters.Any() || !exportParameterInfo) return text;

        var head = GetParameterText(cmd);
        if (string.IsNullOrEmpty(head)) return text;
        return head + text;
    }

    public static string ToOneLineText(this IQueryCommandable source)
    {
        var cmd = source.ToOneLineCommand();
        var text = cmd.CommandText;

        if (!cmd.Parameters.Any()) return text;

        var head = GetParameterText(cmd);
        if (string.IsNullOrEmpty(head)) return text;
        return head + text;
    }

    private static string GetParameterText(this QueryCommand source)
    {
        var prms = source.Parameters;
        if (!prms.Any()) return string.Empty;

        var names = new List<string>();

        var sb = ZString.CreateStringBuilder();
        sb.AppendLine("/*");
        foreach (var item in prms)
        {
            if (names.Contains(item.Key)) continue;

            names.Add(item.Key);
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

        return sb.ToString();
    }
}