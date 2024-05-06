using Carbunql.Clauses;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses an INSERT clause from SQL text or token streams.
/// </summary>
public static class InsertClauseParser
{
    /// <summary>
    /// Parses an INSERT clause from SQL text.
    /// </summary>
    /// <param name="text">The SQL text containing the INSERT clause.</param>
    /// <returns>The parsed INSERT clause.</returns>
    public static InsertClause Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

    /// <summary>
    /// Parses an INSERT clause from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed INSERT clause.</returns>
    public static InsertClause Parse(ITokenReader r)
    {
        r.Read("insert into");

        var t = PhysicalTableParser.Parse(r);

        if (r.Peek() != "(") return new InsertClause(t);

        r.Read("(");
        var aliases = ValueCollectionParser.Parse(r);
        r.Read(")");

        return new InsertClause(t)
        {
            ColumnAliases = aliases,
        };
    }
}
