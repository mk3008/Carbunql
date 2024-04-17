using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Clauses;

[MessagePackObject(keyAsPropertyName: true)]
public class SelectClause : QueryCommandCollection<SelectableItem>, IQueryCommandable
{
    public SelectClause()
    {
    }

    public SelectClause(List<SelectableItem> collection)
    {
        Items.AddRange(collection);
    }

    [Obsolete("Using Distinct property")]
    public bool HasDistinctKeyword
    {
        get
        {
            return (Distinct != null);
        }
        set
        {
            if (value)
            {
                Distinct = new DistinctClause();
            }
            else
            {
                Distinct = null;
            }
        }
    }

    public DistinctClause? Distinct { get; set; }

    public TopClause? Top { get; set; }

    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        if (Distinct != null)
        {
            foreach (var item in Distinct.GetInternalQueries())
            {
                yield return item;
            }
        }

        if (Top != null)
        {
            foreach (var item in Top.GetInternalQueries())
            {
                yield return item;
            }
        }

        foreach (var value in Items)
        {
            foreach (var item in value.GetInternalQueries())
            {
                yield return item;
            }
        }
    }

    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        if (Distinct != null)
        {
            foreach (var item in Distinct.GetPhysicalTables())
            {
                yield return item;
            }
        }

        if (Top != null)
        {
            foreach (var item in Top.GetPhysicalTables())
            {
                yield return item;
            }
        }

        foreach (var value in Items)
        {
            foreach (var item in value.GetPhysicalTables())
            {
                yield return item;
            }
        }
    }

    public IEnumerable<CommonTable> GetCommonTables()
    {
        if (Distinct != null)
        {
            foreach (var item in Distinct.GetCommonTables())
            {
                yield return item;
            }
        }

        if (Top != null)
        {
            foreach (var item in Top.GetCommonTables())
            {
                yield return item;
            }
        }

        foreach (var value in Items)
        {
            foreach (var item in value.GetCommonTables())
            {
                yield return item;
            }
        }
    }

    public override IEnumerable<Token> GetTokens(Token? parent)
    {
        var clause = Token.Reserved(this, parent, "select");
        yield return clause;

        if (Distinct != null)
        {
            foreach (var item in Distinct.GetTokens(parent))
            {
                yield return item;
            }
        }

        if (Top != null)
        {
            foreach (var item in Top.GetTokens(parent))
            {
                yield return item;
            }
        }

        foreach (var item in base.GetTokens(clause)) yield return item;
    }

    public void FilterInColumns(IEnumerable<string> columns)
    {
        var lst = this.Where(x => !columns.Contains(x.Alias)).ToList();
        foreach (var item in lst) Remove(item);
    }
}