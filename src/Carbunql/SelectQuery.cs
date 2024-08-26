using Carbunql.Analysis;
using Carbunql.Analysis.Parser;
using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;
using Carbunql.Values;
using Cysharp.Text;
using MessagePack;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql;

/// <summary>
/// Represents a class that defines a select query to retrieve data from a database.
/// Using FromClause, you can specify the table to retrieve from.
/// By specifying WhereClause, you can define retrieval conditions.
/// You can specify the columns to retrieve using SelectClause.
/// Additionally, you can specify clauses such as WithClause, GroupClause, and HavingClause.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class SelectQuery : ReadQuery, IQueryCommandable, ICommentable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SelectQuery"/> class.
    /// </summary>
    /// <remarks>
    /// This constructor initializes a new instance of the SelectQuery class in an empty state.
    /// After instantiation, additional processing is required to populate components such as the SELECT clause and FROM clause.
    /// </remarks>
    public SelectQuery() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectQuery"/> class from the provided SQL query string.
    /// </summary>
    /// <param name="query">The SQL select query string.</param>
    /// <remarks>
    /// This constructor parses the provided SQL query string and initializes the select query object with its components, such as SELECT, FROM, WHERE, GROUP BY, HAVING, and ORDER BY clauses.
    /// The query string may contain additional clauses such as WITH, WINDOW, and LIMIT, which are also parsed and initialized if present.
    /// </remarks>
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
    /// Common Table Expressions (CTE) are available.
    /// </summary>
    public WithClause? WithClause { get; set; } = new();

    /// <summary>
    /// Gets or sets the SELECT clause of the select query.
    /// The SELECT clause specifies which columns to retrieve from the database.
    /// </summary>
    public SelectClause? SelectClause { get; set; }

    /// <summary>
    /// Gets or sets the FROM clause of the select query.
    /// The FROM clause specifies the table, view, CTE (Common Table Expression), or subquery from which to retrieve data.
    /// </summary>
    public FromClause? FromClause { get; set; }

    /// <summary>
    /// Gets or sets the WHERE clause of the select query.
    /// The WHERE clause specifies the conditions that must be met for a row to be returned by the query.
    /// </summary>
    public WhereClause? WhereClause { get; set; }

    /// <summary>
    /// Gets or sets the GROUP BY clause of the select query.
    /// The GROUP BY clause is used to group rows that have the same values into summary rows.
    /// </summary>
    public GroupClause? GroupClause { get; set; }

    /// <summary>
    /// Gets or sets the HAVING clause of the select query.
    /// The HAVING clause is used to filter groups that appear in the result set, typically used in conjunction with the GROUP BY clause.
    /// </summary>
    public HavingClause? HavingClause { get; set; }

    /// <summary>
    /// Gets or sets the WINDOW clause of the select query.
    /// The WINDOW clause defines a window or set of rows for a query result.
    /// </summary>
    public WindowClause? WindowClause { get; set; }

    /// <summary>
    /// Gets or sets the comment clause of the select clause.
    /// </summary>
    /// <remarks>
    /// A comment that is output before the selection clause. It is suitable for indicating the nature of the selection query.
    /// Since it is output after the With clause, it is less visible than the HeaderCommentClause.
    /// </remarks>
    [IgnoreMember]
    public CommentClause? CommentClause { get; set; }

    /// <summary>
    /// Gets or sets the comment clause for the select query.
    /// This is printed before the With clause, which can be useful for debugging.
    /// </summary>
    /// <remarks>
    /// This is printed before the With clause and is useful for debugging.
    /// However, it is not printed if it is not the root query.
    /// </remarks>
    [IgnoreMember]
    public CommentClause? HeaderCommentClause { get; set; }

    public void AddHeaderComment(string comment)
    {
        HeaderCommentClause ??= new CommentClause();
        HeaderCommentClause.Add(comment);
    }

    public IList<IQuerySource> GetQuerySources()
    {
        var commonTables = GetCommonTables().ToList();
        var sources = new List<IQuerySource>();
        var lst = CreateQuerySources(ref sources, commonTables, new Numbering(0));

        return sources;
    }

    private IList<IQuerySource> CreateQuerySources(ref List<IQuerySource> sources, IList<CommonTable> commonTables, Numbering numbering)
    {
        if (FromClause == null) return new List<IQuerySource>();

        var currentSources = new List<IQuerySource>();

        var hasRelation = (FromClause.Relations?.Any() ?? false);
        var columns = GetColumns().ToList();

        if (hasRelation && columns.Any(x => string.IsNullOrEmpty(x.TableAlias) && x.Column != "*"))
        {
            var cols = string.Join(", ", columns.Where(x => string.IsNullOrEmpty(x.TableAlias)).Select(x => x.Column).Distinct());
            throw new InvalidProgramException($"There are columns whose table alias names cannot be parsed: {cols}.");
        }

        foreach (var source in GetQuerySourceSelectableTables())
        {
            if (sources.Where(x => x.Source == source).Any())
            {
                var qs = sources.Where(x => x.Source == source).First();
                currentSources.Add(qs);
            }
            else if (source.Table.TryGetSelectQuery(out var query))
            {
                var qs = DisassembleQuerySources(ref sources, source, query, commonTables, numbering, SourceType.SubQuery);
                currentSources.Add(qs);
            }
            else if (source.Table is PhysicalTable table && commonTables.Any(x => x.Alias == table.GetTableFullName()))
            {
                // disassemble cte
                var ct = commonTables.First(x => x.Alias == table.GetTableFullName());

                if (ct.IsSelectQuery)
                {
                    // select query

                    // Only those declared before this will be recognized as CTEs
                    var id = commonTables.IndexOf(ct);
                    var ctes = commonTables.Where((item, index) => index < id).ToList();

                    var commonQuery = commonTables.First(x => x.Alias == table.GetTableFullName()).GetSelectQuery();
                    var qs = DisassembleQuerySources(ref sources, source, commonQuery, ctes, numbering, SourceType.CommonTableExtension);
                    currentSources.Add(qs);
                }
                else if (ct.Table is VirtualTable vt && vt.Query is ValuesQuery && ct.ColumnAliases != null)
                {
                    // values query
                    var names = ct.ColumnAliases.OfType<ColumnValue>().Select(x => x.Column).ToHashSet();

                    var qs = new QuerySource(numbering.GetNext(), names, this, source, SourceType.ValuesQuery);
                    sources.Add(qs);
                    currentSources.Add(qs);
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
            else
            {
                var cname = columns
                    .Where(x => x.TableAlias == source.Alias || (!hasRelation && string.IsNullOrEmpty(x.TableAlias) && x.Column != "*"))
                    .Select(x => x.Column)
                    .ToHashSet();

                var qs = new QuerySource(numbering.GetNext(), cname, this, source, SourceType.PhysicalTable);
                sources.Add(qs);
                currentSources.Add(qs);
            }
        }

        // ex. union query
        foreach (var item in OperatableQueries.Select(x => x.Query).OfType<SelectQuery>())
        {
            foreach (var qs in item.CreateQuerySources(ref sources, commonTables, numbering))
            {
                currentSources.Add(qs);
            }
        }

        return currentSources;
    }

    private IQuerySource DisassembleQuerySources(ref List<IQuerySource> sources, SelectableTable source, SelectQuery query, IList<CommonTable> commonTables, Numbering numbering, SourceType type)
    {
        var index = numbering.GetNext();

        var parents = query.CreateQuerySources(ref sources, commonTables, numbering);

        var cname = query.GetColumnNames().ToList();

        // decode wild card
        if (cname.Contains("*"))
        {
            var allColumns = new Dictionary<string, IEnumerable<string>>();
            foreach (var nestedSource in parents)
            {
                allColumns[nestedSource.Alias] = nestedSource.ColumnNames;
            }

            cname.Remove("*");

            var wilds = query.SelectClause!.Where(x => x.Alias == "*");
            foreach (var wild in wilds.Select(x => x.Value).OfType<ColumnValue>())
            {
                if (string.IsNullOrEmpty(wild.TableAlias))
                {
                    // all
                    cname.AddRange(allColumns.SelectMany(x => x.Value));
                    break;
                }
                else
                {
                    //table all
                    cname.AddRange(allColumns[wild.TableAlias]);
                }
            }
        }

        // If not available, infer the column names from your query
        if (!cname.Any() && FromClause != null)
        {
            var hasRelation = (FromClause.Relations?.Any() ?? false);
            var columns = GetColumns().ToList();

            cname = columns
                .Where(x => x.TableAlias == source.Alias || (!hasRelation && string.IsNullOrEmpty(x.TableAlias) && x.Column != "*"))
                .Select(x => x.Column)
                .ToList();
        }

        var qs = new QuerySource(index, cname.ToHashSet(), this, source, type);

        foreach (var item in parents)
        {
            item.References.Add(qs);
        }
        sources.Add(qs);

        return qs;
    }

    private IEnumerable<SelectableTable> GetQuerySourceSelectableTables()
    {
        if (FromClause != null)
        {
            foreach (var item in FromClause.GetSelectableTables())
            {
                yield return item;
            }
        }
        if (WhereClause != null)
        {
            foreach (var item in WhereClause.GetSelectableTables())
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        // If this is a root query, a header comment is printed on the first line.
        if (parent == null && HeaderCommentClause != null)
        {
            foreach (var item in HeaderCommentClause.GetTokens(parent))
            {
                yield return item;
            }
        }

        if (parent == null && WithClause != null)
        {
            var commonTables = GetCommonTables();
            foreach (var item in WithClause.GetTokens(parent, commonTables))
            {
                yield return item;
            }
        }

        // Comments will be unified to be displayed just before the selected section.
        if (CommentClause != null)
        {
            foreach (var item in CommentClause.GetTokens(parent))
            {
                yield return item;
            }
        }

        if (SelectClause == null)
        {
            // If SelectClause is not specified,
            // all columns are assumed to be selected.
            var clause = Token.Reserved(this, parent, "select");
            yield return clause;
            yield return new Token(this, clause, "*");
        }
        else
        {
            foreach (var item in SelectClause.GetTokens(parent))
            {
                yield return item;
            }
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

    public override IEnumerable<ColumnValue> GetColumns()
    {
        if (SelectClause != null)
        {
            foreach (var item in SelectClause.GetColumns())
            {
                yield return item;
            }
        }
        if (FromClause != null)
        {
            foreach (var item in FromClause.GetColumns())
            {
                yield return item;
            }
        }
        if (WhereClause != null)
        {
            foreach (var item in WhereClause.GetColumns())
            {
                yield return item;
            }
        }
        if (GroupClause != null)
        {
            foreach (var item in GroupClause.GetColumns())
            {
                yield return item;
            }
        }
        if (HavingClause != null)
        {
            foreach (var item in HavingClause.GetColumns())
            {
                yield return item;
            }
        }
        if (WindowClause != null)
        {
            foreach (var item in WindowClause.GetColumns())
            {
                yield return item;
            }
        }
        if (OrderClause != null)
        {
            foreach (var item in OrderClause.GetColumns())
            {
                yield return item;
            }
        }
        if (LimitClause != null)
        {
            foreach (var item in LimitClause.GetColumns())
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

    private class Numbering(int startIndex)
    {
        public int Current { get; private set; } = startIndex;
        public int GetNext()
        {
            Current++;
            return Current;
        }
    }

    /// <summary>
    /// Returns a select query that references itself as a CTE (Common Table Expression).
    /// </summary>
    /// <param name="name">The name of the CTE.</param>
    /// <param name="alias">
    /// The alias to use. If omitted, the select clause will use a wildcard.
    /// </param>
    /// <returns>The modified select query with the CTE reference.</returns>
    /// <exception cref="InvalidProgramException">
    /// Thrown if a CTE with the same name already exists.
    /// </exception>
    public SelectQuery ToCTEQuery(string name, string alias = "")
    {
        if (GetCommonTables().Where(x => x.Alias.IsEqualNoCase(name)).Any())
        {
            throw new InvalidProgramException($"A CTE with the same name already exists. Name: {name}");
        }

        var (q, t) = this.ToCTE(name);

        if (string.IsNullOrEmpty(alias))
        {
            q.From(t).As(name);
            q.SelectAll();
        }
        else
        {
            q.From(t).As(alias);
            GetColumnNames().ForEach(column => q.Select($"{alias}.{column}").As(column));
        }

        return q;
    }

    /// <summary>
    /// Returns a select query that references itself as a subquery.
    /// </summary>
    /// <param name="alias">The alias to use for the subquery.</param>
    /// <returns>The modified select query with the subquery reference.</returns>
    public SelectQuery ToSubQuery(string alias)
    {
        var q = new SelectQuery();
        var (f, t) = q.From(this).As(alias);

        GetColumnNames().ForEach(column => q.Select($"{alias}.{column}").As(column));

        return q;
    }

    [Obsolete("use AddCteQuery method")]
    public SelectQuery AddCTEQuery(string query, string alias)
    {
        return AddCteQuery(query, alias);
    }

    public SelectQuery AddCteQuery(string query, string alias)
    {
        this.With(query).As(alias);
        return this;
    }

    public SelectQuery AddFrom(string table, string alias)
    {
        if (FromClause != null) throw new InvalidOperationException("FromClause is already exists.");
        FromClause = new FromClause(TableParser.Parse(table).ToSelectable(alias));
        return this;
    }

    public SelectQuery AddFrom(string table, string alias, IEnumerable<string> selectColumns)
    {
        if (FromClause != null) throw new InvalidOperationException("FromClause is already exists.");
        FromClause = new FromClause(TableParser.Parse(table).ToSelectable(alias));
        selectColumns.ForEach(x => this.Select(new ColumnValue(alias, x)));
        return this;
    }

    /// <summary>
    /// Adds a column to the select clause.
    /// </summary>
    /// <param name="column">The column information.</param>
    /// <param name="alias">The alias name for the column.</param>
    /// <returns>The modified select query.</returns>
    public SelectQuery AddSelect(string column, string alias)
    {
        this.Select(column).As(alias);
        return this;
    }

    /// <summary>
    /// Adds a column to the select clause.
    /// </summary>
    /// <param name="column">The column information.</param>
    /// <returns>The modified select query.</returns>
    public SelectQuery AddSelect(string column)
    {
        var val = ValueParser.Parse(column);
        if (val is ColumnValue c)
        {
            this.Select(c).As(c.GetDefaultName());
            return this;
        }
        throw new InvalidOperationException();
    }

    public SelectQuery AddSelectAll(string tableName)
    {
        var t = GetSelectableTables().Where(x => x.Table.GetTableFullName().IsEqualNoCase(tableName) || x.Alias.IsEqualNoCase(tableName)).FirstOrDefault();
        if (t == null)
        {
            throw new InvalidProgramException($"Table not found. table:{tableName}");
        }

        GetQuerySources()
            .Where(x => x.Source.Equals(t))
            .GetRootsBySource()
            .EnsureAny()
            .ForEach(x =>
            {
                x.ColumnNames.ForEach(column => this.Select(x.Alias, column));
            });
        return this;
    }

    public SelectQuery RenameSelect(string columnAliasName, string newAliasName)
    {
        var c = GetSelectableItems().Where(x => x.Alias.IsEqualNoCase(columnAliasName)).First();
        c.SetAlias(newAliasName);
        return this;
    }

    public SelectQuery RemoveSelect(string selectableColumnName)
    {
        var c = GetSelectableItems().Where(x => x.Alias.IsEqualNoCase(selectableColumnName)).First();
        SelectClause!.Remove(c);
        return this;
    }

    public SelectQuery RemoveSelectAll()
    {
        SelectClause = null;
        return this;
    }

    /// <summary>
    /// Searches for the specified column and overrides it.
    /// </summary>
    /// <param name="columnAliasName">The name of the column to search for.</param>
    /// <param name="overrider">The function to modify the column value.</param>
    /// <returns>The modified select query.</returns>
    public SelectQuery OverrideSelect(string columnAliasName, Func<IQuerySource, string, string> overrider)
    {
        GetQuerySources()
            .Where(x => x.Query.GetSelectableItems().Where(x => x.Alias.IsEqualNoCase(columnAliasName)).Any())
            .EnsureAny($"column alias:{columnAliasName}")
            .GetRootsBySource()
            .ForEach(x =>
            {
                var si = x.Query.GetSelectableItems().Where(x => x.Alias.IsEqualNoCase(columnAliasName)).First();
                //override
                si.Value = ValueParser.Parse(overrider(x, si.Value.ToOneLineText()));
            });

        return this;
    }

    /// <summary>
    /// Searches for the specified column in the specified table and overrides it.
    /// </summary>
    /// <param name="tableName">The name of the table to search in.</param>
    /// <param name="columnAliasName">The name of the column to search for.</param>
    /// <param name="overrider">The function to modify the column value.</param>
    /// <returns>The modified select query.</returns>
    public SelectQuery OverrideSelect(string tableName, string columnAliasName, Func<IQuerySource, SelectableItem, string> overrider)
    {
        GetQuerySources()
            .Where(x => x.GetTableFullName().IsEqualNoCase(tableName) || x.Alias.IsEqualNoCase(tableName))
            .EnsureAny($"table:{tableName}")
            .Where(x => x.Query.GetSelectableItems().Where(x => x.Alias.IsEqualNoCase(columnAliasName)).Any())
            .EnsureAny($"The table exists, but there is no corresponding column in the table. table:{tableName}, column alias:{columnAliasName}")
            .GetRootsBySource()
            .EnsureAny()
            .ForEach(x =>
            {
                var si = x.Query.GetSelectableItems().Where(x => x.Alias.IsEqualNoCase(columnAliasName)).First();
                //override
                si.Value = ValueParser.Parse(overrider(x, si));
            });

        return this;
    }

    /// <summary>
    /// Adds a search condition.
    /// </summary>
    /// <param name="columnName">The name of the column to search in.</param>
    /// <param name="adder">The function to create the search condition.</param>
    /// <returns>The modified select query.</returns>
    public SelectQuery AddWhere(string columnName, Func<IQuerySource, string> adder, bool isAliasIncluded = false)
    {
        GetQuerySources()
            .Where(x => HasColumn(x, columnName, isAliasIncluded))
            .EnsureAny($"column:{columnName}")
            .GetRootsBySource()
            .ForEach(x =>
            {
                x.Query.Where(adder(x));
            });

        return this;
    }

    /// <summary>
    /// Adds a search condition.
    /// </summary>
    /// <param name="columnName">The name of the column to search in.</param>
    /// <param name="adder">The function to create the search condition.</param>
    /// <returns>The modified select query.</returns>
    public SelectQuery AddWhere(string columnName, Func<IQuerySource, string, string> adder, bool isAliasIncluded = false)
    {
        GetQuerySources()
            .Where(x => HasColumn(x, columnName, isAliasIncluded))
            .EnsureAny($"column:{columnName}")
            .GetRootsBySource()
            .ForEach(x =>
            {
                x.Query.Where(adder(x, GetColumn(x, columnName, isAliasIncluded)));
            });

        return this;
    }

    private bool HasTable(IQuerySource source, string tableName, bool isAliasIncluded)
    {
        if (isAliasIncluded && source.Alias.IsEqualNoCase(tableName))
        {
            return true;
        }
        else
        {
            return source.GetTableFullName().IsEqualNoCase(tableName);
        }
    }

    private bool HasColumn(IQuerySource source, string columnName, bool isAliasIncluded)
    {
        if (isAliasIncluded && source.Query.GetColumnNames().Where(x => x.IsEqualNoCase(columnName)).Any())
        {
            return true;
        }
        else
        {
            return source.ColumnNames.Where(x => x.IsEqualNoCase(columnName)).Any();
        }
    }

    private string GetColumn(IQuerySource x, string columnName, bool isAliasIncluded)
    {
        if (x.Query.SelectClause != null && isAliasIncluded)
        {
            var selectableItem = x.Query.SelectClause!.Where(x => x.Alias.IsEqualNoCase(columnName)).FirstOrDefault();
            if (selectableItem != null)
            {
                return selectableItem.Value.ToOneLineText();
            }
        }

        var column = x.ColumnNames.Where(x => x.IsEqualNoCase(columnName)).FirstOrDefault();
        if (column != null)
        {
            return $"{x.Alias}.{column}";
        }

        throw new InvalidProgramException();
    }

    /// <summary>
    /// Adds a search condition.
    /// </summary>
    /// <param name="condition">search condition.</param>
    /// <returns>The modified select query.</returns>
    public SelectQuery AddWhere(string condition)
    {
        this.Where(condition);
        return this;
    }

    /// <summary>
    /// Adds a search condition.
    /// </summary>
    /// <param name="adder">The function to create the search condition.</param>
    /// <returns>The modified select query.</returns>
    public SelectQuery AddWhere(Func<string> adder)
    {
        this.Where(adder());
        return this;
    }

    /// <summary>
    /// Adds a search condition.
    /// </summary>
    /// <param name="tableName">The name of the table to search in.</param>
    /// <param name="columnName">The name of the column to search in.</param>
    /// <param name="adder">The function to create the search condition.</param>
    /// <returns>The modified select query.</returns>
    public SelectQuery AddWhere(string tableName, string columnName, Func<IQuerySource, string> adder, bool isAliasIncluded = false)
    {
        GetQuerySources()
            .Where(x => HasTable(x, tableName, true))
            .EnsureAny($"table:{tableName}")
            .Where(x => HasColumn(x, columnName, isAliasIncluded))
            .EnsureAny($"The table exists, but there is no corresponding column in the table. table:{tableName}, column:{columnName}")
            .GetRootsBySource()
            .ForEach(x =>
            {
                x.Query.Where(adder(x));
            });

        return this;
    }

    /// <summary>
    /// Adds a search condition.
    /// </summary>
    /// <param name="tableName">The name of the table to search in.</param>
    /// <param name="columnName">The name of the column to search in.</param>
    /// <param name="adder">The function to create the search condition.</param>
    /// <returns>The modified select query.</returns>
    public SelectQuery AddWhere(string tableName, string columnName, Func<IQuerySource, string, string> adder, bool isAliasIncluded = false)
    {
        GetQuerySources()
            .Where(x => HasTable(x, tableName, true))
            .EnsureAny($"table:{tableName}")
            .Where(x => HasColumn(x, columnName, isAliasIncluded))
            .EnsureAny($"The table exists, but there is no corresponding column in the table. table:{tableName}, column:{columnName}")
            .GetRootsBySource()
            .ForEach(x =>
            {
                x.Query.Where(adder(x, GetColumn(x, columnName, isAliasIncluded)));
            });

        return this;
    }

    /// <summary>
    /// Adds an EXISTS clause.
    /// </summary>
    /// <param name="keyColumnNames">The columns to use for the search condition.</param>
    /// <param name="validationTableName">The table to use for comparison within the EXISTS clause.</param>
    /// <param name="action">An optional function to process the query source, primarily used for adding comments.</param>
    /// <returns>The modified select query.</returns>
    public SelectQuery AddExists(IEnumerable<string> keyColumnNames, string validationTableName, Action<IQuerySource>? action = null)
    {
        GetQuerySources()
            .Where(x => keyColumnNames.All(keyColumn => x.ColumnNames.Contains(keyColumn)))
            .EnsureAny($"columns:{string.Join(",", keyColumnNames)}")
            .GetRootsBySource()
            .ForEach(qs =>
            {
                qs.Query.Where(() =>
                {
                    var sq = new SelectQuery($"select * from {validationTableName} as x");
                    keyColumnNames.ForEach(keyColumn => sq.Where($"x.{keyColumn} = {qs.Alias}.{keyColumn}"));
                    return sq.ToExists();
                });
                action?.Invoke(qs);
            });

        return this;
    }

    /// <summary>
    /// Adds an EXISTS clause.
    /// </summary>
    /// <param name="sourceTableName">The name of the table to search in.</param>
    /// <param name="keyColumnNames">The columns to use for the search condition.</param>
    /// <param name="validationTableName">The table to use for comparison within the EXISTS clause.</param>
    /// <param name="action">An optional function to process the query source, primarily used for adding comments.</param>
    /// <returns>The modified select query.</returns>
    public SelectQuery AddExists(string sourceTableName, IEnumerable<string> keyColumnNames, string validationTableName, Action<IQuerySource>? action = null)
    {
        GetQuerySources()
            .Where(x => HasTable(x, sourceTableName, true))
            .EnsureAny($"table:{sourceTableName}")
            .Where(x => keyColumnNames.All(keyColumn => x.ColumnNames.Contains(keyColumn)))
            .EnsureAny($"The table exists, but there is no corresponding column in the table. table:{sourceTableName}, columns:{string.Join(",", keyColumnNames)}")
            .GetRootsBySource()
            .EnsureAny()
            .ForEach(qs =>
            {
                qs.Query.Where(() =>
                {
                    var sq = new SelectQuery($"select * from {validationTableName} as x");
                    keyColumnNames.ForEach(keyColumn => sq.Where($"x.{keyColumn} = {qs.Alias}.{keyColumn}"));
                    return sq.ToExists();
                });
                action?.Invoke(qs);
            });

        return this;
    }

    /// <summary>
    /// Adds a NOT EXISTS clause.
    /// </summary>
    /// <param name="keyColumnNames">The columns to use for the search condition.</param>
    /// <param name="validationTableName">The table to use for comparison within the NOT EXISTS clause.</param>
    /// <param name="action">An optional function to process the query source, primarily used for adding comments.</param>
    /// <returns>The modified select query.</returns>
    public SelectQuery AddNotExists(IEnumerable<string> keyColumnNames, string validationTableName, Action<IQuerySource>? action = null)
    {
        GetQuerySources()
            .Where(x => keyColumnNames.All(keyColumn => x.ColumnNames.Contains(keyColumn)))
            .EnsureAny($"columns:{string.Join(",", keyColumnNames)}")
            .GetRootsBySource()
            .ForEach(qs =>
            {
                qs.Query.Where(() =>
                {
                    var sq = new SelectQuery($"select * from {validationTableName} as x");
                    keyColumnNames.ForEach(keyColumn => sq.Where($"x.{keyColumn} = {qs.Alias}.{keyColumn}"));
                    return sq.ToNotExists();
                });
                action?.Invoke(qs);
            });

        return this;
    }

    /// <summary>
    /// Adds a NOT EXISTS clause.
    /// </summary>
    /// <param name="sourceTableName">The name of the table to search in.</param>
    /// <param name="keyColumnNames">The columns to use for the search condition.</param>
    /// <param name="validationTableName">The table to use for comparison within the NOT EXISTS clause.</param>
    /// <param name="action">An optional function to process the query source, primarily used for adding comments.</param>
    /// <returns>The modified select query.</returns>
    public SelectQuery AddNotExists(string sourceTableName, IEnumerable<string> keyColumnNames, string validationTableName, Action<IQuerySource>? action = null)
    {
        GetQuerySources()
            .Where(x => HasTable(x, sourceTableName, true))
            .EnsureAny($"table:{sourceTableName}")
            .Where(x => keyColumnNames.All(keyColumn => x.ColumnNames.Contains(keyColumn)))
            .EnsureAny($"The table exists, but there is no corresponding column in the table. table:{sourceTableName}, columns:{string.Join(",", keyColumnNames)}")
            .GetRootsBySource()
            .ForEach(qs =>
            {
                qs.Query.Where(() =>
                {
                    var sq = new SelectQuery($"select * from {validationTableName} as x");
                    keyColumnNames.ForEach(keyColumn => sq.Where($"x.{keyColumn} = {qs.Alias}.{keyColumn}"));
                    return sq.ToNotExists();
                });
                action?.Invoke(qs);
            });

        return this;
    }

    public SelectQuery AddJoin(string joinType, string table, string alias, string condition)
    {
        var f = FromClause!;
        var t = TableParser.Parse(table);
        var r = f.Join(t.ToSelectable(alias), joinType);
        r.On(condition);
        return this;
    }

    public SelectQuery AddJoin(string joinType, string table, string alias, Func<string> builder)
    {
        var f = FromClause!;
        var t = TableParser.Parse(table);
        var r = f.Join(t.ToSelectable(alias), joinType);
        r.On(builder);
        return this;
    }

    public SelectQuery AddSelectQuery(string jointType, Func<SelectQuery, SelectQuery> builder)
    {
        AddOperatableValue(jointType, builder(this));
        return this;
    }

    public SelectQuery ImportCTEQueries(SelectQuery from)
    {
        from.GetCommonTables().ForEach(x => this.With(x));
        return this;
    }

    public SelectQuery AddParameter(QueryParameter prm)
    {
        Parameters.Add(prm);
        return this;
    }

    public SelectQuery AddOrder(string columnName, Func<IQuerySource, string, string> adder, bool isAliasIncluded = false)
    {
        GetQuerySources()
            .Where(x => x.Query.Equals(this))
            .ForEach(x =>
            {
                x.Query.OrderClause ??= new();
                x.Query.OrderClause.Add(SortableItemParser.Parse(adder(x, GetColumn(x, columnName, isAliasIncluded))));
            });

        return this;
    }

    public SelectQuery FilterInColumns(IEnumerable<string> columnNames)
    {
        if (SelectClause == null) throw new NullReferenceException(nameof(SelectClause));
        SelectClause.FilterInColumns(columnNames);
        return this;
    }

    public string GetParameterText()
    {
        var prms = GetParameters();
        if (!prms.Any()) return string.Empty;

        var names = new List<string>();

        var sb = ZString.CreateStringBuilder();
        sb.AppendLine("/*");
        foreach (var item in prms)
        {
            if (names.Contains(item.ParameterName)) continue;

            names.Add(item.ParameterName);
            if (item.Value == null)
            {
                sb.AppendLine($"  {item.ParameterName} is NULL");
            }
            else if (item.Value.GetType() == typeof(string) || item.Value.GetType() == typeof(DateTime))
            {
                sb.AppendLine($"  {item.ParameterName} = '{item.Value}'");
            }
            else
            {
                sb.AppendLine($"  {item.ParameterName} = {item.Value}");
            }
        }
        sb.Append("*/");

        return sb.ToString();
    }
}
