using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Tables;
using MessagePack;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql;

/// <summary>
/// Represents a query for inserting data into a table.
/// </summary>
public class InsertQuery : IQueryCommandable, IReturning, ICommentable
{
    /// <summary>
    /// Gets or sets the insert clause.
    /// </summary>
    public InsertClause? InsertClause { get; set; }

    /// <summary>
    /// Gets or sets the returning clause.
    /// </summary>
    public ReturningClause? ReturningClause { get; set; }

    /// <summary>
    /// Gets or sets the comment clause.
    /// </summary>
    [IgnoreMember]
    public CommentClause? CommentClause { get; set; }

    /// <summary>
    /// Gets or sets the query used to provide values for insertion.
    /// </summary>
    public IReadQuery? Query { get; set; }

    /// <summary>
    /// Retrieves internal queries.
    /// </summary>
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

    /// <summary>
    /// Retrieves physical tables.
    /// </summary>
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

    /// <summary>
    /// Retrieves common tables.
    /// </summary>
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

    /// <summary>
    /// Retrieves query parameters.
    /// </summary>
    public IEnumerable<QueryParameter>? Parameters { get; set; }

    /// <summary>
    /// Retrieves query parameters.
    /// </summary>
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

    /// <summary>
    /// Retrieves tokens.
    /// </summary>
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

    /// <summary>
    /// Tries to convert to an insert query from a values query.
    /// </summary>
    public bool TryConvertToInsertSelect([MaybeNullWhen(false)] out InsertQuery insertQuery)
    {
        insertQuery = default;
        if (Query is ValuesQuery v && InsertClause != null && InsertClause.ColumnAliases != null)
        {
            var names = InsertClause.ColumnAliases.GetColumnNames();
            var iq = new InsertQuery();
            iq.InsertClause = InsertClause;
            iq.Query = v.ToPlainSelectQuery(names);
            insertQuery = iq;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Tries to convert to an insert query from a select query.
    /// </summary>
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
