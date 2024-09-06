using Carbunql.Building;
using Carbunql.Extensions;
using Carbunql.Tables;
using Carbunql.Values;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Clauses;

/// <summary>
/// Represents the base class for various types of tables in a query, including physical tables, views, function tables, and lateral tables.
/// </summary>
public abstract class TableBase : IQueryCommandable
{
    /// <summary>
    /// Gets the tokens representing this table in a query.
    /// </summary>
    /// <param name="parent">The parent token.</param>
    /// <returns>The tokens representing this table.</returns>
    public abstract IEnumerable<Token> GetTokens(Token? parent);

    /// <summary>
    /// Gets the default name of the table.
    /// </summary>
    /// <returns>The default name of the table.</returns>
    public virtual string GetDefaultName() => string.Empty;

    /// <summary>
    /// Converts the table to a selectable table with the specified alias.
    /// </summary>
    /// <param name="alias">The alias for the table.</param>
    /// <returns>A selectable table representing this table with the specified alias.</returns>
    public virtual SelectableTable ToSelectable() => ToSelectable(GetDefaultName());

    /// <summary>
    /// Converts the table to a selectable table with the specified alias.
    /// </summary>
    /// <param name="alias">The alias for the table.</param>
    /// <returns>A selectable table representing this table with the specified alias.</returns>
    public virtual SelectableTable ToSelectable(string alias)
    {
        return new SelectableTable(this, alias);
    }

    /// <summary>
    /// Converts the table to a selectable table with the specified alias and column aliases.
    /// </summary>
    /// <param name="alias">The alias for the table.</param>
    /// <param name="columnAliases">The column aliases.</param>
    /// <returns>A selectable table representing this table with the specified alias and column aliases.</returns>
    public virtual SelectableTable ToSelectable(string alias, IEnumerable<string> columnAliases)
    {
        return new SelectableTable(this, alias, columnAliases.ToValueCollection());
    }

    /// <summary>
    /// Converts the table to a selectable table with the specified alias and column aliases.
    /// </summary>
    /// <param name="alias">The alias for the table.</param>
    /// <param name="columnAliases">The column aliases.</param>
    /// <returns>A selectable table representing this table with the specified alias and column aliases.</returns>
    public virtual SelectableTable ToSelectable(string alias, ValueCollection columnAliases)
    {
        return new SelectableTable(this, alias, columnAliases);
    }

    /// <summary>
    /// Gets the parameters associated with this table.
    /// </summary>
    /// <returns>The parameters associated with this table.</returns>
    public virtual IEnumerable<QueryParameter> GetParameters()
    {
        yield break;
    }

    /// <summary>
    /// Gets the names of the columns in this table.
    /// </summary>
    /// <returns>The names of the columns in this table.</returns>
    public virtual IList<string> GetColumnNames()
    {
        return ImmutableList<string>.Empty;
    }

    /// <summary>
    /// Gets a value indicating whether this table is a select query.
    /// </summary>
    public virtual bool IsSelectQuery => false;

    /// <summary>
    /// Gets the full name of the table.
    /// </summary>
    /// <returns>The full name of the table.</returns>
    public virtual string GetTableFullName() => "";

    /// <summary>
    /// Gets the select query associated with this table.
    /// </summary>
    /// <returns>The select query associated with this table.</returns>
    public virtual SelectQuery GetSelectQuery() => throw new NotSupportedException();

    public bool TryGetSelectQuery([MaybeNullWhen(false)] out SelectQuery query)
    {
        if (IsSelectQuery)
        {
            query = GetSelectQuery();
            return true;
        }
        query = null;
        return false;
    }

    /// <summary>
    /// Gets the internal queries associated with this table.
    /// </summary>
    /// <returns>The internal queries associated with this table.</returns>
    public abstract IEnumerable<SelectQuery> GetInternalQueries();

    /// <summary>
    /// Gets the physical tables associated with this table.
    /// </summary>
    /// <returns>The physical tables associated with this table.</returns>
    public abstract IEnumerable<PhysicalTable> GetPhysicalTables();

    /// <summary>
    /// Gets the common tables associated with this table.
    /// </summary>
    /// <returns>The common tables associated with this table.</returns>
    public abstract IEnumerable<CommonTable> GetCommonTables();

    public IEnumerable<ColumnValue> GetColumns()
    {
        yield break;
    }
}
