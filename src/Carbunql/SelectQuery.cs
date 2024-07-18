using Carbunql.Analysis;
using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;
using Carbunql.Values;
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

		if (hasRelation && columns.Any(x => string.IsNullOrEmpty(x.TableAlias)))
		{
			var cols = string.Join(", ", columns.Where(x => string.IsNullOrEmpty(x.TableAlias)).Select(x => x.Column));
			throw new InvalidProgramException($"There are columns whose table alias names cannot be parsed: {cols}.");
		}

		foreach (var source in FromClause.GetSelectableTables())
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
					var commonQuery = commonTables.First(x => x.Alias == table.GetTableFullName()).GetSelectQuery();
					var qs = DisassembleQuerySources(ref sources, source, commonQuery, commonTables, numbering, SourceType.CommonTableExtension);
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
					.Where(x => !hasRelation || x.TableAlias == source.Alias)
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

		var qs = new QuerySource(index, cname.ToHashSet(), this, source, type);

		foreach (var item in parents)
		{
			item.References.Add(qs);
		}
		sources.Add(qs);

		return qs;
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

	public SelectQuery ToSubQuery(string alias)
	{
		var q = new SelectQuery();
		var (f, t) = q.From(this).As(alias);

		GetColumnNames().ForEach(column => q.Select($"{alias}.{column}").As(column));

		return q;
	}

	public SelectQuery AddColumn(string column, string alias)
	{
		this.Select(column).As(alias);
		return this;
	}
}
