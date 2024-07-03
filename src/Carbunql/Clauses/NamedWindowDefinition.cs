using Carbunql.Tables;
using Carbunql.Values;
using MessagePack;

namespace Carbunql.Clauses;

/// <summary>
/// Represents a named window definition in a SQL query.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class NamedWindowDefinition : IQueryCommandable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NamedWindowDefinition"/> class.
    /// </summary>
    public NamedWindowDefinition()
    {
        Alias = null!;
        WindowDefinition = null!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NamedWindowDefinition"/> class with the specified alias.
    /// </summary>
    /// <param name="alias">The alias for the named window.</param>
    public NamedWindowDefinition(string alias)
    {
        Alias = alias;
        WindowDefinition = null!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NamedWindowDefinition"/> class with the specified alias and window definition.
    /// </summary>
    /// <param name="alias">The alias for the named window.</param>
    /// <param name="definition">The window definition.</param>
    public NamedWindowDefinition(string alias, WindowDefinition definition)
    {
        Alias = alias;
        WindowDefinition = definition;
    }

    /// <summary>
    /// Gets or sets the alias for the named window.
    /// </summary>
    public string Alias { get; set; }

    /// <summary>
    /// Gets or sets the window definition.
    /// </summary>
    public WindowDefinition WindowDefinition { get; set; }

    /// <inheritdoc/>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        foreach (var item in WindowDefinition.GetInternalQueries()) yield return item;
    }

    /// <inheritdoc/>
    public IEnumerable<QueryParameter> GetParameters()
    {
        return WindowDefinition.GetParameters();
    }

    /// <inheritdoc/>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        foreach (var item in WindowDefinition.GetPhysicalTables()) yield return item;
    }

    /// <inheritdoc/>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        foreach (var item in WindowDefinition.GetCommonTables()) yield return item;
    }

    /// <inheritdoc/>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        yield return new Token(this, parent, Alias);

        yield return Token.Reserved(this, parent, "as");

        foreach (var item in WindowDefinition.GetTokens(parent)) yield return item;
    }

    public IEnumerable<ColumnValue> GetColumns()
    {
        foreach (var item in WindowDefinition.GetColumns())
        {
            yield return item;
        }
    }
}

