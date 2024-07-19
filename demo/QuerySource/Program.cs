﻿using Carbunql;
using Carbunql.Analysis.Parser;
using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Extensions;

var sql = @"select s.sale_id, s.store_id, s.sale_date as journal_date, s.sale_price from sales as s";

//column info
var keyColumn = "sale_id";
var dateColumn = "journal_date";
var valueColumn = "sale_price";
var ignoreColumn = "journal_date";

var keyTable = "sale_keys";
var journalTable = "sale_journals";

var lowerLimit = new DateTime(2024, 1, 1);

Console.WriteLine("--origin----------");
var query = new SelectQuery(sql);
Console.WriteLine(query.ToText());
Console.WriteLine(";");

try
{
    Console.WriteLine("--snapshot----------");
    var crateTablequery = new SelectQuery(sql)
        .ToCTEQuery("datasource", "ds")
        .OverrideSelect(dateColumn, (source, value) =>
        {
            source.Query.AddComment($"Inject a lower limit for {dateColumn}");
            return $"greatest({value}, {source.Query.AddParameter(":lower_limit", lowerLimit)})";
        })
        .InjectNotJournaledFilter(keyTable, keyColumn)
        .AddSelect("nextval('sale_journal_id_seq')", "sale_journal_id")
        .ToCTEQuery("final", "f")
        .ToCreateTableQuery("tmp", isTemporary: true);
    Console.WriteLine(crateTablequery.ToText());
    Console.WriteLine(";");

    Console.WriteLine("--validate----------");
    var insertQuery = new SelectQuery(sql)
        .ToDiffQuery(journalTable, keyColumn, valueColumn, ignoreColumn)
        .OverrideSelect(dateColumn, (source, value) =>
        {
            source.Query.AddComment($"Inject a lower limit for {dateColumn}");
            return $"greatest({value}, {source.Query.AddParameter(":lower_limit", lowerLimit)})";
        })
        .AddWhere(keyColumn, source =>
        {
            source.Query.AddComment($"Inject filter. {keyColumn}");
            return $"{source.Alias}.{keyColumn} = {source.Query.AddParameter(":sale_id", 1)}";
        })
        .ToCTEQuery("final", "f")
        .ToInsertQuery("sale_journals");
    Console.WriteLine(insertQuery.ToText());
    Console.WriteLine(";");
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}


public static class SelectQueryExtension
{
    public static SelectQuery InjectNotJournaledFilter(this SelectQuery query, string keyTable, string keyColumn)
    {
        query.GetQuerySources()
            .Where(x => x.ColumnNames.Contains(keyColumn))
            .GetRootsBySource()
            .EnsureAny()
            .ForEach(qs =>
            {
                //filter 
                qs.Query.AddComment($"Inject a not archived filter. {keyTable}");
                qs.Query.Where(() =>
                {
                    var sq = new SelectQuery($"select * from {keyTable} as x");
                    sq.Where($"x.{keyColumn} = {qs.Alias}.{keyColumn}");
                    return sq.ToNotExists();
                });
            });

        return query;
    }

    public static SelectQuery ToDiffQuery(this SelectQuery actualQuery, string journalTable, string keyColumn, string valueColumn, string ignoreColumn)
    {
        var expectQuery = new SelectQuery($"select arch.sale_id, arch.store_id, arch.journal_date, arch.sale_price from {journalTable} as arch");

        var checkColumns = expectQuery.GetColumnNames().Where(x => x != keyColumn && x != ignoreColumn).ToList();
        var notValueColumns = expectQuery.GetColumnNames().Where(x => x != valueColumn).ToList();

        var expectCTE = "expect";
        var expectAlias = "exp";
        var actualCTE = "actual";
        var actualAlias = "act";

        SelectQuery selectCancellationQuery()
        {
            var (q, expect) = expectQuery.ToCTE(expectCTE);
            var actual = expectQuery.With(actualQuery).As(actualCTE);

            var (f, e) = q.From(expect).As(expectAlias);
            var a = f.LeftJoin(actual).As(actualAlias).On(e, keyColumn);
            q.Where(a, keyColumn).IsNull();

            notValueColumns.ForEach(column => q.Select(e, column));
            q.Select($"{e.Alias}.{valueColumn} * -1").As(valueColumn);
            q.Select("'cancellation'").As("remarks");
            return q;
        }

        SelectQuery selectCollectionQuery()
        {
            var (q, expect) = expectQuery.ToCTE(expectCTE);
            var actual = expectQuery.With(actualQuery).As(actualCTE);

            var (f, e) = q.From(expect).As(expectAlias);
            var a = f.InnerJoin(actual).As(actualAlias).On(e, keyColumn);
            q.Where(() =>
            {
                var condition = new List<string>();
                checkColumns.ForEach(column => condition.Add($"{e.Alias}.{column} <> {a.Alias}.{column}"));
                return ValueParser.Parse($"({string.Join(" or ", condition)})");
            });

            notValueColumns.ForEach(column => q.Select(e, column));
            q.Select($"{e.Alias}.{valueColumn} * -1").As(valueColumn);
            q.Select("'correction'").As("remarks");
            return q;
        }

        SelectQuery selectRevisedQuery()
        {
            var (q, expect) = expectQuery.ToCTE(expectCTE);
            var actual = expectQuery.With(actualQuery).As(actualCTE);

            var (f, e) = q.From(expect).As(expectAlias);
            var a = f.InnerJoin(actual).As(actualAlias).On(e, keyColumn);
            q.Where(() =>
            {
                var condition = new List<string>();
                checkColumns.ForEach(column => condition.Add($"{e.Alias}.{column} <> {a.Alias}.{column}"));
                return ValueParser.Parse($"({string.Join(" or ", condition)})");
            });

            notValueColumns.ForEach(column => q.Select(a.Alias, column));
            q.Select($"{a.Alias}.{valueColumn}").As(valueColumn);
            q.Select("'revised'").As("remarks");
            return q;
        }

        //diff
        var diffquer = selectCancellationQuery();
        diffquer.AddOperatableValue("union all", selectCollectionQuery());
        diffquer.AddOperatableValue("union all", selectRevisedQuery());

        return diffquer;
    }
}
