using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Building;

/// <summary>
/// Provides extension methods for defining window functions.
/// </summary>
public static class WindowFunctionExtension
{
    /// <summary>
    /// Adds a partition clause to the OVER clause.
    /// </summary>
    /// <param name="source">The OVER clause to which the partition clause will be added.</param>
    /// <param name="partition">The value to partition by.</param>
    public static void Partition(this OverClause source, ValueBase partition)
    {
        source.WindowDefinition ??= new();
        source.WindowDefinition.Partition(partition);
    }

    /// <summary>
    /// Adds a partition clause to the OVER clause using a builder function.
    /// </summary>
    /// <param name="source">The OVER clause to which the partition clause will be added.</param>
    /// <param name="partitionbuilder">A function that builds the partition value.</param>
    public static void Partition(this OverClause source, Func<ValueBase> partitionbuilder)
    {
        source.WindowDefinition ??= new();
        source.WindowDefinition.Partition(partitionbuilder());
    }

    /// <summary>
    /// Adds a partition clause to the OVER clause based on a selectable item.
    /// </summary>
    /// <param name="source">The OVER clause to which the partition clause will be added.</param>
    /// <param name="order">The selectable item to partition by.</param>
    public static void Partition(this OverClause source, SelectableItem order)
    {
        source.WindowDefinition ??= new();
        source.WindowDefinition.Partition(order.Value);
    }

    /// <summary>
    /// Adds an order clause to the OVER clause using a column value.
    /// </summary>
    /// <param name="source">The OVER clause to which the order clause will be added.</param>
    /// <param name="order">The column value to order by.</param>
    public static void Order(this OverClause source, ColumnValue order)
    {
        source.WindowDefinition ??= new();
        source.WindowDefinition.Order(order.ToSortable());
    }

    /// <summary>
    /// Adds an order clause to the OVER clause using a sortable item.
    /// </summary>
    /// <param name="source">The OVER clause to which the order clause will be added.</param>
    /// <param name="order">The sortable item to order by.</param>
    public static void Order(this OverClause source, SortableItem order)
    {
        source.WindowDefinition ??= new();
        source.WindowDefinition.Order(order);
    }

    /// <summary>
    /// Adds an order clause to the OVER clause using a builder function.
    /// </summary>
    /// <param name="source">The OVER clause to which the order clause will be added.</param>
    /// <param name="orderbuilder">A function that builds the sortable item for ordering.</param>
    public static void Order(this OverClause source, Func<SortableItem> orderbuilder)
    {
        source.WindowDefinition ??= new();
        source.WindowDefinition.Order(orderbuilder());
    }

    /// <summary>
    /// Adds a partition clause to the window definition.
    /// </summary>
    /// <param name="source">The window definition to which the partition clause will be added.</param>
    /// <param name="partition">The value to partition by.</param>
    public static void Partition(this WindowDefinition source, ValueBase partition)
    {
        source.PartitionBy ??= new();
        source.PartitionBy.Add(partition);
    }

    /// <summary>
    /// Adds a partition clause to the window definition using a builder function.
    /// </summary>
    /// <param name="source">The window definition to which the partition clause will be added.</param>
    /// <param name="partitionbuilder">A function that builds the partition value.</param>
    public static void Partition(this WindowDefinition source, Func<ValueBase> partitionbuilder)
    {
        source.PartitionBy ??= new();
        source.PartitionBy.Add(partitionbuilder());
    }

    /// <summary>
    /// Adds a partition clause to the window definition based on a selectable item.
    /// </summary>
    /// <param name="source">The window definition to which the partition clause will be added.</param>
    /// <param name="order">The selectable item to partition by.</param>
    public static void Partition(this WindowDefinition source, SelectableItem order)
    {
        source.PartitionBy ??= new();
        source.PartitionBy.Add(order.Value);
    }


