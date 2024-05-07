using Carbunql.Analysis;
using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Building;

/// <summary>
/// Provides functionality to execute pivot queries based on date columns.
/// </summary>
public class DatePivotQueryBuilder
{
    /// <summary>
    /// Gets or sets the month for which the pivot query is executed.
    /// </summary>
    public DateTime Month { get; set; }

    /// <summary>
    /// Gets or sets the date format used in the pivot query.
    /// </summary>
    public string DateFormat { get; set; } = "dd";

    /// <summary>
    /// Gets or sets a value indicating whether the pivot query includes summary columns.
    /// </summary>
    public bool HasSummary { get; set; } = true;

    /// <summary>
    /// Gets or sets the alias for the data source in the pivot query.
    /// </summary>
    public string DatasourceAlias { get; set; } = "datasource";

    /// <summary>
    /// Gets or sets the alias for the matrix table in the pivot query.
    /// </summary>
    public string MatrixAlias { get; set; } = "matrix";

    /// <summary>
    /// Executes the pivot query.
    /// </summary>
    /// <param name="sql">The SQL query.</param>
    /// <param name="dateColumn">The name of the date column in the SQL query.</param>
    /// <param name="groupColumns">The list of group columns in the SQL query.</param>
    /// <param name="valueColumn">The name of the value column in the SQL query.</param>
    /// <param name="columnHeaders">Optional. The list of column headers for the pivot table.</param>
    /// <returns>The executed pivot query.</returns>
    public IReadQuery Execute(string sql, string dateColumn, IList<string> groupColumns, string valueColumn, List<string>? columnHeaders = null)
    {
        var (cteq, ds) = QueryParser.Parse(sql).ToCTE(DatasourceAlias);

        var matrix = cteq.With(CreateMatrixTable(dateColumn, columnHeaders));

        var sq = cteq.GetOrNewSelectQuery();
        var (f, d) = sq.From(ds).As("d");
        var m = f.InnerJoin(matrix).As("m").On(d, dateColumn);

        foreach (var item in groupColumns)
        {
            sq.Select(d, item);
        }

        var ca = matrix!.ColumnAliases;
        foreach (var item in ca!.Where(x => x.ToText() != dateColumn).Select(x => x.ToText()))
        {
            var c = new ColumnValue(d, valueColumn);
            c.Expression("*", new ColumnValue(m, item));
            sq.Select(new FunctionValue("sum", c)).As(item);
        }

        foreach (var item in groupColumns)
        {
            sq.Group(d, item);
        }

        foreach (var item in groupColumns)
        {
            sq.Order(d, item);
        }

        return cteq;
    }

    /// <summary>
    /// Creates the matrix table for the pivot query.
    /// </summary>
    /// <param name="ymColumn">The name of the year-month column.</param>
    /// <param name="columnHeaders">Optional. The list of column headers for the matrix table.</param>
    /// <returns>The created matrix table.</returns>
    private CommonTable CreateMatrixTable(string ymColumn, List<string>? columnHeaders)
    {
        var startdate = new DateTime(Month.Year, Month.Month, 1);
        var enddate = startdate.AddMonths(1);

        var keyValues = new List<string>();

        var d = startdate;
        while (d < enddate)
        {
            keyValues.Add($"cast('{d}' as date)");
            d = d.AddDays(1);
        }

        if (columnHeaders == null)
        {
            columnHeaders = new List<string>();
            d = startdate;
            while (d < enddate)
            {
                columnHeaders.Add("v" + d.ToString(DateFormat));
                d = d.AddDays(1);
            }
        }

        var ct = MatrixEditor.Create(ymColumn, keyValues, columnHeaders);

        if (HasSummary)
        {
            MatrixEditor.AddTotalSummaryColumn(ct);
        }

        return ct;
    }
}
