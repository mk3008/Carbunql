using Carbunql.Analysis.Parser;
using Carbunql.Clauses;
using Carbunql.Tables;
using Carbunql.Values;

namespace Carbunql.Building;

/// <summary>
/// Provides methods for creating and editing matrix tables.
/// </summary>
public static class MatrixEditor
{
    /// <summary>
    /// Creates a matrix table from the provided key values and column headers.
    /// </summary>
    /// <param name="key">The key column name.</param>
    /// <param name="keyvalues">The list of key values.</param>
    /// <param name="columnHeaders">The list of column headers.</param>
    /// <returns>A <see cref="CommonTable"/> representing the matrix table.</returns>
    public static CommonTable Create(string key, List<string> keyvalues, List<string> columnHeaders)
    {
        var rows = new List<ValueCollection>();
        var index = 0;
        foreach (var keyvalue in keyvalues)
        {
            rows.Add(CreateValueCollection(keyvalue, columnHeaders, index));
            index++;
        }

        var vq = new ValuesQuery(rows);
        var headers = new List<string>() { key };
        columnHeaders.ForEach(x => headers.Add(x.Trim()));

        return vq.ToCommonTable("matrix", headers);
    }

    /// <summary>
    /// Adds a total summary column to the matrix table.
    /// </summary>
    /// <param name="table">The matrix table.</param>
    /// <param name="columnName">The name of the total summary column.</param>
    /// <exception cref="NotSupportedException">Thrown when the table type is not supported.</exception>
    public static void AddTotalSummaryColumn(CommonTable table, string columnName = "total_summary")
    {
        if (table.Table is VirtualTable t && t.Query is ValuesQuery vq)
        {
            var vals = table.ColumnAliases;
            vals!.Add(new LiteralValue(columnName));

            foreach (var row in vq.Rows)
            {
                row.Add(new LiteralValue("1"));
            }
            return;
        }

        throw new NotSupportedException();
    }

    private static ValueCollection CreateValueCollection(string keyvalue, List<string> columns, int index)
    {
        var row = new ValueCollection() { ValueParser.Parse(keyvalue) };

        foreach (var item in columns)
        {
            if (columns.IndexOf(item) == index)
            {
                row.Add(new LiteralValue("1"));
            }
            else
            {
                row.Add(new LiteralValue("0"));
            }
        }

        return row;
    }
}