    /// <summary>
    /// Adds an order clause to the window definition using a column value.
    /// </summary>
    /// <param name="source">The window definition to which the order clause will be added.</param>
    /// <param name="order">The column value to order by.</param>
    public static void Order(this WindowDefinition source, ColumnValue order)
    {
        source.OrderBy ??= new();
        source.OrderBy.Add(order.ToSortable());
    }

    /// <summary>
    /// Adds an order clause to the window definition using a sortable item.
    /// </summary>
    /// <param name="source">The window definition to which the order clause will be added.</param>
    /// <param name="order">The sortable item to order by.</param>
    public static void Order(this WindowDefinition source, SortableItem order)
    {
        source.OrderBy ??= new();
        source.OrderBy.Add(order);
    }

    /// <summary>
    /// Adds an order clause to the window definition using a builder function.
    /// </summary>
    /// <param name="source">The window definition to which the order clause will be added.</param>
    /// <param name="orderbuilder">A function that builds the sortable item for ordering.</param>
    public static void Order(this WindowDefinition source, Func<SortableItem> orderbuilder)
    {
        source.OrderBy ??= new();
        source.OrderBy.Add(orderbuilder());
    }

    /// <summary>
    /// Adds a partition clause to the named window definition.
    /// </summary>
    /// <param name="source">The named window definition to which the partition clause will be added.</param>
    /// <param name="partition">The value to partition by.</param>
    public static void Partition(this NamedWindowDefinition source, ValueBase partition)
    {
        source.WindowDefinition ??= new();
        source.WindowDefinition.Partition(partition);
    }

    /// <summary>
    /// Adds a partition clause to the named window definition using a builder function.
    /// </summary>
    /// <param name="source">The named window definition to which the partition clause will be added.</param>
    /// <param name="partitionbuilder">A function that builds the partition value.</param>
    public static void Partition(this NamedWindowDefinition source, Func<ValueBase> partitionbuilder)
    {
        source.WindowDefinition ??= new();
        source.WindowDefinition.Partition(partitionbuilder());
    }

    /// <summary>
    /// Adds a partition clause to the named window definition based on a selectable item.
    /// </summary>
    /// <param name="source">The named window definition to which the partition clause will be added.</param>
    /// <param name="order">The selectable item to partition by.</param>
    public static void Partition(this NamedWindowDefinition source, SelectableItem order)
    {
        source.WindowDefinition ??= new();
        source.WindowDefinition.Partition(order.Value);
    }

    /// <summary>
    /// Adds an order clause to the named window definition using a column value.
    /// </summary>
    /// <param name="source">The named window definition to which the order clause will be added.</param>
    /// <param name="order">The column value to order by.</param>
    public static void Order(this NamedWindowDefinition source, ColumnValue order)
    {
        source.WindowDefinition ??= new();
        source.WindowDefinition.Order(order.ToSortable());
    }

    /// <summary>
    /// Adds an order clause to the named window definition based on a selectable item.
    /// </summary>
    /// <param name="source">The named window definition to which the order clause will be added.</param>
    /// <param name="order">The selectable item to order by.</param>
    public static void Order(this NamedWindowDefinition source, SelectableItem order)
    {
        source.WindowDefinition ??= new();
        source.WindowDefinition.Order(order.Value.ToSortable());
    }

    /// <summary>
    /// Adds an order clause to the named window definition using a sortable item.
    /// </summary>
    /// <param name="source">The named window definition to which the order clause will be added.</param>
    /// <param name="order">The sortable item to order by.</param>
    public static void Order(this NamedWindowDefinition source, SortableItem order)
    {
        source.WindowDefinition ??= new();
        source.WindowDefinition.Order(order);
    }

    /// <summary>
    /// Adds an order clause to the named window definition using a builder function.
    /// </summary>
    /// <param name="source">The named window definition to which the order clause will be added.</param>
    /// <param name="orderbuilder">A function that builds the sortable item for ordering.</param>
    public static void Order(this NamedWindowDefinition source, Func<SortableItem> orderbuilder)
    {
        source.WindowDefinition ??= new();
        source.WindowDefinition.Order(orderbuilder());
    }
}
