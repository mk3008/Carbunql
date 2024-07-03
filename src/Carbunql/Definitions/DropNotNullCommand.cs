﻿using Carbunql.Clauses;
using Carbunql.Tables;
using Carbunql.Values;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Definitions;

/// <summary>
/// Represents a command to drop the NOT NULL constraint from a column.
/// </summary>
public class DropNotNullCommand : IAlterCommand
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DropNotNullCommand"/> class with the specified table and column name.
    /// </summary>
    /// <param name="t">The table containing the column.</param>
    /// <param name="columnName">The name of the column from which the NOT NULL constraint will be dropped.</param>
    public DropNotNullCommand(ITable t, string columnName)
    {
        ColumnName = columnName;
        Schema = t.Schema;
        Table = t.Table;
    }

    /// <summary>
    /// Gets or sets the name of the column from which the NOT NULL constraint will be dropped.
    /// </summary>
    public string ColumnName { get; set; }

    /// <summary>
    /// Gets or sets the schema of the table containing the column.
    /// </summary>
    public string Schema { get; init; }

    /// <summary>
    /// Gets or sets the name of the table containing the column.
    /// </summary>
    public string Table { get; init; }

    public IEnumerable<ColumnValue> GetColumns()
    {
        yield break;
    }

    /// <summary>
    /// Gets the common tables associated with the drop NOT NULL command (currently empty).
    /// </summary>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        yield break;
    }

    /// <summary>
    /// Gets the internal queries associated with the drop NOT NULL command (currently empty).
    /// </summary>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        yield break;
    }

    /// <summary>
    /// Gets the parameters associated with the drop NOT NULL command (currently empty).
    /// </summary>
    public IEnumerable<QueryParameter> GetParameters()
    {
        yield break;
    }

    /// <summary>
    /// Gets the physical tables associated with the drop NOT NULL command (currently empty).
    /// </summary>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        yield break;
    }

    /// <summary>
    /// Gets the tokens representing the drop NOT NULL command.
    /// </summary>
    /// <param name="parent">The parent token.</param>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        yield return new Token(this, parent, "alter column", isReserved: true);
        yield return new Token(this, parent, ColumnName);
        yield return new Token(this, parent, "drop", isReserved: true);
        yield return new Token(this, parent, "not null", isReserved: true);
    }

    /// <summary>
    /// Attempts to apply the drop NOT NULL command to a table definition clause.
    /// </summary>
    /// <param name="clause">The table definition clause to which the command will be applied.</param>
    /// <returns><c>true</c> if the command was successfully applied; otherwise, <c>false</c>.</returns>
    public bool TrySet(TableDefinitionClause clause)
    {
        var c = clause.OfType<ColumnDefinition>().Where(x => x.ColumnName == ColumnName).First();
        c.IsNullable = true;
        return true;
    }

    /// <summary>
    /// Tries to convert the drop NOT NULL command to a create index query.
    /// </summary>
    /// <param name="query">When this method returns, contains the create index query, if conversion succeeded; otherwise, <c>null</c>.</param>
    /// <returns><c>true</c> if the conversion succeeded; otherwise, <c>false</c>.</returns>
    public bool TryToIndex([MaybeNullWhen(false)] out CreateIndexQuery query)
    {
        query = default;
        return false;
    }
}
