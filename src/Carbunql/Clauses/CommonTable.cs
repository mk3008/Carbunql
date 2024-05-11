using Carbunql.Extensions;
using Carbunql.Tables;
using Carbunql.Values;
using MessagePack;

namespace Carbunql.Clauses;

/// <summary>
/// Represents a common table expression (CTE) in SQL.
/// </summary>
/// <remarks>
/// A Common Table Expression (CTE) is a temporary result set that can be referenced within a SELECT, INSERT, UPDATE, or DELETE statement.
/// It allows for defining a named temporary result set that can be referenced multiple times in a query.
/// </remarks>
[MessagePackObject(keyAsPropertyName: true)]
public class CommonTable : SelectableTable

{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommonTable"/> class with the specified table and alias.
    /// </summary>
    /// <param name="table">The virtual table represented by a query or subquery.</param>
    /// <param name="alias">The alias assigned to the common table.</param>
    public CommonTable(TableBase table, string alias) : base(table, alias)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommonTable"/> class with the specified table, alias, and column aliases.
    /// </summary>
    /// <param name="table">The virtual table represented by a query or subquery.</param>
    /// <param name="alias">The alias assigned to the common table.</param>
    /// <param name="columnAliases">The aliases assigned to the columns of the common table.</param>
    public CommonTable(TableBase table, string alias, ValueCollection columnAliases) : base(table, alias, columnAliases)
    {
    }

    /// <summary>
    /// Gets or sets the materialization type of the common table expression (CTE).
    /// </summary>
    /// <remarks>
    /// The materialization type indicates whether the CTE should be materialized or not.
    /// In databases like PostgreSQL, the MATERIALIZED keyword is used to force materialization of the CTE, storing its result in a temporary table.
    /// </remarks>
    public Materialized Materialized { get; set; } = Materialized.Undefined;

    /// <summary>
    /// Gets the tokens representing this common table expression.
    /// </summary>
    public override IEnumerable<Token> GetTokens(Token? parent)
    {
        foreach (var item in GetAliasTokens(parent)) yield return item;
        yield return Token.Reserved(this, parent, "as");

        if (Materialized != Materialized.Undefined)
        {
            yield return Token.Reserved(this, parent, Materialized.ToCommandText());
        }

        foreach (var item in Table.GetTokens(parent)) yield return item;
    }

    /// <summary>
    /// Gets a value indicating whether this common table expression is based on a SELECT query.
    /// </summary>
    public bool IsSelectQuery => Table.IsSelectQuery;

    /// <summary>
    /// Gets the underlying SELECT query if this common table expression is based on a SELECT query.
    /// </summary>
    public SelectQuery GetSelectQuery() => Table.GetSelectQuery();

    /// <summary>
    /// Gets the common table expressions associated with this common table.
    /// </summary>
    public override IEnumerable<CommonTable> GetCommonTables()
    {
        if (Table is VirtualTable v)
        {
            foreach (var item in v.GetCommonTables())
            {
                yield return item;
            }
        }

        yield return this;
    }
}
