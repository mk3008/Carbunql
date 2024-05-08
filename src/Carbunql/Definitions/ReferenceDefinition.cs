namespace Carbunql.Definitions;

/// <summary>
/// Represents a reference definition for a foreign key constraint.
/// </summary>
public class ReferenceDefinition
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReferenceDefinition"/> class with table name and column names.
    /// </summary>
    public ReferenceDefinition(string tableName, List<string> columnNames)
    {
        TableName = tableName;
        ColumnNames = columnNames;
    }

    /// <summary>
    /// Gets or sets the name of the referenced table.
    /// </summary>
    public string TableName { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of column names in the referenced table.
    /// </summary>
    public List<string> ColumnNames { get; set; } = new();

    /// <summary>
    /// Gets or sets the option for the reference.
    /// </summary>
    public string Option { get; set; } = string.Empty;

    /// <summary>
    /// Generates tokens for the reference definition.
    /// </summary>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        yield return new Token(this, parent, "references", isReserved: true);
        yield return new Token(this, parent, TableName);

        yield return Token.ExpressionBracketStart(this, parent);
        foreach (var item in ColumnNames)
        {
            yield return new Token(this, parent, item);
        }
        yield return Token.ExpressionBracketEnd(this, parent);

        if (!string.IsNullOrEmpty(Option))
        {
            yield return new Token(this, parent, "on", isReserved: true);
            yield return new Token(this, parent, Option, isReserved: true);
        }
    }
}
