using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Clauses;

/// <summary>
/// Represents a SELECT clause in a query.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class SelectClause : QueryCommandCollection<SelectableItem>, IQueryCommandable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SelectClause"/> class.
    /// </summary>
    public SelectClause()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectClause"/> class with the specified collection of selectable items.
    /// </summary>
    /// <param name="collection">The collection of selectable items.</param>
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

    /// <summary>
    /// Gets or sets the distinct clause of the select query.
    /// </summary>
    public DistinctClause? Distinct { get; set; }

    /// <summary>
    /// Gets or sets the top clause of the select query.
    /// </summary>
    public TopClause? Top { get; set; }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <summary>
    /// Filters out the columns in the SELECT clause that are not present in the specified collection of columns.
    /// </summary>
    /// <param name="columns">The collection of column names to filter in.</param>
    public void FilterInColumns(IEnumerable<string> columns)
    {
        var lst = this.Where(x => !columns.Contains(x.Alias)).ToList();
        foreach (var item in lst) Remove(item);
    }
}
