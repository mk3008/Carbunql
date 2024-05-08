﻿using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Clauses;

/// <summary>
/// Represents a selectable item in a query.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class SelectableItem : IQueryCommandable, ISelectable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SelectableItem"/> class with the specified value and alias.
    /// </summary>
    /// <param name="value">The value to be selected.</param>
    /// <param name="alias">The alias for the selected value.</param>
    public SelectableItem(ValueBase value, string alias)
    {
        Value = value;
        Alias = alias;
    }

    /// <summary>
    /// Gets or sets the value to be selected.
    /// </summary>
    public ValueBase Value { get; private set; }

    /// <summary>
    /// Gets or sets the alias for the selected value.
    /// </summary>
    public string Alias { get; private set; }

    /// <summary>
    /// Sets the value to be selected.
    /// </summary>
    /// <param name="value">The value to be selected.</param>
    public void SetValue(ValueBase value)
    {
        Value = value;
    }

    /// <summary>
    /// Sets the alias for the selected value.
    /// </summary>
    /// <param name="alias">The alias for the selected value.</param>
    public void SetAlias(string alias)
    {
        Alias = alias;
    }

    /// <inheritdoc/>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        foreach (var item in Value.GetInternalQueries())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        foreach (var item in Value.GetPhysicalTables())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        foreach (var item in Value.GetCommonTables())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        foreach (var item in Value.GetTokens(parent)) yield return item;
        if (!string.IsNullOrEmpty(Alias) && Alias != Value.GetDefaultName())
        {
            yield return Token.Reserved(this, parent, "as");
            yield return new Token(this, parent, Alias);
        }
    }

    /// <inheritdoc/>
    public IEnumerable<QueryParameter> GetParameters()
    {
        return Value.GetParameters();
    }
}
