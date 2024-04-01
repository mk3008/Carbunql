using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Tables;
using MessagePack;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql;

public class InsertQuery : IQueryCommandable, IReturning, ICommentable
{
    public InsertClause? InsertClause { get; set; }

    public ReturningClause? ReturningClause { get; set; }

    [IgnoreMember]
    public CommentClause? CommentClause { get; set; }

    public IReadQuery? Query { get; set; }

    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        if (Query != null)
        {
            foreach (var item in Query.GetInternalQueries())
            {
                yield return item;
            }
        }
    }

    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        if (Query != null)
        {
            foreach (var item in Query.GetPhysicalTables())
            {
                yield return item;
            }
        }
    }

    public IEnumerable<CommonTable> GetCommonTables()
    {
        if (Query != null)
        {
            foreach (var item in Query.GetCommonTables())
            {
                yield return item;
            }
        }
    }

    public IEnumerable<QueryParameter>? Parameters { get; set; }

    public virtual IEnumerable<QueryParameter> GetParameters()
    {
        if (Parameters != null)
        {
            foreach (var item in Parameters)
            {
                yield return item;
            }
        }
    }

    public IEnumerable<Token> GetTokens(Token? parent)
    {
        if (Query == null) throw new NullReferenceException(nameof(Query));
        if (InsertClause == null) throw new NullReferenceException(nameof(InsertClause));

        if (CommentClause != null) foreach (var item in CommentClause.GetTokens(parent)) yield return item;

        foreach (var item in InsertClause.GetTokens(parent)) yield return item;
        foreach (var item in Query.GetTokens(parent)) yield return item;

        if (ReturningClause == null) yield break;
        foreach (var item in ReturningClause.GetTokens(parent)) yield return item;
    }

    public bool TryConvertToInsertSelect([MaybeNullWhen(false)] out InsertQuery insertQuery)
    {
        insertQuery = default;
        if (Query is ValuesQuery v && InsertClause != null && InsertClause.Table.Table is FunctionTable ft)
        {
            var names = ft.Argument.GetValues().Select(x => x.ToText()).ToList();
            var iq = new InsertQuery();
            iq.InsertClause = InsertClause;
            iq.Query = v.ToPlainSelectQuery(names);
            insertQuery = iq;
            return true;
        }
        return false;
    }

    public bool TryConvertToInsertValues([MaybeNullWhen(false)] out InsertQuery insertQuery)
    {
        insertQuery = default;
        if (Query is SelectQuery sq && sq.FromClause is null && sq.SelectClause is not null)
        {
            if (sq.TryToValuesQuery(out var values))
            {
                var iq = new InsertQuery();
                iq.InsertClause = InsertClause;
                iq.Query = values;
                insertQuery = iq;
                return true;
            }
        }
        return false;
    }
}