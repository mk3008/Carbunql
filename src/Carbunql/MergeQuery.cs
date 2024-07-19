using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Tables;
using Carbunql.Values;

namespace Carbunql;

/// <summary>
/// Represents a merge query.
/// </summary>
public class MergeQuery : IQueryCommandable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MergeQuery"/> class.
    /// </summary>
    /// <param name="datasource">The data source.</param>
    /// <param name="destinationTable">The destination table.</param>
    /// <param name="keys">The keys for merging.</param>
    /// <param name="datasourceAlias">The alias for the data source.</param>
    /// <param name="destinationAlias">The alias for the destination.</param>
    public MergeQuery(IReadQuery datasource, string destinationTable, IEnumerable<string> keys, string datasourceAlias = "s", string destinationAlias = "d")
    {
        var destination = CreateDestination(datasource, destinationTable, keys, destinationAlias);

        Datasource = datasource;
        DatasourceAlias = datasourceAlias;

        if (datasource.GetCommonTables().Any())
        {
            WithClause = new WithClause();
            datasource.GetCommonTables().ForEach(WithClause.Add);     
        }

        UsingClause = CreateUsingClause(datasource, keys, destination.Alias, datasourceAlias);
        MergeClause = new MergeClause(destination);
        Parameters = datasource.GetParameters();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MergeQuery"/> class.
    /// </summary>
    /// <param name="datasource">The datasource for the merge query.</param>
    /// <param name="destination">The destination for the merge query.</param>
    /// <param name="keys">The keys for the merge query.</param>
    /// <param name="datasourceAlias">The alias for the datasource.</param>
    public MergeQuery(IReadQuery datasource, SelectableTable destination, IEnumerable<string> keys, string datasourceAlias = "s")
    {
        Datasource = datasource;
        DatasourceAlias = datasourceAlias;

        if (datasource.GetCommonTables().Any())
        {
            WithClause = new WithClause();
            datasource.GetCommonTables().ForEach(WithClause.Add);
        }

        UsingClause = CreateUsingClause(datasource, keys, destination.Alias, datasourceAlias);
        MergeClause = new MergeClause(destination);
        Parameters = datasource.GetParameters();
    }

    /// <summary>
    /// Gets or sets the with clause for the merge query.
    /// </summary>
    public WithClause? WithClause { get; set; }

    /// <summary>
    /// Gets or sets the merge clause for the merge query.
    /// </summary>
    public MergeClause MergeClause { get; set; }

    /// <summary>
    /// Gets or sets the using clause for the merge query.
    /// </summary>
    public UsingClause UsingClause { get; set; }

    /// <summary>
    /// Gets or sets the when clause for the merge query.
    /// </summary>
    public WhenClause? WhenClause { get; set; }

    /// <summary>
    /// Gets or sets the parameters of the merge query.
    /// </summary>
    public IEnumerable<QueryParameter>? Parameters { get; set; }

    /// <summary>
    /// Gets or sets the datasource for the merge query.
    /// </summary>
    public IReadQuery Datasource { get; init; }

    /// <summary>
    /// Gets or sets the alias for the datasource.
    /// </summary>
    public string DatasourceAlias { get; set; }

    /// <summary>
    /// Retrieves the internal queries associated with the merge query.
    /// </summary>
    /// <returns>The internal queries associated with the merge query.</returns>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        if (WithClause != null)
        {
            foreach (var item in WithClause.GetInternalQueries())
            {
                yield return item;
            }
        }
        foreach (var item in MergeClause.GetInternalQueries())
        {
            yield return item;
        }
        foreach (var item in UsingClause.GetInternalQueries())
        {
            yield return item;
        }
        if (WhenClause != null)
        {
            foreach (var item in WhenClause.GetInternalQueries())
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Retrieves the physical tables associated with the merge query.
    /// </summary>
    /// <returns>The physical tables associated with the merge query.</returns>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        if (WithClause != null)
        {
            foreach (var item in WithClause.GetPhysicalTables())
            {
                yield return item;
            }
        }
        foreach (var item in MergeClause.GetPhysicalTables())
        {
            yield return item;
        }
        foreach (var item in UsingClause.GetPhysicalTables())
        {
            yield return item;
        }
        if (WhenClause != null)
        {
            foreach (var item in WhenClause.GetPhysicalTables())
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Retrieves the common tables associated with the merge query.
    /// </summary>
    /// <returns>The common tables associated with the merge query.</returns>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        if (WithClause != null)
        {
            foreach (var item in WithClause.GetCommonTables())
            {
                yield return item;
            }
        }
        foreach (var item in MergeClause.GetCommonTables())
        {
            yield return item;
        }
        foreach (var item in UsingClause.GetCommonTables())
        {
            yield return item;
        }
        if (WhenClause != null)
        {
            foreach (var item in WhenClause.GetCommonTables())
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Retrieves the parameters of the merge query.
    /// </summary>
    /// <returns>The parameters of the merge query.</returns>
    public virtual IEnumerable<QueryParameter> GetParameters()
    {
        if (WithClause != null)
        {
            foreach (var item in WithClause.GetParameters())
            {
                yield return item;
            }
        }
        if (MergeClause != null)
        {
            foreach (var item in MergeClause.GetParameters())
            {
                yield return item;
            }
        }
        if (UsingClause != null)
        {
            foreach (var item in UsingClause.GetParameters())
            {
                yield return item;
            }
        }
        if (WhenClause != null)
        {
            foreach (var item in WhenClause.GetParameters())
            {
                yield return item;
            }
        }
        if (Parameters != null)
        {
            foreach (var item in Parameters)
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Retrieves the tokens associated with the merge query.
    /// </summary>
    /// <param name="parent">The parent token.</param>
    /// <returns>The tokens associated with the merge query.</returns>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        if (WhenClause == null) throw new NullReferenceException();

        if (WithClause != null) foreach (var item in WithClause.GetTokens(parent)) yield return item;
        foreach (var item in MergeClause.GetTokens(parent)) yield return item;
        foreach (var item in UsingClause.GetTokens(parent)) yield return item;
        foreach (var item in WhenClause.GetTokens(parent)) yield return item;
    }

    /// <summary>
    /// Creates the destination table for the merge query.
    /// </summary>
    private SelectableTable CreateDestination(IReadQuery datasource, string destinationTable, IEnumerable<string> keys, string destinationAlias)
    {
        var s = datasource.GetSelectClause();
        if (s == null) throw new NotSupportedException("Select clause is not found.");
        return new SelectableTable(new PhysicalTable(destinationTable), destinationAlias);
    }

    /// <summary>
    /// Creates the using clause for the merge query.
    /// </summary>
    private UsingClause CreateUsingClause(IReadQuery datasource, IEnumerable<string> keys, string destinationName, string sourceName)
    {
        var s = datasource.GetSelectClause();
        if (s == null) throw new NotSupportedException("Select clause is not found.");
        var cols = s.Where(x => keys.Contains(x.Alias)).Select(x => x.Alias).ToList();

        ValueBase? v = null;
        foreach (var item in cols)
        {
            if (v == null)
            {
                v = new ColumnValue(destinationName, item);
            }
            else
            {
                v.And(new ColumnValue(destinationName, item));
            }
            v.Equal(new ColumnValue(sourceName, item));
        };
        if (v == null) throw new Exception();

        return new UsingClause(datasource.ToSelectableTable(sourceName), v, keys);
    }

    public IEnumerable<ColumnValue> GetColumns()
    {
        if (WithClause != null)
        {
            foreach (var item in WithClause.GetColumns())
            {
                yield return item;
            }
        }
        foreach (var item in MergeClause.GetColumns())
        {
            yield return item;
        }
        foreach (var item in UsingClause.GetColumns())
        {
            yield return item;
        }
        if (WhenClause != null)
        {
            foreach (var item in WhenClause.GetColumns())
            {
                yield return item;
            }
        }
    }
}
