using Carbunql.Analysis;

namespace Carbunql.Building;

public class MigrationQueryBuilder
{
	public static DefinitionQuerySet Execute(string expectsql, string actualsql)
	{
		var actual = DefinitionQuerySetParser.Parse(actualsql);
		return actual.GenerateMigrationQuery(expectsql).MergeAlterTableQuery();
	}
}
