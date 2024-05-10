using System.Text;

namespace Carbunql;

/// <summary>
/// Represents a list of definition query sets.
/// </summary>
public class DefinitionQuerySetList : List<DefinitionQuerySet>
{
    /// <summary>
    /// Converts the list of definition query sets to text.
    /// </summary>
    /// <param name="includeDropTableQuery">Flag to include drop table queries.</param>
    /// <returns>Text representation of the definition query sets.</returns>
    public string ToText(bool includeDropTableQuery = false)
    {
        var sb = new StringBuilder();
        foreach (var item in this)
        {
            sb.Append(item.ToText(includeDropTableQuery));
        }
        return sb.ToString();
    }

    /// <summary>
    /// Normalizes the list of definition query sets.
    /// </summary>
    /// <param name="doMergeAltarTablerQuery">Flag to indicate whether to merge alter table queries.</param>
    /// <returns>Normalized list of definition query sets.</returns>
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

    /// <summary>
    /// Merges alter table queries in the list of definition query sets.
    /// </summary>
    /// <returns>List of definition query sets with merged alter table queries.</returns>
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
