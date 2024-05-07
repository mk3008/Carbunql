using Carbunql.Analysis;
using Carbunql.Definitions;

namespace Carbunql.Building;

/// <summary>
/// Provides methods to execute migration queries between expected and actual SQL schemas.
/// </summary>
public class MigrationQueryBuilder
{
    /// <summary>
    /// Executes migration queries between expected and actual SQL schemas.
    /// </summary>
    /// <param name="expectsql">The expected SQL schema.</param>
    /// <param name="actualsql">The actual SQL schema.</param>
    /// <returns>A list of definition query sets representing the migration queries.</returns>
    public static DefinitionQuerySetList Execute(string expectsql, string actualsql)
    {
        var expects = DefinitionQuerySetParser.Parse(expectsql).ToNormalize();
        var actuals = DefinitionQuerySetParser.Parse(actualsql).ToNormalize();

        var lst = new DefinitionQuerySetList();

        foreach (var item in actuals)
        {
            var exists = expects.Select(x => x.GetTableFullName()).Contains(item.GetTableFullName());
            if (!exists)
            {
                lst.Add(new DefinitionQuerySet(new DropTableQuery(item)));
                continue;
            }
        }

        foreach (var item in expects)
        {
            var actual = actuals.FirstOrDefault(x => x.GetTableFullName() == item.GetTableFullName());

            if (actual == null)
            {
                // If it does not currently exist, use the expected value as is.
                lst.Add(item);
                continue;
            }

            lst.Add(actual.GenerateMigrationQuery(item).MergeAlterTableQuery());
        }

        return lst;
    }
}
