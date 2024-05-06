using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses ALTER TABLE clauses from SQL text or token streams.
/// </summary>
public static class AlterTableClauseParser
{
    /// <summary>
    /// Parses the ALTER TABLE clause from SQL text.
    /// </summary>
    /// <param name="text">The SQL text containing the ALTER TABLE clause.</param>
    /// <returns>The parsed ALTER TABLE clause.</returns>
    /// <exception cref="NotSupportedException">Thrown when parsing is terminated due to unparsed tokens.</exception>
    public static AlterTableClause Parse(string text)
    {
        var r = new SqlTokenReader(text);
        var q = Parse(r);

        if (!r.Peek().IsEndToken())
        {
            throw new NotSupportedException($"Parsing terminated despite the presence of unparsed tokens. (Token:'{r.Peek()}')");
        }

        return q;
    }

    /// <summary>
    /// Parses the ALTER TABLE clause from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed ALTER TABLE clause.</returns>
    public static AlterTableClause Parse(ITokenReader r)
    {
        var t = ParseAsAlterTableCommand(r);
        do
        {
            t.Add(AlterCommandParser.Parse(t, r));
        } while (r.TryRead(",", out _));
        return t;
    }

    /// <summary>
    /// Parses the "alter table" command.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed ALTER TABLE clause.</returns>
    private static AlterTableClause ParseAsAlterTableCommand(ITokenReader r)
    {
        r.Read("alter table");

        var token = r.Read();

        var schema = string.Empty;
        string? table;
        if (r.Peek() == ".")
        {
            r.Read(".");
            schema = token;
            table = r.Read();
        }
        else
        {
            table = token;
        }

        return new AlterTableClause(schema, table);
    }
}
