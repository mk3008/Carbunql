using Carbunql.Annotations;

namespace Carbunql.TypeSafe;

public interface IDataRow
{
    [IgnoreMapping]
    IDataSet DataSet { get; set; }
}
