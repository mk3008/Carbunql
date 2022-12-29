using Carbunql.Clauses;
using Carbunql.Values;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carbunql.Building;

public static class QuerytExtension
{
    //public static SelectableItem ToSelectable(this ColumnValue source)
    //{
    //    return new SelectableItem(source, source.GetDefaultName());
    //}

    //public static void Select(this SelectQuery source, SelectableTable table, string column)
    //{
    //    var item = new ColumnValue(table.Alias, column).ToSelectable();
    //    source.SelectClause ??= new();
    //    source.SelectClause.Add(item);
    //}
}
