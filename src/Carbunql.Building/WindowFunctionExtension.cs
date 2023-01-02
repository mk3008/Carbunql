using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Building;

public static class WindowFunctionExtension
{
    public static void AddPartition(this WindowFunction source, ValueBase partition)
    {
        source.PartitionBy ??= new();
        source.PartitionBy.Add(partition);
    }

    public static void AddPartition(this WindowFunction source, Func<ValueBase> partitionbuilder)
    {
        source.PartitionBy ??= new();
        source.PartitionBy.Add(partitionbuilder());
    }

    public static void AddOrder(this WindowFunction source, SortableItem order)
    {
        source.OrderBy ??= new();
        source.OrderBy.Add(order);
    }

    public static void AddOrder(this WindowFunction source, Func<SortableItem> orderbuilder)
    {
        source.OrderBy ??= new();
        source.OrderBy.Add(orderbuilder());
    }
}
