using Carbunql;
using Carbunql.Building;

var tmp = new SelectQuery("select * from table_a as a");

//Convert select query to Common table
//Return value is SelectQuery class and Common table class
var (sq, ct) = tmp.ToCTE("alias");

//Set common table to From clause
var t = sq.From(ct);

//Select all columns of the Common table
sq.Select(t);

Console.WriteLine(sq.ToCommand().CommandText);

Console.WriteLine();
Console.WriteLine("press enter key");
Console.ReadLine();