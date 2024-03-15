using Carbunql.Analysis;
using Carbunql.Definitions;

namespace Carbunql.Building;

public class MigrationQueryBuilder
{
    public static DefinitionQuerySetList Execute(string expectsql, string actualsql)
    {
        var expects = DefinitionQuerySetParser.Parse(expectsql);
        var actuals = DefinitionQuerySetParser.Parse(actualsql);

        var lst = new DefinitionQuerySetList();

        foreach (var item in expects)
        {
            var actual = actuals.Where(x => x.GetTableFullName() == item.GetTableFullName()).FirstOrDefault();

            if (actual == null)
            {
                // If it does not currently exist, use the expected value as is.
                lst.Add(item);
                continue;
            }

            lst.Add(actual.GenerateMigrationQuery(item).MergeAlterTableQuery());
        }
        return lst;
        //= new List<DefinitionQuerySet>();
        //foreach (var item in DefinitionQuerySetParser.Parse(actualsql))
        //{
        //	lst.Add(item.GenerateMigrationQuery(expectsql).MergeAlterTableQuery());
        //}
    }
}
