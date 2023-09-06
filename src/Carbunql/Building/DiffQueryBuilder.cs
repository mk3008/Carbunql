using Carbunql.Analysis;
using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Building;

public class DiffQueryBuilder
{
	public static SelectQuery Execute(string expectsql, string actualsql, string[] keys)
	{
		return Execute(SelectQueryParser.Parse(expectsql), SelectQueryParser.Parse(actualsql), keys);
	}

	public static SelectQuery Execute(SelectQuery expectsql, SelectQuery actualsql, string[] keys)
	{
		var sq = GenerateQueryAsChanged(expectsql, actualsql, keys);
		sq.UnionAll(GenerateQueryAsDeleted(expectsql, actualsql, keys));
		sq.UnionAll(GenerateQueryAsAdded(expectsql, actualsql, keys));
		return sq;
	}

	private static SelectQuery GenerateQueryAsChanged(SelectQuery expectsq, SelectQuery actualsq, string[] keys)
	{
		var sq = new SelectQuery();
		var (from, e) = sq.From(expectsq).As("expect");

		var a = from.InnerJoin(actualsq).As("actual").On(e, keys);

		var expectColumns = expectsq.SelectClause!.Select(x => x.Alias).Where(x => !keys.Contains(x));
		var acutalColumns = actualsq.SelectClause!.Select(x => x.Alias).Where(x => !keys.Contains(x));
		var commonColumns = expectColumns.Where(x => acutalColumns.Contains(x));

		foreach (var item in keys) sq.Select(e, item);
		sq.Select("'update'").As("diff_type");
		CaseExpression? exp = null;

		ValueBase? tmp = null;
		foreach (var item in commonColumns)
		{
			var changecase = new CaseExpression();
			changecase.When($"{e.Alias}.{item} is null and {a.Alias}.{item} is null").Then($"''");
			changecase.When($"{e.Alias}.{item} is null and {a.Alias}.{item} is not null").Then($"'{item},'");
			changecase.When($"{e.Alias}.{item} is not null and {a.Alias}.{item} is null").Then($"'{item},'");
			changecase.When($"{e.Alias}.{item} <> {a.Alias}.{item}").Then($"'{item},'");
			changecase.Else("''");

			if (tmp == null)
			{
				exp = changecase;
				tmp = changecase;
			}
			else
			{
				tmp = tmp.AddOperatableValue("||", changecase);
			}
		}

		if (exp != null) sq.Select(exp).As("remarks");

		var q = new SelectQuery();
		var (_, t) = q.From(sq).As("t");
		q.Select(t);
		q.Where(t, "remarks").NotEqual("''");

		return q;
	}

	private static SelectQuery GenerateQueryAsDeleted(SelectQuery expectsq, SelectQuery actualsq, string[] keys)
	{
		var sq = new SelectQuery();
		var (from, e) = sq.From(expectsq).As("expect");

		var a = from.LeftJoin(actualsq).As("actual").On(e, keys);

		foreach (var item in keys) sq.Select(e, item);
		sq.Select("'delete'").As("diff_type");
		sq.Select("'*deleted'").As("remarks");
		sq.Where(a, keys[0]).IsNull();

		return sq;
	}

	private static SelectQuery GenerateQueryAsAdded(SelectQuery expectsq, SelectQuery actualsq, string[] keys)
	{
		var sq = new SelectQuery();
		var (from, a) = sq.From(actualsq).As("actual");

		var e = from.LeftJoin(expectsq).As("expect").On(a, keys);

		foreach (var item in keys) sq.Select(a, item);
		sq.Select("'insert'").As("diff_type");
		sq.Select("'*added'").As("remarks");
		sq.Where(e, keys[0]).IsNull();

		return sq;
	}
}