using Carbunql.Analysis;
using Carbunql.Analysis.Parser;
using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;
using Carbunql.Values;

namespace Carbunql;

/// <summary>
/// Represents a query that retrieves multiple rows of values from a database.
/// This type of query is typically used for inserting multiple rows of data into a table.
/// </summary>
public class ValuesQuery : ReadQuery
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValuesQuery"/> class.
    /// </summary>
    /// <remarks>
    /// This constructor initializes a new instance of the ValuesQuery class in an empty state.
    /// After instantiation, additional processing is required to populate the Rows property with values to be inserted.
    /// </remarks>
    public ValuesQuery()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValuesQuery"/> class with specified rows.
    /// </summary>
    /// <param name="rows">The rows to be inserted.</param>
    public ValuesQuery(List<ValueCollection> rows)
    {
        Rows = rows;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValuesQuery"/> class from the specified query string.
    /// </summary>
    /// <param name="query">The query string to parse.</param>
    /// <remarks>
    /// This constructor initializes a new instance of the ValuesQuery class by parsing the provided query string.
    /// The parsed components, such as the rows to be inserted, operatable queries, ORDER BY clause, and LIMIT clause, are assigned to the corresponding properties of the ValuesQuery object.
    /// </remarks>
    public ValuesQuery(string query)
    {
        var q = ValuesQueryParser.Parse(query);
        Rows = q.Rows;
        OperatableQueries = q.OperatableQueries;
        OrderClause = q.OrderClause;
        LimitClause = q.LimitClause;
    }

    [Obsolete("This feature has been deprecated. Consider building it outside of class.")]
    public ValuesQuery(IEnumerable<object> matrix)
    {
        foreach (var row in matrix)
        {
            var lst = new List<ValueBase>();

            foreach (var column in row.GetType().GetProperties().Where(x => x.CanRead && x.CanWrite).Select(x => x.GetValue(row)))
            {
                if (column == null || column.ToString() == null)
                {
                    lst.Add(new LiteralValue("null"));
                }
                else
                {
                    var v = $"\"{column}\"";
                    lst.Add(ValueParser.Parse(column.ToString()!));
                }
            }
            Rows.Add(new ValueCollection(lst));
        }
    }

    [Obsolete("This feature has been deprecated. Consider building it outside of class.")]
    public ValuesQuery(IEnumerable<object> matrix, string placeholderIndentifer)
    {
        var r = 0;

        foreach (var row in matrix)
        {
            var lst = new List<ValueBase>();
            var c = 0;
            foreach (var column in row.GetType().GetProperties().Where(x => x.CanRead && x.CanWrite).Select(x => x.GetValue(row)))
            {
                var name = $"r{r}c{c}";
                var v = placeholderIndentifer + name;
                lst.Add(ValueParser.Parse(v));
                AddParameter(name, column);
                c++;
            }
            Rows.Add(new ValueCollection(lst));
            r++;
        }
    }

    [Obsolete("This feature has been deprecated. Consider building it outside of class.")]
    public ValuesQuery(string[,] matrix)
    {
        for (int row = 0; row < matrix.GetLength(0); row++)
        {
            var lst = new List<ValueBase>();

            for (int column = 0; column < matrix.GetLength(1); column++)
            {
                var v = $"\"{matrix[row, column]}\"";
                lst.Add(ValueParser.Parse(v));
            }
            Rows.Add(new ValueCollection(lst));
        }
    }

    [Obsolete("This feature has been deprecated. Consider building it outside of class.")]
    public ValuesQuery(IEnumerable<IEnumerable<string>> matrix)
    {
        foreach (var row in matrix)
        {
            var lst = new List<ValueBase>();
            foreach (var column in row)
            {
                var v = $"\"{column}\"";
                lst.Add(ValueParser.Parse(v));
            }
            Rows.Add(new ValueCollection(lst));
        }
    }

    [Obsolete("This feature has been deprecated. Consider building it outside of class.")]
    public ValuesQuery(string[,] matrix, string placeholderIndentifer)
    {
        for (int row = 0; row < matrix.GetLength(0); row++)
        {
            var lst = new List<ValueBase>();

            for (int column = 0; column < matrix.GetLength(1); column++)
            {
                var name = $"r{row}c{column}";
                var v = placeholderIndentifer + name;
                lst.Add(ValueParser.Parse(v));
                AddParameter(name, matrix[row, column]);
            }
            Rows.Add(new ValueCollection(lst));
        }
    }

    [Obsolete("This feature has been deprecated. Consider building it outside of class.")]
    public ValuesQuery(IEnumerable<IEnumerable<string>> matrix, string placeholderIndentifer)
    {
        var r = 0;
        foreach (var row in matrix)
        {
            var c = 0;
            var lst = new List<ValueBase>();
            foreach (var column in row)
            {
                var name = $"r{r}c{c}";
                var v = placeholderIndentifer + name;
                lst.Add(ValueParser.Parse(v));
                AddParameter(name, column);
                c++;
            }
            Rows.Add(new ValueCollection(lst));
            r++;
        }
    }

    /// <summary>
    /// Gets or sets the rows to be inserted.
    /// </summary>
    public List<ValueCollection> Rows { get; init; } = new();

    /// <inheritdoc/>
    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        var clause = Token.Reserved(this, parent, "values");
        yield return clause;

        var isFirst = true;
        foreach (var item in Rows)
        {
            if (isFirst)
            {
                isFirst = false;
            }
            else
            {
                yield return Token.Comma(this, clause);
            }
            var bracket = Token.ReservedBracketStart(this, clause);
            yield return bracket;
            foreach (var token in item.GetTokens(bracket)) yield return token;
            yield return Token.ReservedBracketEnd(this, clause);
        }
    }

    /// <inheritdoc/>
    public override WithClause? GetWithClause() => null;

    /// <inheritdoc/>
    public override SelectClause? GetSelectClause() => null;

    /// <inheritdoc/>
    public override IEnumerable<SelectQuery> GetInternalQueries()
    {
        foreach (var row in Rows)
        {
            foreach (var item in row.GetInternalQueries())
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    public override IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        foreach (var row in Rows)
        {
            foreach (var item in row.GetPhysicalTables())
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    public override IEnumerable<CommonTable> GetCommonTables()
    {
        foreach (var row in Rows)
        {
            foreach (var item in row.GetCommonTables())
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    public override SelectQuery GetOrNewSelectQuery()
    {
        return ToSelectQuery();
    }

    /// <inheritdoc/>
    public override IEnumerable<QueryParameter> GetInnerParameters()
    {
        foreach (var item in Rows)
        {
            foreach (var p in item.GetParameters())
            {
                yield return p;
            }
        }
    }

    /// <summary>
    /// Converts the <see cref="ValuesQuery"/> to a plain <see cref="SelectQuery"/>.
    /// </summary>
    /// <returns>The resulting <see cref="SelectQuery"/>.</returns>
    public SelectQuery ToPlainSelectQuery()
    {
        var lst = GetDefaultColumnAliases();
        return ToPlainSelectQuery(lst);
    }

    /// <summary>
    /// Converts the <see cref="ValuesQuery"/> to a plain <see cref="SelectQuery"/> with specified column aliases.
    /// </summary>
    /// <param name="columnAlias">The column aliases.</param>
    /// <returns>The resulting <see cref="SelectQuery"/>.</returns>
    public SelectQuery ToPlainSelectQuery(IEnumerable<string> columnAlias)
    {
        var columns = columnAlias.ToList();
        SelectQuery? sq = null;

        foreach (var row in Rows)
        {
            if (sq == null)
            {
                sq = row.ToPlainSelectQuery(columns);
            }
            else
            {
                sq.AddOperatableValue("union all", row.ToPlainSelectQuery(columns));
            }
        }
        if (sq == null) throw new InvalidOperationException();

        return sq;
    }

    /// <summary>
    /// Converts the <see cref="ValuesQuery"/> to a <see cref="SelectQuery"/>.
    /// </summary>
    /// <returns>The resulting <see cref="SelectQuery"/>.</returns>
    public SelectQuery ToSelectQuery()
    {
        var lst = GetDefaultColumnAliases();
        return ToSelectQuery(lst);
    }

    /// <summary>
    /// Converts the <see cref="ValuesQuery"/> to a <see cref="SelectQuery"/> with specified column aliases.
    /// </summary>
    /// <param name="columnAlias">The column aliases.</param>
    /// <returns>The resulting <see cref="SelectQuery"/>.</returns>
    public SelectQuery ToSelectQuery(IEnumerable<string> columnAlias)
    {
        var sq = new SelectQuery();
        var f = sq.From(ToSelectableTable(columnAlias));

        foreach (var item in columnAlias) sq.Select(f, item);

        sq.OrderClause = OrderClause;
        sq.LimitClause = LimitClause;

        sq.Parameters = Parameters;

        return sq;
    }

    /// <summary>
    /// Converts the <see cref="ValuesQuery"/> to a selectable table.
    /// </summary>
    /// <param name="columnAliases">The column aliases.</param>
    /// <returns>The resulting selectable table.</returns>
    public override SelectableTable ToSelectableTable(IEnumerable<string>? columnAliases)
    {
        var vt = new VirtualTable(this);
        if (columnAliases == null)
        {
            var lst = GetDefaultColumnAliases();
            return vt.ToSelectable("v", lst);
        }
        else
        {
            return vt.ToSelectable("v", columnAliases);
        }
    }

    /// <inheritdoc/>
    public override IEnumerable<string> GetColumnNames()
    {
        return Enumerable.Empty<string>();
    }

    private List<string> GetDefaultColumnAliases()
    {
        if (!Rows.Any() || Rows.First().Count == 0) throw new Exception();
        var cnt = Rows.First().Count;

        var lst = new List<string>();
        cnt.ForEach(x => lst.Add("c" + x));

        return lst;
    }
}
