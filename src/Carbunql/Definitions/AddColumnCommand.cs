using Carbunql.Clauses;
using Carbunql.Tables;
using Carbunql.Values;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Definitions;

/// <summary>
/// Represents a command to add a column.
/// </summary>
public class AddColumnCommand : IAlterCommand
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddColumnCommand"/> class with the specified column definition.
    /// </summary>
    /// <param name="definition">The column definition to be added.</param>
    public AddColumnCommand(ColumnDefinition definition)
    {
        Definition = definition;
    }

    /// <summary>
    /// Gets or sets the column definition to be added.
    /// </summary>
    public ColumnDefinition Definition { get; set; }

    /// <summary>
    /// Gets the schema of the column to be added.
    /// </summary>
    public string Schema => Definition.Schema;

    /// <summary>
    /// Gets the table of the column to be added.
    /// </summary>
    public string Table => Definition.Table;

    public IEnumerable<ColumnValue> GetColumns()
    {
        yield break;
    }

    /// <summary>
    /// Gets the common tables affected by this command.
    /// </summary>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        yield break;
    }

    /// <summary>
    /// Gets the internal queries affected by this command.
    /// </summary>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        yield break;
    }

    /// <summary>
    /// Gets the parameters used in this command.
    /// </summary>
    public IEnumerable<QueryParameter> GetParameters()
    {
        yield break;
    }

    /// <summary>
    /// Gets the physical tables affected by this command.
    /// </summary>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        yield break;
    }

    /// <summary>
    /// Gets the tokens representing this command.
    /// </summary>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        yield return new Token(this, parent, "add", isReserved: true);
        yield return new Token(this, parent, "column", isReserved: true);
        foreach (var item in Definition.GetTokens(parent))
        {
            yield return item;
        }
    }

    /// <summary>
    /// Tries to set the specified column definition clause with this command.
    /// </summary>
    /// <param name="clause">The table definition clause to set.</param>
    /// <returns><see langword="true"/> if the setting was successful; otherwise, <see langword="false"/>.</returns>
    public bool TrySet(TableDefinitionClause clause)
    {
        var q = clause.OfType<ColumnDefinition>().Where(x => x.ColumnName == Definition.ColumnName);
        if (q.Any()) throw new InvalidOperationException();

        clause.Add(Definition);
        return true;
    }

    /// <summary>
    /// Tries to convert this command to an index creation query.
    /// </summary>
    /// <param name="query">When this method returns, contains the index creation query if the conversion succeeded, or <see langword="null"/> if the conversion failed.</param>
    /// <returns><see langword="true"/> if the conversion succeeded; otherwise, <see langword="false"/>.</returns>
    public bool TryToIndex([MaybeNullWhen(false)] out CreateIndexQuery query)
    {
        query = default;
        return false;
    }
}