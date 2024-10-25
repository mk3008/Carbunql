using Carbunql.Analysis;
using Carbunql.Analysis.Parser;
using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;

namespace Carbunql.Fluent;

/// <summary>
/// The FluentTable class manages information related to a database table, 
/// including the table itself, column aliases, CTE (Common Table Expression) names, 
/// and aliases. It provides methods to create instances of FluentTable from various 
/// input sources such as SQL queries and command text.
/// </summary>
public class FluentTable(TableBase table, IEnumerable<string> columnAliases, string cteName, string alias)
{
    /// <summary>
    /// Creates a FluentTable instance from a SelectQuery object with a specified alias.
    /// </summary>
    /// <param name="query">The SelectQuery object representing the SQL query.</param>
    /// <param name="alias">The alias name for the table.</param>
    /// <returns>A new instance of FluentTable.</returns>
    public static FluentTable Create(SelectQuery query, string alias)
    {
        return Create(query, Enumerable.Empty<string>(), string.Empty, alias);
    }

    /// <summary>
    /// Creates a FluentTable instance from a SelectQuery object with a specified CTE name and alias.
    /// </summary>
    /// <param name="query">The SelectQuery object representing the SQL query.</param>
    /// <param name="cteName">The name of the Common Table Expression (CTE), if any.</param>
    /// <param name="alias">The alias name for the table.</param>
    /// <returns>A new instance of FluentTable.</returns>
    public static FluentTable Create(SelectQuery query, string cteName, string alias)
    {
        return Create(query, Enumerable.Empty<string>(), cteName, alias);
    }

    /// <summary>
    /// Creates a FluentTable instance from a SelectQuery object with specified column aliases, CTE name, and alias.
    /// </summary>
    /// <param name="query">The SelectQuery object representing the SQL query.</param>
    /// <param name="columnAliases">The list of column aliases associated with the table.</param>
    /// <param name="cteName">The name of the Common Table Expression (CTE), if any.</param>
    /// <param name="alias">The alias name for the table.</param>
    /// <returns>A new instance of FluentTable.</returns>
    public static FluentTable Create(SelectQuery query, IEnumerable<string> columnAliases, string cteName, string alias)
    {
        if (string.IsNullOrEmpty(alias)) throw new ArgumentNullException(nameof(alias));
        return new FluentTable(new VirtualTable(query), columnAliases, cteName, alias);
    }

