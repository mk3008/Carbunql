using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Clauses;

/// <summary>
/// Represents an OVER clause in a SQL query.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class OverClause : IQueryCommandable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OverClause"/> class.
    /// </summary>
    public OverClause()
    {
        WindowDefinition = null!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OverClause"/> class with the specified window definition.
    /// </summary>
    /// <param name="definition">The window definition.</param>
    public OverClause(WindowDefinition definition)
    {
        WindowDefinition = definition;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OverClause"/> class with the specified named window definition.
    /// </summary>
    /// <param name="definition">The named window definition.</param>
    public OverClause(NamedWindowDefinition definition)
    {
        WindowDefinition = new WindowDefinition(definition.Alias);
    }

    /// <summary>
    /// Gets or sets the window definition associated with the OVER clause.
    /// </summary>
    public WindowDefinition WindowDefinition { get; set; }

    /// <inheritdoc/>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        foreach (var item in WindowDefinition.GetCommonTables())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        foreach (var item in WindowDefinition.GetInternalQueries())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        foreach (var item in WindowDefinition.GetPhysicalTables())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    public IEnumerable<QueryParameter> GetParameters()
    {
        return WindowDefinition.GetParameters();
    }

    /// <inheritdoc/>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        var overToken = Token.Reserved(this, parent, "over");
        yield return overToken;

        foreach (var item in WindowDefinition.GetTokens(overToken)) yield return item;
    }
}
