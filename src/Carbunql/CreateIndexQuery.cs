using Carbunql.Clauses;
using Carbunql.Tables;
using Carbunql.Values;
using MessagePack;

namespace Carbunql;

/// <summary>
/// Represents a query for creating an index.
/// </summary>
public class CreateIndexQuery : IAlterIndexQuery
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateIndexQuery"/> class with the specified index-on clause.
    /// </summary>
    /// <param name="clause">The index-on clause.</param>
    public CreateIndexQuery(IndexOnClause clause)
    {
        OnClause = clause;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the index is unique.
    /// </summary>
    public bool IsUnique { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether the index has the "if not exists" clause.
    /// </summary>
    public bool HasIfNotExists { get; set; } = false;

    /// <summary>
    /// Gets or sets the name of the index.
    /// </summary>
    public string? IndexName { get; init; } = null;

    /// <summary>
    /// Gets or sets the index-on clause.
    /// </summary>
    public IndexOnClause OnClause { get; set; }

    /// <summary>
    /// Gets or sets the where clause.
    /// </summary>
    public WhereClause? WhereClause { get; set; } = null;

    /// <summary>
    /// Gets or sets the comment clause for the index.
    /// </summary>
    [IgnoreMember]
    public CommentClause? CommentClause { get; set; }

    /// <summary>
    /// Gets the schema of the table on which the index is created.
    /// </summary>
    public string Schema => OnClause.Schema;

    /// <summary>
    /// Gets the table on which the index is created.
    /// </summary>
    public string Table => OnClause.Table;

    /// <summary>
    /// Gets internal queries.
    /// </summary>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        yield break;
    }

    /// <summary>
    /// Gets physical tables.
    /// </summary>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        yield break;
    }

    /// <summary>
    /// Gets query parameters.
    /// </summary>
    public virtual IEnumerable<QueryParameter> GetParameters()
    {
        yield break;
    }

    private Token GetCreateIndexToken(Token? parent)
    {
        return IsUnique
            ? Token.Reserved(this, parent, "create unique index")
            : Token.Reserved(this, parent, "create index");
    }

    /// <summary>
    /// Gets tokens representing the create index query.
    /// </summary>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        if (CommentClause != null)
        {
            foreach (var item in CommentClause.GetTokens(parent))
            {
                yield return item;
            }
        }

        yield return GetCreateIndexToken(parent);

        if (HasIfNotExists)
        {
            yield return Token.Reserved(this, parent, "if not exists");
        }

        if (!string.IsNullOrEmpty(IndexName))
        {
            yield return new Token(this, parent, IndexName);
        }

        if (OnClause != null)
        {
            foreach (var item in OnClause.GetTokens(parent))
            {
                yield return item;
            }
        }

        if (WhereClause != null)
        {
            foreach (var item in WhereClause.GetTokens(parent))
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Gets common tables.
    /// </summary>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        yield break;
    }

    public IEnumerable<ColumnValue> GetColumns()
    {
        if (WhereClause != null)
        {
            foreach (var item in WhereClause.GetColumns())
            {
                yield return item;
            }
        }
    }
}