    /// <summary>
    /// Creates a FluentTable instance from an SQL query string with specified column aliases, CTE name, and alias.
    /// </summary>
    /// <param name="query">The SQL query string.</param>
    /// <param name="columnAliases">The list of column aliases associated with the table.</param>
    /// <param name="cteName">The name of the Common Table Expression (CTE), if any.</param>
    /// <param name="alias">The alias name for the table.</param>
    /// <returns>A new instance of FluentTable.</returns>
    public static FluentTable Create(string query, IEnumerable<string> columnAliases, string cteName, string alias)
    {
        if (string.IsNullOrEmpty(alias)) throw new ArgumentNullException(nameof(alias));

        var r = new SqlTokenReader(query);
        if (r.Peek().IsEqualNoCase(["with", "select"]))
        {
            var sq = SelectQuery.Parse(query);
            return new FluentTable(new VirtualTable(sq), columnAliases, cteName, alias);
        }
        if (r.Peek().IsEqualNoCase("values"))
        {
            if (!columnAliases.Any()) throw new Exception();

            var vq = ValuesQueryParser.Parse(query);
            return new FluentTable(new VirtualTable(vq), columnAliases, cteName, alias);
        }
        else
        {
            var t = TableParser.Parse(query);
            if (t is PhysicalTable pt)
            {
                if (!string.IsNullOrEmpty(cteName))
                {
                    var ft = Create(pt.GetTableFullName(), columnAliases, pt.GetDefaultName());
                    ft.HideColumnAliases = true;

                    var sq = new SelectQuery()
                        .From(ft);

                    return Create(sq, Enumerable.Empty<string>(), cteName, alias);

                    //return Create($"select {string.Join(",", columnAliases)} from {pt.GetTableFullName()}", Enumerable.Empty<string>(), cteName, alias);
                }

                //return new FluentTable(pt, columnAliases, cteName, alias);
                return new FluentTable(pt, columnAliases, cteName, alias)
                {
                    HideColumnAliases = true
                };
            }

            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Creates a FluentTable instance from an SQL command text with a specified alias.
    /// </summary>
    /// <param name="commandText">The SQL command text.</param>
    /// <param name="alias">The alias name for the table.</param>
    /// <returns>A new instance of FluentTable.</returns>
    public static FluentTable Create(string commandText, string alias)
    {
        return Create(commandText, Enumerable.Empty<string>(), string.Empty, alias);
    }

    /// <summary>
    /// Creates a FluentTable instance from an SQL command text with a specified CTE name and alias.
    /// </summary>
    /// <param name="commandText">The SQL command text.</param>
    /// <param name="cteName">The name of the Common Table Expression (CTE), if any.</param>
    /// <param name="alias">The alias name for the table.</param>
    /// <returns>A new instance of FluentTable.</returns>
    public static FluentTable Create(string commandText, string cteName, string alias)
    {
        return Create(commandText, Enumerable.Empty<string>(), cteName, alias);
    }

    /// <summary>
    /// Creates a FluentTable instance from an SQL command text with specified column names and alias.
    /// </summary>
    /// <param name="commandText">The SQL command text.</param>
    /// <param name="columnNames">The list of column names associated with the table.</param>
    /// <param name="alias">The alias name for the table.</param>
    /// <returns>A new instance of FluentTable.</returns>
    public static FluentTable Create(string commandText, IEnumerable<string> columnNames, string alias)
    {
        return Create(commandText, columnNames, string.Empty, alias);
    }

    /// <summary>
    /// Gets the base table object associated with the FluentTable.
    /// </summary>
    public TableBase Table { get; } = table;

    /// <summary>
    /// Gets the list of column aliases associated with the FluentTable.
    /// </summary>
    public IEnumerable<string> ColumnAliases { get; } = columnAliases;

    /// <summary>
    /// Determines whether column aliases should be hidden in the SQL output.
    /// </summary>
    /// <remarks>
    /// When set to <c>true</c>, column aliases will be omitted from the generated SQL statements.
    /// The default value is <c>false</c>, which includes column aliases in the SQL output.
    /// </remarks>
    public bool HideColumnAliases { get; private set; } = false;

    /// <summary>
    /// Gets the name of the Common Table Expression (CTE), if any.
    /// </summary>
    public string CteName { get; } = cteName;

    /// <summary>
    /// Gets the alias name for the FluentTable.
    /// </summary>
    public string Alias { get; } = alias;

    public Materialized Materialized { get; set; } = Materialized.Undefined;

    /// <summary>
    /// Gets a value indicating whether the table is a Common Table Expression (CTE).
    /// </summary>
    public bool IsCommonTable => string.IsNullOrEmpty(CteName) ? false : true;

    /// <summary>
    /// Converts the FluentTable to a CommonTable instance.
    /// </summary>
    /// <returns>A new instance of CommonTable.</returns>
    /// <exception cref="NotSupportedException">Thrown if the table is not a Common Table Expression (CTE).</exception>
    public CommonTable ToCommonTable()
    {
        if (!IsCommonTable) throw new NotSupportedException();

        var columnAliases = ColumnAliases.ToValueCollection();

        if (ColumnAliases.Any() && HideColumnAliases == false)
        {
            return new CommonTable(Table, CteName, columnAliases)
            {
                Materialized = Materialized
            };
        }
        else
        {
            return new CommonTable(Table, CteName)
            {
                Materialized = Materialized
            };
        }
    }

    /// <summary>
    /// Converts the FluentTable to a SelectableTable instance.
    /// </summary>
    /// <returns>A new instance of SelectableTable.</returns>
    public SelectableTable ToSelectable()
    {
        var columnAliases = ColumnAliases.ToValueCollection();

        if (!IsCommonTable)
        {
            if (columnAliases.Any())
            {
                return new SelectableTable(Table, Alias, columnAliases)
                {
                    HideColumnAliases = HideColumnAliases
                };
            }
            else
            {
                return new SelectableTable(Table, Alias);
            }
        }
        else
        {
            return new SelectableTable(new PhysicalTable(CteName), Alias);
        }
    }
}
