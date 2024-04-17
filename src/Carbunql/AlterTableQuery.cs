using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Definitions;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql;

public class AlterTableQuery : IQueryCommandable, ICommentable, ITable
{
    public AlterTableQuery(AlterTableClause clause)
    {
        AlterTableClause = clause;
    }

    public AlterTableClause AlterTableClause { get; set; }

    [IgnoreMember]
    public CommentClause? CommentClause { get; set; }

    public string Schema => ((ITable)AlterTableClause).Schema;

    public string Table => ((ITable)AlterTableClause).Table;

    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        yield break;
    }

    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        yield break;
    }

    public IEnumerable<Token> GetTokens(Token? parent)
    {
        if (CommentClause != null) foreach (var item in CommentClause.GetTokens(parent)) yield return item;

        foreach (var item in AlterTableClause.GetTokens(parent))
        {
            yield return item;
        }
    }

    public IEnumerable<CommonTable> GetCommonTables()
    {
        yield break;
    }

    public IEnumerable<QueryParameter> GetParameters()
    {
        yield break;
    }

    public List<AlterTableQuery> Disassemble()
    {
        //1 command equals 1 query
        var lst = new List<AlterTableQuery>();
        foreach (var item in AlterTableClause)
        {
            lst.Add(new AlterTableQuery(new AlterTableClause(AlterTableClause, item)));
        }
        return lst;
    }

    public bool TrySet(TableDefinitionClause clause)
    {
        if (AlterTableClause.Items.Count != 1) throw new InvalidOperationException();
        var cmd = AlterTableClause.Items[0];
        return cmd.TrySet(clause);
    }

    //public bool TryToIndex([MaybeNullWhen(false)] out CreateIndexQuery query)
    //{
    //	if (AlterTableClause.Items.Count != 1) throw new InvalidOperationException();
    //	var cmd = AlterTableClause.Items[0];
    //	return cmd.TryToIndex(out query);
    //}
}