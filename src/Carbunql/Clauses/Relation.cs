using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Clauses;

/// <summary>
/// Represents a relation in a query.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class Relation : IQueryCommandable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Relation"/> class.
    /// </summary>
    public Relation()
    {
        JoinCommand = string.Empty;
        Condition = null;
        Table = null!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Relation"/> class with the specified query and join command.
    /// </summary>
    /// <param name="query">The selectable table.</param>
    /// <param name="joinCommand">The join command.</param>
    public Relation(SelectableTable query, string joinCommand)
    {
        Table = query;
        JoinCommand = joinCommand;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Relation"/> class with the specified query, join command, and condition.
    /// </summary>
    /// <param name="query">The selectable table.</param>
    /// <param name="joinCommand">The join command.</param>
    /// <param name="condition">The join condition.</param>
    public Relation(SelectableTable query, string joinCommand, ValueBase condition)
    {
        Table = query;
        JoinCommand = joinCommand;
        Condition = condition;
    }

    /// <summary>
    /// Gets or sets the join command.
    /// </summary>
    public string JoinCommand { get; init; }

    /// <summary>
    /// Gets or sets the join condition.
    /// </summary>
    public ValueBase? Condition { get; set; }

    /// <summary>
    /// Gets or sets the selectable table.
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
