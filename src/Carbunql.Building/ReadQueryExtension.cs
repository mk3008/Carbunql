using Carbunql.Clauses;
using Carbunql.Tables;

namespace Carbunql.Building;

public static class ReadQueryExtension
{
    public static CTEQuery ToCTE(this IReadQuery source, string alias)
    {
        var sq = new CTEQuery();

        sq.WithClause.Add(source.ToCommonTable(alias));

        return sq;
    }

    public static CommonTable ToCommonTable(this IReadQuery source, string alias)
    {
        return new CommonTable(new VirtualTable(source), alias);
    }

    public static (ReadQuery, FromClause) ToSubQuery(this IReadQuery source, string alias)
    {
        var sq = new SelectQuery();
        var f = sq.From(source, alias);
        return (sq, f);
    }

    public static QueryCommand ToCreateTableCommand(this IReadQuery source, string table, bool isTemporary = false, CommandTextBuilder? builder = null)
    {
        builder ??= new CommandTextBuilder();
        var cmd = source.ToCommand(builder);
        cmd.CommandText = "CREATE " + (isTemporary ? "TEMPORARY TABLE " : "TABLE ") + table + "\r\nAS\r\n" + cmd.CommandText;
        return cmd;
    }
}