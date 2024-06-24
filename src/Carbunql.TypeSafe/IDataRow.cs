using Carbunql.Annotations;

namespace Carbunql.TypeSafe;

/// <summary>
/// You can get information about the dataset you belong to.
/// </summary>
public interface IDataRow
{
    /// <summary>
    /// The dataset it belongs to (table, subquery, CTE)
    /// </summary>
    [IgnoreMapping]
    IDataSet DataSet { get; set; }
}
