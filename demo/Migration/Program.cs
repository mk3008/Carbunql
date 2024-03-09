using Carbunql.Building;
using System.Text;

Console.WriteLine("> Input DDL query obtained from DBMS\n> (End by entering a blank line)");
var actual = ReadMultiLine();

Console.WriteLine("> Enter the expected DDL query\n> (End by entering a blank line)");
var expect = ReadMultiLine();

var migration = MigrationQueryBuilder.Execute(expect!, actual!);
Console.WriteLine("> export migration query:");
Console.WriteLine(migration.ToText());


Console.WriteLine();
Console.WriteLine("> Press enter key to exit");
Console.ReadLine();

static string ReadMultiLine()
{
	StringBuilder sb = new StringBuilder();

	while (true)
	{
		var line = Console.ReadLine();
		if (string.IsNullOrEmpty(line))
		{
			break;
		}
		sb.AppendLine(line);
	}
	return sb.ToString();
}

