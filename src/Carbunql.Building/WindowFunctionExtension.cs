using Carbunql.Clauses;
using Carbunql.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carbunql.Building;

public static class WindowFunctionExtension
{
    public static void AddPartition(this WindowFunction source, ValueBase partition)
    {
        source.PartitionBy ??= new();
        source.PartitionBy.Add(partition);
    }

    public static void AddOrder(this WindowFunction source, ValueBase order)
    {
        source.OrderBy ??= new();
        source.OrderBy.Add(order);
    }
}
