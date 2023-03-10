using Carbunql.Analysis;
using Carbunql.Clauses;
using Carbunql.Values;
using Carbunql.Extensions;
using System.Linq;

namespace Carbunql.Building;

public class DiffQueryBuilder
{
	public string LeftCteName { get; init; } = "lf_ds";
	public string RightCteName { get; init; } = "rt_ds";

	public string LeftValueColumnPrefix { get; init; } = "lf_";
	public string RightValueColumnPrefix { get; init; } = "rt_";

	public string DetailAlias { get; init; } = "detail";
	public string SummaryAlias { get; init; } = "summary";

	public IReadQuery Execute(string leftSql, string rightSql, IEnumerable<string> keyColumns)
	{
		var leftSq = QueryParser.Parse(leftSql);
		var rightSq = QueryParser.Parse(rightSql);

		var commons = GetCommonColumns(leftSq, rightSq);

		var keycols = keyColumns.ToList().Select(x => x.Trim());
		var keys = commons.Where(x => x.IsEqualNoCase(keycols)).ToList();
		var vals = commons.Where(x => !x.IsEqualNoCase(keycols)).ToList();

		if (!keys.Any()) throw new ArgumentException("key columns are not found.");
		if (!vals.Any()) throw new ArgumentException("value columns are not found.");

		var sq = new SelectQuery();
		var leftTable = sq.With(leftSq).As(LeftCteName);
		var rightTable = sq.With(rightSq).As(RightCteName);

		var detail = sq.With(BuildSelectDetailQuery(leftTable, rightTable, keys, vals)).As(DetailAlias);
		var summary = sq.With(BuildSelectSummaryQuery(detail, keys, vals)).As(SummaryAlias);

		var (f, d) = sq.From(summary).As("d");

		keys.ForEach(x => sq.Select(d, x));
		vals.Select(x => LeftValueColumnPrefix + x).ToList().ForEach(x =>
		{
			sq.Select(d, x);
		});
		vals.Select(x => RightValueColumnPrefix + x).ToList().ForEach(x =>
		{
			sq.Select(d, x);
		});

		sq.Where(vals.Select(x => new ColumnValue(d, LeftValueColumnPrefix + x).NotEqual(d, RightValueColumnPrefix + x).ToGroup()).MergeOr().ToGroup());

		keys.ForEach(x => sq.Order(d, x));

		return sq;
	}

	private List<string> GetCommonColumns(IReadQuery sq1, IReadQuery sq2)
	{
		var cols1 = sq1.GetColumnNames();
		var cols2 = sq2.GetColumnNames();

		if (cols1 == null || cols2 == null) throw new NotSupportedException();

		return cols1.Where(x => cols2.Contains(x)).ToList();
	}

	private SelectQuery BuildSelectSummaryQuery(CommonTable detail, List<string> keys, List<string> vals)
	{
		var sq = new SelectQuery();
		var (f, d) = sq.From(detail).As("d");
		keys.ForEach(x => sq.Select(d, x));

		vals.Select(x => LeftValueColumnPrefix + x).ToList().ForEach(x =>
		{
			sq.Select(new FunctionValue("sum", new ColumnValue(d, x))).As(x);
		});
		vals.Select(x => RightValueColumnPrefix + x).ToList().ForEach(x =>
		{
			sq.Select(new FunctionValue("sum", new ColumnValue(d, x))).As(x);
		});

		keys.ForEach(x => sq.Group(d, x));
		return sq;
	}

	private SelectQuery BuildSelectDetailQuery(CommonTable leftTable, CommonTable rightTable, List<string> keys, List<string> vals)
	{
		var sq = BuildSelectDetailQuery(leftTable, keys, vals, true);
		sq.UnionAll(BuildSelectDetailQuery(rightTable, keys, vals, false));
		return sq;
	}

	private SelectQuery BuildSelectDetailQuery(CommonTable table, List<string> keys, List<string> vals, bool isLeft)
	{
		var sq = new SelectQuery();
		var (f, d) = sq.From(table).As("d");
		keys.ForEach(x => sq.Select(d, x));

		if (isLeft)
		{
			vals.ForEach(x =>
			{
				sq.Select(d, x).As(LeftValueColumnPrefix + x);
			});
			vals.ForEach(x =>
			{
				sq.Select(0).As(RightValueColumnPrefix + x);
			});
		}
		else
		{
			vals.ForEach(x =>
			{
				sq.Select(0).As(LeftValueColumnPrefix + x);
			});
			vals.ForEach(x =>
			{
				sq.Select(d, x).As(RightValueColumnPrefix + x);
			});
		}
		return sq;
	}
}