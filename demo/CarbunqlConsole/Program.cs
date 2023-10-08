using Carbunql;
using Carbunql.Building;
using Carbunql.Postgres;

var sq = new SelectQuery();

var (f, ss) = sq.FromAs<shop_sales>("ss");
sq.SelectAll();
var x = f.InnerJoinAs<shops>(s => ss.shop_id == s.shop_id);
var a = f.InnerJoinAs<shop_owners>(o => x.owner_id == o.owner_id);


Console.WriteLine(sq.ToCommand().CommandText);

Console.WriteLine();
Console.WriteLine("press enter key");
Console.ReadLine();

public record struct shops(int shop_id, int owner_id, string shop_name);
public record struct shop_sales(int shop_sales_id, int shop_id, int article_id, DateTime sale_date, int price);
public record struct shop_owners(int owner_id, string owner_name);
