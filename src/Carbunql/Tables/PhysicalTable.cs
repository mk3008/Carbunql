using Carbunql.Clauses;
using MessagePack;
using System.Collections.Immutable;

namespace Carbunql.Tables;

/// <summary>
/// Represents a physical table.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class PhysicalTable : TableBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PhysicalTable"/> class.
    /// </summary>
    public PhysicalTable()
    {
        Table = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PhysicalTable"/> class with the specified table name.
    /// </summary>
    /// <param name="table">The name of the table.</param>
    public PhysicalTable(string table)
    {
        Table = table;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PhysicalTable"/> class with the specified schema and table names.
    /// </summary>
    /// <param name="schema">The schema name.</param>
    /// <param name="table">The table name.</param>
    public PhysicalTable(string schema, string table)
    {
        Schema = schema;
        Table = table;
    }

    /// <summary>
    /// Gets or sets the schema name.
    /// </summary>
    public string? Schema { get; init; }

    /// <summary>
    /// Gets or sets the table name.
    /// </summary>
    public string Table { get; init; }

    /// <summary>
    /// Gets or sets the list of column names.
    /// </summary>
    public List<string>? ColumnNames { get; set; }

    /// <inheritdoc/>
    public override IEnumerable<Token> GetTokens(Token? parent)
    {
        if (!string.IsNullOrEmpty(Schema))
        {
            yield return new Token(this, parent, Schema);
            yield return Token.Dot(this, parent);
        }
        yield return new Token(this, parent, Table);
    }

    /// <inheritdoc/>
    public override string GetDefaultName() => Table;

    /// <inheritdoc/>
    public override string GetTableFullName() => !string.IsNullOrEmpty(Schema) ? Schema + "." + Table : Table;

    /// <inheritdoc/>
    public override IList<string> GetColumnNames()
    {
        if (ColumnNames == null) return ImmutableList<string>.Empty;
        return ColumnNames;
    }

    /// <inheritdoc/>
    public override IEnumerable<SelectQuery> GetInternalQueries()
    {
        yield break;
    }

    /// <inheritdoc/>
    public override IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        yield return this;
    }

    /// <inheritdoc/>
    public override IEnumerable<CommonTable> GetCommonTables()
    {
        yield break;
    }
}
