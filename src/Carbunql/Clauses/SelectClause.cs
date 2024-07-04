using Carbunql.Tables;
using Carbunql.Values;
using MessagePack;

namespace Carbunql.Clauses;

/// <summary>
/// Represents a SELECT clause in a SQL query.
/// </summary>
/// <remarks>
/// The SELECT clause specifies the columns to be returned in the query result set.
/// It can include individual columns, expressions, or even aggregate functions.
/// </remarks>
[MessagePackObject(keyAsPropertyName: true)]
public class SelectClause : QueryCommandCollection<SelectableItem>, IQueryCommandable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SelectClause"/> class without any selectable items defined.
    /// To define selectable items, use the AddItem method after creating an instance.
    /// </summary>
    public SelectClause()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectClause"/> class with the specified collection of selectable items.
    /// </summary>
    /// <param name="collection">The collection of selectable items representing the columns to be selected.</param>
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
    /// Gets or sets the DISTINCT clause of the select query.
    /// </summary>
    /// <remarks>
    /// The DISTINCT clause is used to remove duplicate rows from the result set of a SELECT query.
    /// It specifies that only unique rows should be returned by the query.
    /// </remarks>
    public DistinctClause? Distinct { get; set; }

    /// <summary>
    /// Gets or sets the TOP clause of the select query.
    /// </summary>
    /// <remarks>
    /// The TOP clause is used to specify the number of rows to be returned from the result set of a SELECT query.
    /// It limits the number of rows returned by the query to a specified number.
    /// </remarks>
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
    /// Filters out the columns in the SELECT clause that are not present in the specified collection of column names.
    /// </summary>
    /// <param name="columns">The collection of column names to filter in.</param>
    public void FilterInColumns(IEnumerable<string> columns)
    {
        // Removes columns that are not present in the specified collection of column names.
        var toRemove = this.Where(column => !columns.Contains(column.Alias)).ToList();
        foreach (var item in toRemove)
        {
            Remove(item);
        }
    }

    public IEnumerable<ColumnValue> GetColumns()
    {
        if (Distinct != null)
        {
            foreach (var item in Distinct.GetColumns())
            {
                yield return item;
            }
        }

        if (Top != null)
        {
            foreach (var item in Top.GetColumns())
            {
                yield return item;
            }
        }

        foreach (var value in Items)
        {
            foreach (var item in value.GetColumns())
            {
                yield return item;
            }
        }
    }
}
