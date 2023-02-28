using Carbunql;
using Carbunql.Building;

var builder = new DiffQueryBuilder();
var sq = builder.Execute(
	"select id, val from table_a",
	"select id, val from table_b",
	new[] { "id" });

Console.WriteLine(sq.ToCommand().CommandText);

Console.WriteLine();
Console.WriteLine("press enter key");
Console.ReadLine();