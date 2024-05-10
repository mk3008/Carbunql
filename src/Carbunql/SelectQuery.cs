using Carbunql.Analysis;
using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;
using Carbunql.Values;
using MessagePack;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql;

/// <summary>
/// Represents a class that defines a select query.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class SelectQuery : ReadQuery, IQueryCommandable, ICommentable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SelectQuery"/> class.
    /// </summary>
    public SelectQuery() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectQuery"/> class from the provided query string.
    /// </summary>
    /// <param name="query">The select query string.</param>
    public SelectQuery(string query)
    {
        var parsedQuery = SelectQueryParser.Parse(query);
        WithClause = parsedQuery.WithClause;
        SelectClause = parsedQuery.SelectClause;
        FromClause = parsedQuery.FromClause;
        WhereClause = parsedQuery.WhereClause;
        GroupClause = parsedQuery.GroupClause;
        HavingClause = parsedQuery.HavingClause;
        WindowClause = parsedQuery.WindowClause;
        OperatableQueries = parsedQuery.OperatableQueries;
        OrderClause = parsedQuery.OrderClause;
        LimitClause = parsedQuery.LimitClause;
    }

    /// <summary>
    /// Gets or sets the WITH clause of the select query.
    /// </summary>
    public WithClause? WithClause { get; set; } = new();

    /// <summary>
    /// Gets or sets the SELECT clause of the select query.
    /// </summary>
    public SelectClause? SelectClause { get; set; }

    /// <summary>
    /// Gets or sets the FROM clause of the select query.
    /// </summary>
    public FromClause? FromClause { get; set; }

    /// <summary>
    /// Gets or sets the WHERE clause of the select query.
    /// </summary>
    public WhereClause? WhereClause { get; set; }

    /// <summary>
    /// Gets or sets the GROUP BY clause of the select query.
    /// </summary>
    public GroupClause? GroupClause { get; set; }

    /// <summary>
    /// Gets or sets the HAVING clause of the select query.
    /// </summary>
    public HavingClause? HavingClause { get; set; }

    /// <summary>
    /// Gets or sets the WINDOW clause of the select query.
    /// </summary>
    public WindowClause? WindowClause { get; set; }

    /// <summary>
    /// Gets or sets the comment clause of the select query.
    /// </summary>
    [IgnoreMember]
    public CommentClause? CommentClause { get; set; }

    /// <inheritdoc/>
    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        if (CommentClause != null)
        {
            foreach (var item in CommentClause.GetTokens(parent))
            {
                yield return item;
            }
        }

        if (SelectClause == null) yield break;

        if (parent == null && WithClause != null)
        {
            var commonTables = GetCommonTables();
            foreach (var item in WithClause.GetTokens(parent, commonTables))
            {
                yield return item;
            }
        }

        foreach (var item in SelectClause.GetTokens(parent))
        {
            yield return item;
        }

        if (FromClause == null) yield break;

        if (FromClause != null)
        {
            foreach (var item in FromClause.GetTokens(parent))
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

        if (GroupClause != null)
        {
            foreach (var item in GroupClause.GetTokens(parent))
            {
                yield return item;
            }
        }

        if (HavingClause != null)
        {
            foreach (var item in HavingClause.GetTokens(parent))
            {
                yield return item;
            }
        }

        if (WindowClause != null)
        {
            foreach (var item in WindowClause.GetTokens(parent))
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    public override WithClause? GetWithClause() => WithClause;

    /// <inheritdoc/>
    public override SelectClause? GetSelectClause() => SelectClause;

    /// <inheritdoc/>
    public override SelectQuery GetOrNewSelectQuery() => this;

    /// <inheritdoc/>
    public override IEnumerable<QueryParameter> GetInnerParameters()
    {
        var fromClauseParameters = FromClause?.GetParameters();
        if (fromClauseParameters != null)
        {
            foreach (var item in fromClauseParameters)
            {
                yield return item;
            }
        }

        var whereClauseParameters = WhereClause?.GetParameters();
        if (whereClauseParameters != null)
        {
            foreach (var item in whereClauseParameters)
            {
                yield return item;
            }
        }

        var groupClauseParameters = GroupClause?.GetParameters();
        if (groupClauseParameters != null)
        {
            foreach (var item in groupClauseParameters)
            {
                yield return item;
            }
        }

        var havingClauseParameters = HavingClause?.GetParameters();
        if (havingClauseParameters != null)
        {
            foreach (var item in havingClauseParameters)
            {
                yield return item;
            }
        }

        var windowClauseParameters = WindowClause?.GetParameters();
        if (windowClauseParameters != null)
        {
            foreach (var item in windowClauseParameters)
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    public override SelectableTable ToSelectableTable(IEnumerable<string>? columnAliases)
    {
        var vt = new VirtualTable(this);
        if (columnAliases != null)
        {
            return new SelectableTable(vt, "q", columnAliases.ToValueCollection());
        }
        return new SelectableTable(vt, "q");
    }

    /// <inheritdoc/>
    public override IEnumerable<string> GetColumnNames()
    {
        if (SelectClause == null) return Enumerable.Empty<string>();
        return SelectClause.Select(x => x.Alias);
    }

    /// <inheritdoc/>
    public override IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        if (WithClause != null)
        {
            foreach (var item in WithClause.GetPhysicalTables())
            {
                yield return item;
            }
        }

        if (SelectClause != null)
        {
            foreach (var item in SelectClause.GetPhysicalTables())
            {
                yield return item;
            }
        }
        if (FromClause != null)
        {
            foreach (var item in FromClause.GetPhysicalTables())
            {
                yield return item;
            }
        }
        if (WhereClause != null)
        {
            foreach (var item in WhereClause.GetPhysicalTables())
            {
                yield return item;
            }
        }
        if (GroupClause != null)
        {
            foreach (var item in GroupClause.GetPhysicalTables())
            {
                yield return item;
            }
        }
        if (HavingClause != null)
        {
            foreach (var item in HavingClause.GetPhysicalTables())
            {
                yield return item;
            }
        }
        if (WindowClause != null)
        {
            foreach (var item in WindowClause.GetPhysicalTables())
            {
                yield return item;
            }
        }
        foreach (var oq in OperatableQueries)
        {
            foreach (var item in oq.GetPhysicalTables())
            {
                yield return item;
            }
        }
        if (OrderClause != null)
        {
            foreach (var item in OrderClause.GetPhysicalTables())
            {
                yield return item;
            }
        }
        if (LimitClause != null)
        {
            foreach (var item in LimitClause.GetPhysicalTables())
            {
                yield return item;
            }
        }
    }


    /// <inheritdoc/>
    public override IEnumerable<SelectQuery> GetInternalQueries()
    {
        if (WithClause != null)
        {
            foreach (var item in WithClause.GetInternalQueries())
            {
                yield return item;
            }
        }

        yield return this;

        if (SelectClause != null)
        {
            foreach (var item in SelectClause.GetInternalQueries())
            {
                yield return item;
            }
        }
        if (FromClause != null)
        {
            foreach (var item in FromClause.GetInternalQueries())
            {
                yield return item;
            }
        }
        if (WhereClause != null)
        {
            foreach (var item in WhereClause.GetInternalQueries())
            {
                yield return item;
            }
        }
        if (GroupClause != null)
        {
            foreach (var item in GroupClause.GetInternalQueries())
            {
                yield return item;
            }
        }
        if (HavingClause != null)
        {
            foreach (var item in HavingClause.GetInternalQueries())
            {
                yield return item;
            }
        }
        if (WindowClause != null)
        {
            foreach (var item in WindowClause.GetInternalQueries())
            {
                yield return item;
            }
        }
        foreach (var oq in OperatableQueries)
        {
            foreach (var item in oq.GetInternalQueries())
            {
                yield return item;
            }
        }
        if (OrderClause != null)
        {
            foreach (var item in OrderClause.GetInternalQueries())
            {
                yield return item;
            }
        }
        if (LimitClause != null)
        {
            foreach (var item in LimitClause.GetInternalQueries())
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<SelectableItem> GetSelectableItems()
    {
        if (SelectClause != null)
        {
            foreach (var item in SelectClause)
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<SelectableTable> GetSelectableTables()
    {
        if (FromClause == null) yield break;
        foreach (var item in FromClause.GetSelectableTables()) yield return item;
    }

    /// <inheritdoc/>
    public override IEnumerable<CommonTable> GetCommonTables()
    {
        if (WithClause != null)
        {
            foreach (var item in WithClause.GetCommonTables())
            {
                yield return item;
            }
        }
        if (SelectClause != null)
        {
            foreach (var item in SelectClause.GetCommonTables())
            {
                yield return item;
            }
        }
        if (FromClause != null)
        {
            foreach (var item in FromClause.GetCommonTables())
            {
                yield return item;
            }
        }
        if (WhereClause != null)
        {
            foreach (var item in WhereClause.GetCommonTables())
            {
                yield return item;
            }
        }
        if (GroupClause != null)
        {
            foreach (var item in GroupClause.GetCommonTables())
            {
                yield return item;
            }
        }
        if (HavingClause != null)
        {
            foreach (var item in HavingClause.GetCommonTables())
            {
                yield return item;
            }
        }
        if (WindowClause != null)
        {
            foreach (var item in WindowClause.GetCommonTables())
            {
                yield return item;
            }
        }
        foreach (var oq in OperatableQueries)
        {
            foreach (var item in oq.GetCommonTables())
            {
                yield return item;
            }
        }
        if (OrderClause != null)
        {
            foreach (var item in OrderClause.GetCommonTables())
            {
                yield return item;
            }
        }
        if (LimitClause != null)
        {
            foreach (var item in LimitClause.GetCommonTables())
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// This method tries to convert the SelectQuery instance to a ValuesQuery instance.
    /// </summary>
    /// <param name="query">The resulting ValuesQuery if conversion succeeds; otherwise, default.</param>
    /// <returns>True if the conversion succeeds; otherwise, false.</returns>
    public bool TryToValuesQuery([MaybeNullWhen(false)] out ValuesQuery query)
    {
        query = default;
        if (SelectClause is null) return false;

        var row = new ValueCollection(SelectClause.Select(x => x.Value).ToList<ValueBase>());
        var q = new ValuesQuery();
        q.Rows.Add(row);

        foreach (var item in OperatableQueries)
        {
            if (item.Query is SelectQuery sq)
            {
                if (sq.TryToValuesQuery(out var vq))
                {
                    q.Rows.AddRange(vq.Rows);
                }
                else
                {
                    return false;
                }
            }
            else if (item.Query is ValuesQuery vq)
            {
                q.Rows.AddRange(vq.Rows);
            }
        }

        // Conversion is not possible if the number of columns is different.
        if (q.Rows.Min(x => x.Count()) != q.Rows.Max(x => x.Count()))
        {
            return false;
        }

        query = q;
        return true;
    }
}
