using Carbunql;
using Carbunql.Clauses;
using Carbunql.Tables;
using System.Text;

Console.WriteLine("> Input Select Query\n> (End by entering a blank line. If nothing is entered, display a sample.)");
var sql = ReadMultiLine();
if (string.IsNullOrEmpty(sql))
{
	sql = @"
with
dat(line_id, name, unit_price, quantity, tax_rate) as ( 
    values
    (1, 'apple' , 105, 5, 0.07),
    (2, 'orange', 203, 3, 0.07),
    (3, 'banana', 233, 9, 0.07),
    (4, 'tea'   , 309, 7, 0.08),
    (5, 'coffee', 555, 9, 0.08),
    (6, 'cola'  , 456, 2, 0.08)
),
detail as (
    select  
        q.*,
        trunc(q.price * (1 + q.tax_rate)) - q.price as tax,
        q.price * (1 + q.tax_rate) - q.price as raw_tax
    from
        (
            select
                dat.*,
                (dat.unit_price * dat.quantity) as price
            from
                dat
        ) q
), 
tax_summary as (
    select
        d.tax_rate,
        trunc(sum(raw_tax)) as total_tax
    from
        detail d
    group by
        d.tax_rate
)
select 
   line_id,
    name,
    unit_price,
    quantity,
    tax_rate,
    price,
    price + tax as tax_included_price,
    tax
from
    (
        select
            line_id,
            name,
            unit_price,
            quantity,
            tax_rate,
            price,
            tax + adjust_tax as tax
        from
            (
                select
                    q.*,
                    case when q.total_tax - q.cumulative >= q.priority then 1 else 0 end as adjust_tax
                from
                    (
                        select  
                            d.*, 
                            s.total_tax,
                            sum(d.tax) over (partition by d.tax_rate) as cumulative,
                            row_number() over (partition by d.tax_rate order by d.raw_tax % 1 desc, d.line_id) as priority
                        from
                            detail d
                            inner join tax_summary s on d.tax_rate = s.tax_rate
                    ) q
            ) q
    ) q
order by 
    line_id
";
}

var sq = new SelectQuery(sql);
Console.WriteLine("\n> Format:");
Console.WriteLine(sq.ToText());
Console.WriteLine("\n> Model:");
WriteLineSelectQuery(sq);

Console.WriteLine("\n> Press enter key to exit");
Console.ReadLine();

static void WriteLineSelectQuery(SelectQuery sq, int level = 0)
{
	WriteLineWithIndent($"SelectQuery:{sq.ToOneLineText()}", level);

	WriteLineWithIndent($"CommonTables:{sq.GetCommonTables().Count()}", level + 1);
	foreach (var item in sq.GetCommonTables())
	{
		WriteLineSelectableTable(item, level + 2);
		if (item.ColumnAliases != null)
		{
			WriteLineWithIndent($"ColumnAliases:{item.ColumnAliases.Count()}", level + 3);
			foreach (var ca in item.ColumnAliases)
			{
				WriteLineWithIndent($"Text:{ca.ToOneLineText()}", level + 4);
			}
		}
	}

	WriteLineWithIndent($"SelectableItems:{sq.GetSelectableItems().Count()}", level + 1);
	foreach (var item in sq.GetSelectableItems())
	{
		WriteLineSelectableItem(item, level + 2);
	}

	WriteLineWithIndent($"SelectableTables:{sq.GetSelectableTables().Count()}", level + 1);
	foreach (var item in sq.GetSelectableTables())
	{
		WriteLineSelectableTable(item, level + 2);
	}
}

static void WriteLineValuesQuery(ValuesQuery vq, int level = 0)
{
	WriteLineWithIndent($"ValuesQuery:{vq.ToOneLineText()}", level);

	WriteLineWithIndent($"CommonTables:{vq.GetCommonTables().Count()}", level + 1);
	foreach (var item in vq.GetCommonTables())
	{
		WriteLineSelectableTable(item, level + 2);
	}

	WriteLineWithIndent($"Rows:{vq.Rows.Count()}", level + 1);
	foreach (var row in vq.Rows)
	{
		WriteLineWithIndent($"Columns:{row.Count()}", level + 2);
		foreach (var col in row)
		{
			WriteLineWithIndent($"Text:{col.ToOneLineText()}", level + 3);
		}
	}
}

static void WriteLineSelectableItem(SelectableItem item, int level = 0)
{
	WriteLineWithIndent($"SelectableItem:{item.Value.ToOneLineText()}", level);
	WriteLineWithIndent($"Alias:{item.Alias}", level + 1);
}

static void WriteLineSelectableTable(SelectableTable table, int level = 0)
{
	WriteLineWithIndent($"SelectableTable:{table.Table.ToOneLineText()}", level);
	WriteLineWithIndent($"Alias:{table.Alias}", level + 1);
	WriteLineWithIndent($"Type:{table.Table.GetType().Name}", level + 1);
	if (table.Table is VirtualTable vt)
	{
		if (vt.Query is SelectQuery sq)
		{
			WriteLineSelectQuery(sq, level + 1);
		}
		else if (vt.Query is ValuesQuery vq)
		{
			WriteLineValuesQuery(vq, level + 1);
		}
	}
}

static void WriteLineWithIndent(string value, int level, int limit = 80)
{
	var indentation = new string(' ', level * 4);
	value = indentation + value;

	if (value.Length > limit)
	{
		value = value.Substring(0, limit) + "...";
	}
	Console.WriteLine(value);
}

static string ReadMultiLine()
{
	StringBuilder sb = new StringBuilder();

	while (true)
	{
		var line = Console.ReadLine();
		if (string.IsNullOrEmpty(line))
		{
			break;
		}
		sb.AppendLine(line);
	}
	return sb.ToString();
}