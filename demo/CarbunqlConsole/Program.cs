using Carbunql;
using Carbunql.Building;

var sq = new SelectQuery("select * from table_a as a");
var q = sq.ToDeleteQuery("destination", new[] { "id" });
Console.WriteLine(q.ToCommand().CommandText);

Console.WriteLine();
Console.WriteLine("press enter key");
Console.ReadLine();