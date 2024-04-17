namespace Carbunql.Definitions;

public class ReferenceDefinition
{
    public ReferenceDefinition(string tableName, List<string> columnNames)
    {
        TableName = tableName;
        ColumnNames = columnNames;
    }

    public string TableName { get; init; } = string.Empty;

    public List<string> ColumnNames { get; set; } = new();

    public string Option { get; set; } = string.Empty;

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
