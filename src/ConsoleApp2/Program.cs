using Carbunql;
using Carbunql.Building;

var sql = @"SELECT * FROM sales as s WHERE s.sales_date >= '2023-01-01'";

// 選択クエリをモデル化する
var query = new SelectQuery(sql);

// salesテーブルを参照しているか検索し、エイリアス名を取得。
// さらにエイリアス名を使用して検索条件を追加。
query.GetQuerySources()
.Where(x => x.GetTableFullName() == "sales")
.ForEach(x => x.Query.Where($"{x.Alias}.customer_id = 123"));

// 選択クエリに書き戻す
var modifiedSql = query.ToText();
Console.WriteLine(modifiedSql);

/*
SELECT
    *
FROM
    sales AS s
WHERE
    s.sales_date >= '2023-01-01'
    AND s.customer_id = 123
*/