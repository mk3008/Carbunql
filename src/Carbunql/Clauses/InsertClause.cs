using Carbunql.Tables;
using Carbunql.Values;

namespace Carbunql.Clauses;

/// <summary>
/// Represents an INSERT INTO clause in a SQL query.
/// </summary>
public class InsertClause : IQueryCommandable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InsertClause"/> class with the specified physical table.
    /// </summary>
    /// <param name="table">The physical table to insert into.</param>
    public InsertClause(PhysicalTable table)
    {
        Table = table;
    }

    /// <summary>
    /// Gets or sets the physical table to insert into.
    /// </summary>
    public PhysicalTable Table { get; init; }

    /// <summary>
    /// Gets or sets the column aliases for the INSERT INTO clause.
    /// </summary>
    public ValueCollection? ColumnAliases { get; init; }

    public IEnumerable<ColumnValue> GetColumns()
    {
        yield break;
    }

    /// <summary>
    /// Gets the common tables associated with this INSERT INTO clause.
    /// </summary>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        foreach (var item in Table.GetCommonTables())
        {
            yield return item;
        }
    }

    /// <summary>
    /// Gets the internal queries associated with this INSERT INTO clause.
    /// </summary>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        foreach (var item in Table.GetInternalQueries())
        {
            yield return item;
        }
    }

    /// <summary>
    /// Gets the parameters associated with this INSERT INTO clause.
    /// </summary>
    public IEnumerable<QueryParameter> GetParameters()
    {
        return Table.GetParameters();
    }

    /// <summary>
    /// Gets the physical tables associated with this INSERT INTO clause.
    /// </summary>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        foreach (var item in Table.GetPhysicalTables())
        {
            yield return item;
        }
    }

    /// <summary>
    /// Gets the tokens representing this INSERT INTO clause.
    /// </summary>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        var t = Token.Reserved(this, parent, "insert into");
        yield return t;
        foreach (var item in Table.GetTokens(t)) yield return item;

        if (ColumnAliases != null)
        {
            var bracket = Token.ReservedBracketStart(this, t);
            yield return bracket;
            foreach (var item in ColumnAliases.GetTokens(bracket)) yield return item;
            yield return Token.ReservedBracketEnd(this, t);
        }
    }
}
