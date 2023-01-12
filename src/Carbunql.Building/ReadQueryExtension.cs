using Carbunql.Clauses;
using Carbunql.Tables;
using Cysharp.Text;

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

    public static (SelectQuery, FromClause) ToSubQuery(this IReadQuery source, string alias)
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

    public static QueryCommand ToInsertCommand(this IReadQuery source, string table, CommandTextBuilder? builder = null)
    {
        var sb = ZString.CreateStringBuilder();
        sb.Append("INSERT INTO " + table);

        var s = source.GetSelectClause();
        if (s != null)
        {
            var isFirst = true;

            foreach (var item in s.Items)
            {
                if (isFirst)
                {
                    sb.Append("(");
                    isFirst = false;
                }
                else
                {
                    sb.Append(", ");
                }
                sb.Append(item.Alias);
            }
            sb.Append(')');
        }

        builder ??= new CommandTextBuilder();
        var cmd = source.ToCommand(builder);
        cmd.CommandText = sb.ToString() + "\r\n" + cmd.CommandText;
        return cmd;
    }

    public static QueryCommand ToInsertCommand(this IReadQuery source, string table, IEnumerable<string> columns, CommandTextBuilder? builder = null)
    {
        var s = source.GetSelectClause();
        if (s == null) throw new NotSupportedException();

        var cols = s.Items.Where(x => columns.Contains(x.Alias)).Select(x => x.Alias).ToList();
        if (!cols.Any()) throw new Exception();

        var (q, f) = source.ToSubQuery("q");
        foreach (var item in cols)
        {
            q.Select(f, item);
        }

        return q.ToInsertCommand(table, builder);
    }
}