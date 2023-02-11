using Carbunql.Analysis;
using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Building;

public class YmPivotQueryBuilder
{
	public string YmColumn { get; set; } = "ym";

	public DateTime StartDate { get; set; }

	public int Range { get; set; } = 12;

	public bool HasSummary { get; set; } = true;

	public string DatasourceAlias { get; set; } = "datasource";

	public string MatrixAlias { get; set; } = "matrix";

	public IReadQuery Execute(string sql, string ymColumn, IList<string> headerColumns, string valueColumn, List<string>? valuesHeader = null)
	{
		if (valuesHeader == null)
		{
			valuesHeader = new List<string>();
			for (int i = 0; i < Range; i++) valuesHeader.Add("ym_" + i);
			if (HasSummary) valuesHeader.Add("ym_total");
		}

		var (cteq, ds) = QueryParser.Parse(sql).ToCTE(DatasourceAlias);

		var matrix = cteq.With(BuildMatrixTable(valuesHeader));

		var sq = cteq.GetOrNewSelectQuery();
		var (f, d) = sq.From(ds).As("d");
		var m = f.InnerJoin(matrix).As("m").On(d, YmColumn);

		foreach (var item in headerColumns)
		{
			sq.Select(d, item);
		}

		for (int i = 0; i < Range; i++)
		{
			var alias = valuesHeader[i];
			var c = new ColumnValue(d, valueColumn);
			c.Expression("*", new ColumnValue(m, alias));
			sq.Select(new FunctionValue("sum", c)).As(alias);
		}

		if (HasSummary)
		{
			var alias = valuesHeader.Last();
			var c = new ColumnValue(d, valueColumn);
			c.Expression("*", new ColumnValue(m, alias));
			sq.Select(new FunctionValue("sum", c)).As(alias);
		}

		foreach (var item in headerColumns)
		{
			sq.Group(d, item);
		}

		return cteq;
	}

	private CommonTable BuildMatrixTable(List<string> valuesHeader)
	{
		var startYm = new DateTime(StartDate.Year, StartDate.Month, 1);
		var shift = 0;
		var endym = startYm.AddMonths(Range);

		var rows = new List<ValueCollection>();
		var dic = new Dictionary<string, object?>();

		while (startYm < endym)
		{
			var row = new ValueCollection();

			var key = ":ym" + shift;
			var val = startYm;
			dic.Add(key, val);

			row.Add(new LiteralValue(key));

			for (int i = 0; i < Range; i++)
			{
				if (shift == i)
				{
					row.Add(new LiteralValue("1"));
				}
				else
				{
					row.Add(new LiteralValue("0"));
				}
			}

			if (HasSummary)
			{
				row.Add(new LiteralValue("1"));
			}

			rows.Add(row);
			shift++;
			startYm = startYm.AddMonths(1);
		}

		var vq = new ValuesQuery(rows);
		foreach (var item in dic) vq.Parameters.Add(item);

		return vq.ToCommonTable(MatrixAlias, valuesHeader);
	}
}