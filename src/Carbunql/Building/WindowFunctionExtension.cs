using Carbunql.Clauses;

namespace Carbunql.Building;

public static class WindowFunctionExtension
{
    public static void AddPartition(this OverClause source, ValueBase partition)
    {
        source.WindowDefinition ??= new();
		source.WindowDefinition.PartitionBy ??= new();
        source.WindowDefinition.PartitionBy.Add(partition);
    }

    public static void AddPartition(this OverClause source, Func<ValueBase> partitionbuilder)
    {
		source.WindowDefinition ??= new();
		source.WindowDefinition.PartitionBy ??= new();
        source.WindowDefinition.PartitionBy.Add(partitionbuilder());
    }

    public static void AddOrder(this OverClause source, SortableItem order)
    {
		source.WindowDefinition ??= new();
		source.WindowDefinition.OrderBy ??= new();
        source.WindowDefinition.OrderBy.Add(order);
    }

    public static void AddOrder(this OverClause source, Func<SortableItem> orderbuilder)
    {
		source.WindowDefinition ??= new();
		source.WindowDefinition.OrderBy ??= new();
        source.WindowDefinition.OrderBy.Add(orderbuilder());
    }
}
