using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Clauses;

using Carbunql.Values;
using System.Collections.Generic;

/// <summary>
/// Represents a WHERE clause in a SQL query.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class WhereClause : IQueryCommandable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WhereClause"/> class with the specified condition.
    /// </summary>
    /// <param name="condition">The boolean expression representing the condition for the WHERE clause.</param>
    public WhereClause(ValueBase condition)
    {
        Condition = condition;
    }

    /// <summary>
    /// Gets or sets the condition associated with the WHERE clause, which represents a boolean expression.
    /// </summary>
    public ValueBase Condition { get; init; }

    /// <summary>
    /// Retrieves the query parameters associated with this WHERE clause.
    /// </summary>
    /// <returns>An enumerable collection of query parameters.</returns>
    public IEnumerable<QueryParameter> GetParameters()
    {
        return Condition.GetParameters();
    }

    /// <summary>
    /// Retrieves the tokens associated with this WHERE clause.
    /// </summary>
    /// <param name="parent">The parent token.</param>
    /// <returns>An enumerable collection of tokens.</returns>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        var clause = Token.Reserved(this, parent, "where");
        yield return clause;
        foreach (var item in Condition.GetTokens(clause)) yield return item;
    }

    /// <summary>
    /// Retrieves the internal queries associated with this WHERE clause.
    /// </summary>
    /// <returns>An enumerable collection of internal queries.</returns>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        foreach (var item in Condition.GetInternalQueries())
        {
            yield return item;
        }
    }

    /// <summary>
    /// Retrieves the physical tables associated with this WHERE clause.
    /// </summary>
    /// <returns>An enumerable collection of physical tables.</returns>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        foreach (var item in Condition.GetPhysicalTables())
        {
            yield return item;
        }
    }

    /// <summary>
    /// Retrieves the common tables associated with this WHERE clause.
    /// </summary>
    /// <returns>An enumerable collection of common tables.</returns>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        foreach (var item in Condition.GetCommonTables())
        {
            yield return item;
        }
    }

    public IEnumerable<ColumnValue> GetColumns()
    {
        foreach (var item in Condition.GetColumns())
        {
            yield return item;
        }
    }

    public IEnumerable<SelectableTable> GetSelectableTables()
    {
        foreach (var item in Condition.GetValues()
            .OfType<QueryContainer>()
            .Select(x => x.Query)
            .OfType<SelectQuery>()
            .SelectMany(x => x.GetSelectableTables()))
        {
            yield return item;
        }
    }
}
