using System.Text;

namespace Carbunql;

public class DefinitionQuerySetList : List<DefinitionQuerySet>
{
    public string ToText(bool includeDropTableQuery = false)
    {
        var sb = new StringBuilder();
        foreach (var item in this)
        {
            sb.Append(item.ToText(includeDropTableQuery));
        }
        return sb.ToString();
    }

    public DefinitionQuerySetList ToNormalize(bool doMergeAltarTablerQuery = true)
    {
        var lst = new DefinitionQuerySetList();
        foreach (var item in this)
        {
            if (item.DropTableQuery == null)
            {
                lst.Add(item.ToNormalize(doMergeAltarTablerQuery));
            }
        }
        return lst;
    }

    public DefinitionQuerySetList MergeAlterTableQuery()
    {
        var lst = new DefinitionQuerySetList();
        foreach (var item in this)
        {
            lst.Add(item.MergeAlterTableQuery());
        }
        return lst;
    }
}
