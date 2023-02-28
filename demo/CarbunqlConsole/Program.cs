using Carbunql;
using Carbunql.Building;

var builder = new DatePivotQueryBuilder() { Month = new DateTime(2020, 1, 1) };

var sq = builder.Execute(
	"select ymd, shop_id, sales_amount from sales",
	"ymd",
	new[] { "shop_id" },
	"sales_amount");

Console.WriteLine(sq.ToCommand().CommandText);

Console.WriteLine();
Console.WriteLine("press enter key");
Console.ReadLine();