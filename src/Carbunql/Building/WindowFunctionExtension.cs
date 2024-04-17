using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Building;

public static class WindowFunctionExtension
{
    public static void Partition(this OverClause source, ValueBase partition)
    {
        source.WindowDefinition ??= new();
        source.WindowDefinition.Partition(partition);
    }

    public static void Partition(this OverClause source, Func<ValueBase> partitionbuilder)
    {
        source.WindowDefinition ??= new();
        source.WindowDefinition.Partition(partitionbuilder());
    }

    public static void Partition(this OverClause source, SelectableItem order)
    {
        source.WindowDefinition ??= new();
        source.WindowDefinition.Partition(order.Value);
    }

    public static void Order(this OverClause source, ColumnValue order)
    {
        source.WindowDefinition ??= new();
        source.WindowDefinition.Order(order.ToSortable());
    }

    public static void Order(this OverClause source, SortableItem order)
    {
        source.WindowDefinition ??= new();
        source.WindowDefinition.Order(order);
    }

    public static void Order(this OverClause source, Func<SortableItem> orderbuilder)
    {
        source.WindowDefinition ??= new();
        source.WindowDefinition.Order(orderbuilder());
    }

    public static void Partition(this WindowDefinition source, ValueBase partition)
    {
        source.PartitionBy ??= new();
        source.PartitionBy.Add(partition);
    }

    public static void Partition(this WindowDefinition source, Func<ValueBase> partitionbuilder)
    {
        source.PartitionBy ??= new();
        source.PartitionBy.Add(partitionbuilder());
    }

    public static void Partition(this WindowDefinition source, SelectableItem order)
    {
        source.PartitionBy ??= new();
        source.PartitionBy.Add(order.Value);
    }

    public static void Order(this WindowDefinition source, ColumnValue order)
    {
        source.OrderBy ??= new();
        source.OrderBy.Add(order.ToSortable());
    }

    public static void Order(this WindowDefinition source, SortableItem order)
    {
        source.OrderBy ??= new();
        source.OrderBy.Add(order);
    }

    public static void Order(this WindowDefinition source, Func<SortableItem> orderbuilder)
    {
        source.OrderBy ??= new();
        source.OrderBy.Add(orderbuilder());
    }

    public static void Partition(this NamedWindowDefinition source, ValueBase partition)
    {
        source.WindowDefinition ??= new();
        source.WindowDefinition.Partition(partition);
    }

    public static void Partition(this NamedWindowDefinition source, Func<ValueBase> partitionbuilder)
    {
        source.WindowDefinition ??= new();
        source.WindowDefinition.Partition(partitionbuilder());
    }

    public static void Partition(this NamedWindowDefinition source, SelectableItem order)
    {
        source.WindowDefinition ??= new();
        source.WindowDefinition.Partition(order.Value);
    }

    public static void Order(this NamedWindowDefinition source, ColumnValue order)
    {
        source.WindowDefinition ??= new();
        source.WindowDefinition.Order(order.ToSortable());
    }

    public static void Order(this NamedWindowDefinition source, SelectableItem order)
    {
        source.WindowDefinition ??= new();
        source.WindowDefinition.Order(order.Value.ToSortable());
    }

    public static void Order(this NamedWindowDefinition source, SortableItem order)
    {
        source.WindowDefinition ??= new();
        source.WindowDefinition.Order(order);
    }

    public static void Order(this NamedWindowDefinition source, Func<SortableItem> orderbuilder)
    {
        source.WindowDefinition ??= new();
        source.WindowDefinition.Order(orderbuilder());
    }
}
