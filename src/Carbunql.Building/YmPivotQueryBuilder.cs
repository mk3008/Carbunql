using Carbunql.Analysis;
using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Building;

public class YmPivotQueryBuilder
{
	public DateTime StartDate { get; set; }

	public string DateFormat { get; set; } = "MM";

	public int Range { get; set; } = 12;

	public bool HasSummary { get; set; } = true;

	public string DatasourceAlias { get; set; } = "datasource";

	public string MatrixAlias { get; set; } = "matrix";

	public IReadQuery Execute(string sql, string ymColumn, IList<string> groupColumns, string valueColumn, List<string>? columnHeaders = null)
	{
		var (cteq, ds) = QueryParser.Parse(sql).ToCTE(DatasourceAlias);

		var matrix = cteq.With(CreateMatrixTable(ymColumn, columnHeaders));

		var sq = cteq.GetOrNewSelectQuery();
		var (f, d) = sq.From(ds).As("d");
		var m = f.InnerJoin(matrix).As("m").On(d, ymColumn);

		foreach (var item in groupColumns) sq.Select(d, item);

		var ca = matrix!.ColumnAliases;
		foreach (var item in ca!.Where(x => x.ToText() != ymColumn).Select(x => x.ToText()))
		{
			var c = new ColumnValue(d, valueColumn);
			c.Expression("*", new ColumnValue(m, item));
			sq.Select(new FunctionValue("sum", c)).As(item);
		}

		foreach (var item in groupColumns) sq.Group(d, item);

		foreach (var item in groupColumns) sq.Order(d, item);

		return cteq;
	}

	private CommonTable CreateMatrixTable(string ymColumn, List<string>? columnHeaders)
	{
		var startym = new DateTime(StartDate.Year, StartDate.Month, 1);
		var endym = startym.AddMonths(Range);

		var keyValues = new List<string>();

		var ym = startym;
		while (ym < endym)
		{
			keyValues.Add($"cast('{ym}' as date)");
			ym = ym.AddMonths(1);
		}

		if (columnHeaders == null)
		{
			columnHeaders = new List<string>();
			ym = startym;
			while (ym < endym)
			{
				columnHeaders.Add("v" + ym.ToString(DateFormat));
				ym = ym.AddMonths(1);
			}
		}

		var ct = MatrixEditor.Create(ymColumn, keyValues, columnHeaders);

		if (HasSummary) MatrixEditor.AddTotalSummaryColumn(ct);

		return ct;
	}
}