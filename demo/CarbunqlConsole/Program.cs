using Carbunql;
using Carbunql.Analysis;
using Carbunql.Building;

var tmp = QueryParser.Parse("select * from table_a");
var sq = new SelectQuery();
var f = sq.From(tmp);

sq.Select(f);

Console.WriteLine();
Console.WriteLine("press enter key");
Console.ReadLine();