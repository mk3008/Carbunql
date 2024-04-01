﻿using Carbunql.Analysis;
using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;
using Carbunql.Values;
using MessagePack;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql;

[MessagePackObject(keyAsPropertyName: true)]
public class SelectQuery : ReadQuery, IQueryCommandable, ICommentable
{
    public SelectQuery() { }

    public SelectQuery(string query)
    {
        var q = SelectQueryParser.Parse(query);
        WithClause = q.WithClause;
        SelectClause = q.SelectClause;
        FromClause = q.FromClause;
        WhereClause = q.WhereClause;
        GroupClause = q.GroupClause;
        HavingClause = q.HavingClause;
        WindowClause = q.WindowClause;
        OperatableQueries = q.OperatableQueries;
        OrderClause = q.OrderClause;
        LimitClause = q.LimitClause;
    }

    public WithClause? WithClause { get; set; } = new();

    public SelectClause? SelectClause { get; set; }

    public FromClause? FromClause { get; set; }

    public WhereClause? WhereClause { get; set; }

    public GroupClause? GroupClause { get; set; }

    public HavingClause? HavingClause { get; set; }

    public WindowClause? WindowClause { get; set; }

    [IgnoreMember]
    public CommentClause? CommentClause { get; set; }

    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        if (CommentClause != null) foreach (var item in CommentClause.GetTokens(parent)) yield return item;

        if (SelectClause == null) yield break;

        if (parent == null && WithClause != null)
        {
            var lst = GetCommonTables();
            foreach (var item in WithClause.GetTokens(parent, lst)) yield return item;
        }
        foreach (var item in SelectClause.GetTokens(parent)) yield return item;

        if (FromClause == null) yield break;

        if (FromClause != null) foreach (var item in FromClause.GetTokens(parent)) yield return item;
        if (WhereClause != null) foreach (var item in WhereClause.GetTokens(parent)) yield return item;
        if (GroupClause != null) foreach (var item in GroupClause.GetTokens(parent)) yield return item;
        if (HavingClause != null) foreach (var item in HavingClause.GetTokens(parent)) yield return item;
        if (WindowClause != null) foreach (var item in WindowClause.GetTokens(parent)) yield return item;
    }

    public override WithClause? GetWithClause() => WithClause;

    public override SelectClause? GetSelectClause() => SelectClause;

    public override SelectQuery GetOrNewSelectQuery() => this;

    public override IEnumerable<QueryParameter> GetInnerParameters()
    {
        var q = FromClause?.GetParameters();
        if (q != null)
        {
            foreach (var item in q)
            {
                yield return item;
            }
        }
        q = WhereClause?.GetParameters();
        if (q != null)
        {
            foreach (var item in q)
            {
                yield return item;
            }
        }
        q = GroupClause?.GetParameters();
        if (q != null)
        {
            foreach (var item in q)
            {
                yield return item;
            }
        }
        q = HavingClause?.GetParameters();
        if (q != null)
        {
            foreach (var item in q)
            {
                yield return item;
            }
        }
        q = WindowClause?.GetParameters();
        if (q != null)
        {
            foreach (var item in q)
            {
                yield return item;
            }
        }
    }

    public override SelectableTable ToSelectableTable(IEnumerable<string>? columnAliases)
    {
        var vt = new VirtualTable(this);
        if (columnAliases != null)
        {
            return new SelectableTable(vt, "q", columnAliases.ToValueCollection());
        }
        return new SelectableTable(vt, "q");
    }

    public override IEnumerable<string> GetColumnNames()
    {
        if (SelectClause == null) return Enumerable.Empty<string>();
        return SelectClause.Select(x => x.Alias);
    }

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

    public IEnumerable<SelectableTable> GetSelectableTables()
    {
        if (FromClause == null) yield break;
        foreach (var item in FromClause.GetSelectableTables()) yield return item;
    }

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

    public bool TryToValuesQuery([MaybeNullWhen(false)] out ValuesQuery query)
    {
        query = default;
        if (SelectClause is null) return false;

        var row = new ValueCollection(SelectClause.Select(x => x.Value).OfType<LiteralValue>().ToList<ValueBase>());
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
