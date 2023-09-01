using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Building;

public static class WindowFunctionExtension
{
    public static void AddPartition(this Over source, ValueBase partition)
    {
        source.PartitionBy ??= new();
        source.PartitionBy.Add(partition);
    }

    public static void AddPartition(this Over source, Func<ValueBase> partitionbuilder)
    {
        source.PartitionBy ??= new();
        source.PartitionBy.Add(partitionbuilder());
    }

    public static void AddOrder(this Over source, SortableItem order)
    {
        source.OrderBy ??= new();
        source.OrderBy.Add(order);
    }

    public static void AddOrder(this Over source, Func<SortableItem> orderbuilder)
    {
        source.OrderBy ??= new();
        source.OrderBy.Add(orderbuilder());
    }
}
