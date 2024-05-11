using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Clauses;

/// <summary>
/// Represents a relationship between tables in a query.
/// This class manages the join command and condition for joining tables in a query.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class Relation : IQueryCommandable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Relation"/> class.
    /// Constructs a relation without specifying any join command, condition, or table.
    /// </summary>
    public Relation()
    {
        JoinCommand = string.Empty;
        Condition = null;
        Table = null!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Relation"/> class with the specified table and join command.
    /// Constructs a relation with the specified table and join command.
    /// </summary>
    /// <param name="query">The selectable table.</param>
    /// <param name="joinCommand">The join command (e.g., INNER JOIN, LEFT JOIN).</param>
    public Relation(SelectableTable query, string joinCommand)
    {
        Table = query;
        JoinCommand = joinCommand;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Relation"/> class with the specified table, join command, and condition.
    /// Constructs a relation with the specified table, join command, and condition.
    /// </summary>
    /// <param name="query">The selectable table.</param>
    /// <param name="joinCommand">The join command (e.g., INNER JOIN, LEFT JOIN).</param>
    /// <param name="condition">The join condition.</param>
    public Relation(SelectableTable query, string joinCommand, ValueBase condition)
    {
        Table = query;
        JoinCommand = joinCommand;
        Condition = condition;
    }

    /// <summary>
    /// Gets or sets the join command (e.g., INNER JOIN, LEFT JOIN).
    /// </summary>
    public string JoinCommand { get; init; }

    /// <summary>
    /// Gets or sets the join condition, which represents the condition used for joining tables.
    /// </summary>
    public ValueBase? Condition { get; set; }

    /// <summary>
    /// Gets or sets the selectable table involved in the relation, which represents the table being joined with.
    /// </summary>
    public SelectableTable Table { get; init; }

    /// <inheritdoc/>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        foreach (var item in Table.GetInternalQueries())
        {
            yield return item;
        }
        if (Condition != null)
        {
            foreach (var item in Condition.GetInternalQueries())
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        foreach (var item in Table.GetPhysicalTables())
        {
            yield return item;
        }
        if (Condition != null)
        {
            foreach (var item in Condition.GetPhysicalTables())
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        foreach (var item in Table.GetCommonTables())
        {
            yield return item;
        }
        if (Condition != null)
        {
            foreach (var item in Condition.GetCommonTables())
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<QueryParameter> GetParameters()
    {
        foreach (var item in Table.GetParameters())
        {
            yield return item;
        }
        if (Condition != null)
        {
            foreach (var item in Condition.GetParameters())
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        yield return Token.Reserved(this, parent, JoinCommand);
        foreach (var item in Table.GetTokens(parent)) yield return item;

        if (Condition != null)
        {
            yield return Token.Reserved(this, parent, "on");
            foreach (var item in Condition.GetTokens(parent)) yield return item;
        }
    }
}
