using Carbunql.Extensions;
using Carbunql.Tables;
using Carbunql.Values;
using MessagePack;

namespace Carbunql.Clauses;

/// <summary>
/// Represents a common table expression (CTE) in SQL.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class CommonTable : SelectableTable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommonTable"/> class with the specified table and alias.
    /// </summary>
    public CommonTable(TableBase table, string alias) : base(table, alias)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommonTable"/> class with the specified table, alias, and column aliases.
    /// </summary>
    public CommonTable(TableBase table, string alias, ValueCollection columnAliases) : base(table, alias, columnAliases)
    {
    }

    /// <summary>
    /// Gets or sets the materialization type of the common table expression.
    /// </summary>
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
