using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Building;

public static class WindowFunctionExtension
{
	public static void AddPartition(this OverClause source, ValueBase partition)
	{
		source.WindowDefinition ??= new();
		source.WindowDefinition.AddPartition(partition);
	}

	public static void AddPartition(this OverClause source, Func<ValueBase> partitionbuilder)
	{
		source.WindowDefinition ??= new();
		source.WindowDefinition.AddPartition(partitionbuilder());
	}

	public static void AddPartition(this OverClause source, SelectableItem order)
	{
		source.WindowDefinition ??= new();
		source.WindowDefinition.AddPartition(order.Value);
	}

	public static void AddOrder(this OverClause source, ColumnValue order)
	{
		source.WindowDefinition ??= new();
		source.WindowDefinition.AddOrder(order.ToSortable());
	}

	public static void AddOrder(this OverClause source, SortableItem order)
	{
		source.WindowDefinition ??= new();
		source.WindowDefinition.AddOrder(order);
	}

	public static void AddOrder(this OverClause source, Func<SortableItem> orderbuilder)
	{
		source.WindowDefinition ??= new();
		source.WindowDefinition.AddOrder(orderbuilder());
	}

	public static void AddPartition(this WindowDefinition source, ValueBase partition)
	{
		source.PartitionBy ??= new();
		source.PartitionBy.Add(partition);
	}

	public static void AddPartition(this WindowDefinition source, Func<ValueBase> partitionbuilder)
	{
		source.PartitionBy ??= new();
		source.PartitionBy.Add(partitionbuilder());
	}

	public static void AddPartition(this WindowDefinition source, SelectableItem order)
	{
		source.PartitionBy ??= new();
		source.PartitionBy.Add(order.Value);
	}

	public static void AddOrder(this WindowDefinition source, ColumnValue order)
	{
		source.OrderBy ??= new();
		source.OrderBy.Add(order.ToSortable());
	}

	public static void AddOrder(this WindowDefinition source, SortableItem order)
	{
		source.OrderBy ??= new();
		source.OrderBy.Add(order);
	}

	public static void AddOrder(this WindowDefinition source, Func<SortableItem> orderbuilder)
	{
		source.OrderBy ??= new();
		source.OrderBy.Add(orderbuilder());
	}

	public static void AddPartition(this NamedWindowDefinition source, ValueBase partition)
	{
		source.WindowDefinition ??= new();
		source.WindowDefinition.AddPartition(partition);
	}

	public static void AddPartition(this NamedWindowDefinition source, Func<ValueBase> partitionbuilder)
	{
		source.WindowDefinition ??= new();
		source.WindowDefinition.AddPartition(partitionbuilder());
	}

	public static void AddPartition(this NamedWindowDefinition source, SelectableItem order)
	{
		source.WindowDefinition ??= new();
		source.WindowDefinition.AddPartition(order.Value);
	}

	public static void AddOrder(this NamedWindowDefinition source, ColumnValue order)
	{
		source.WindowDefinition ??= new();
		source.WindowDefinition.AddOrder(order.ToSortable());
	}

	public static void AddOrder(this NamedWindowDefinition source, SelectableItem order)
	{
		source.WindowDefinition ??= new();
		source.WindowDefinition.AddOrder(order.Value.ToSortable());
	}

	public static void AddOrder(this NamedWindowDefinition source, SortableItem order)
	{
		source.WindowDefinition ??= new();
		source.WindowDefinition.AddOrder(order);
	}

	public static void AddOrder(this NamedWindowDefinition source, Func<SortableItem> orderbuilder)
	{
		source.WindowDefinition ??= new();
		source.WindowDefinition.AddOrder(orderbuilder());
	}
}
