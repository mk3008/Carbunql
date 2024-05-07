using Carbunql.Analysis;
using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Building;

/// <summary>
/// Provides methods to execute pivot queries based on months.
/// </summary>
public class MonthPivotQueryBuilder
{
    /// <summary>
    /// Gets or sets the start date for the pivot query.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the date format used in the pivot query.
    /// </summary>
    public string DateFormat { get; set; } = "MM";

    /// <summary>
    /// Gets or sets the range of months for the pivot query.
    /// </summary>
    public int Range { get; set; } = 12;

    /// <summary>
    /// Gets or sets a value indicating whether the pivot query includes a summary.
    /// </summary>
    public bool HasSummary { get; set; } = true;

    /// <summary>
    /// Gets or sets the alias for the data source in the pivot query.
    /// </summary>
    public string DatasourceAlias { get; set; } = "datasource";

    /// <summary>
    /// Gets or sets the alias for the matrix in the pivot query.
    /// </summary>
    public string MatrixAlias { get; set; } = "matrix";

    /// <summary>
    /// Executes the pivot query.
    /// </summary>
    /// <param name="sql">The SQL query.</param>
    /// <param name="dateColumn">The column containing the date.</param>
    /// <param name="groupColumns">The list of columns to group by.</param>
    /// <param name="valueColumn">The value column.</param>
    /// <param name="columnHeaders">The optional list of column headers.</param>
    /// <returns>The result of the pivot query.</returns>
    public IReadQuery Execute(string sql, string dateColumn, IList<string> groupColumns, string valueColumn, List<string>? columnHeaders = null)
    {
        var (cteq, ds) = QueryParser.Parse(sql).ToCTE(DatasourceAlias);

        var matrix = cteq.With(CreateMatrixTable(dateColumn, columnHeaders));

        var sq = cteq.GetOrNewSelectQuery();
        var (f, d) = sq.From(ds).As("d");
        var m = f.InnerJoin(matrix).As("m").On(d, dateColumn);

        foreach (var item in groupColumns) sq.Select(d, item);

        var ca = matrix!.ColumnAliases;
        foreach (var item in ca!.Where(x => x.ToText() != dateColumn).Select(x => x.ToText()))
        {
            var c = new ColumnValue(d, valueColumn);
            c.Expression("*", new ColumnValue(m, item));
            sq.Select(new FunctionValue("sum", c)).As(item);
        }

        foreach (var item in groupColumns) sq.Group(d, item);

        foreach (var item in groupColumns) sq.Order(d, item);

        return cteq;
    }

    /// <summary>
    /// Creates the matrix table for the pivot query.
    /// </summary>
    /// <param name="ymColumn">The year-month column.</param>
    /// <param name="columnHeaders">The optional list of column headers.</param>
    /// <returns>The matrix table for the pivot query.</returns>
    private CommonTable CreateMatrixTable(string ymColumn, List<string>? columnHeaders)
    {
        var startdate = new DateTime(StartDate.Year, StartDate.Month, 1);
        var enddate = startdate.AddMonths(Range);

        var keyValues = new List<string>();

        var d = startdate;
        while (d < enddate)
        {
            keyValues.Add($"cast('{d}' as date)");
            d = d.AddMonths(1);
        }

        if (columnHeaders == null)
        {
            columnHeaders = new List<string>();
            d = startdate;
            while (d < enddate)
            {
                columnHeaders.Add("v" + d.ToString(DateFormat));
                d = d.AddMonths(1);
            }
        }

        var ct = MatrixEditor.Create(ymColumn, keyValues, columnHeaders);

        if (HasSummary) MatrixEditor.AddTotalSummaryColumn(ct);

        return ct;
    }
}
