using Carbunql;
using Carbunql.Building;

var tmp = new SelectQuery("select * from table_a as a");
var sq = tmp.ToCountQuery();

Console.WriteLine(sq.ToCommand().CommandText);

Console.WriteLine();
Console.WriteLine("press enter key");
Console.ReadLine();