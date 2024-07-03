namespace Carbunql;

public interface IDataSet
{
    /// <summary>
    /// Branch number.
    /// For union queries.
    /// </summary>
    int Branch { get; }

    /// <summary>
    /// Nested level.
    /// </summary>
    int Level { get; }

    /// <summary>
    /// Reference order.
    /// </summary>
    int Sequence { get; }

    string DataSetName { get; }

    IEnumerable<string> ColumnNames { get; }

    SelectQuery Query { get; }

    //IEnumerable<IDataSet> SubDataSets();
}

public class DataSet(int branch, int level, int sequence, string dataSetName, IEnumerable<string> columnNames, SelectQuery query) : IDataSet
{
    public int Branch { get; } = branch;

    public string DataSetName => dataSetName;

    public IEnumerable<string> ColumnNames => columnNames;

    public SelectQuery Query => query;

    public int Level { get; } = level;

    public int Sequence { get; } = sequence;
    //public IEnumerable<IDataSet> SubDataSets() => subDataSets;
}
