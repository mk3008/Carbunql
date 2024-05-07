using Carbunql.Analysis.Parser;
using Carbunql.Extensions;

namespace Carbunql.Analysis;

/// <summary>
/// Provides functionality to parse CREATE TABLE queries in SQL.
/// </summary>
public static class CreateTableQueryParser
{
    /// <summary>
    /// Parses the specified CREATE TABLE query string.
    /// </summary>
    /// <param name="text">The CREATE TABLE query string.</param>
    /// <returns>The parsed CreateTableQuery object.</returns>
    public static CreateTableQuery Parse(string text)
    {
        var r = new SqlTokenReader(text);
        var q = Parse(r);

        if (!r.Peek().IsEndToken())
        {
            throw new NotSupportedException($"Parsing terminated despite the presence of unparsed tokens. (Token: '{r.Peek()}')");
        }

        return q;
    }

    /// <summary>
    /// Parses the CREATE TABLE query using the provided ITokenReader.
    /// </summary>
    /// <param name="r">The ITokenReader instance.</param>
    /// <returns>The parsed CreateTableQuery object.</returns>
    public static CreateTableQuery Parse(ITokenReader r)
    {
        var t = ParseAsCreateTableCommand(r);

        var token = r.Peek();
        if (token.IsEqualNoCase("as"))
        {
            r.Read("as");
            t.Query = SelectQueryParser.Parse(r);
            return t;
        }
        else if (token == "(")
        {
            t.DefinitionClause ??= new(t);
            r.Read("(");
            do
            {
                r.ReadOrDefault(",");
                token = r.Peek();
                if (token.IsEqualNoCase(ConstraintTokens))
                {
                    var c = ConstraintParser.Parse(t, r);
                    t.DefinitionClause.Add(c);
                }
                else
                {
                    var c = ColumnDefinitionParser.Parse(t, r);
                    t.DefinitionClause.Add(c);
                }
            } while (r.Peek() == ",");
            r.Read(")");
        }

        return t;
    }

    /// <summary>
    /// The tokens representing constraints in CREATE TABLE queries.
    /// </summary>
    private static IEnumerable<string> ConstraintTokens => new[] { "primary key", "unique", "foreign key", "check", "not null", "constraint" };

    /// <summary>
    /// Parses the CREATE TABLE command.
    /// </summary>
    /// <param name="r">The ITokenReader instance.</param>
    /// <returns>The parsed CreateTableQuery object.</returns>
    private static CreateTableQuery ParseAsCreateTableCommand(ITokenReader r)
    {
        var isTemporary = false;
        var token = r.Read();
        if (token.IsEqualNoCase("create temporary table"))
        {
            isTemporary = true;
        }
        else if (token.IsEqualNoCase("create table"))
        {
            isTemporary = false;
        }
        else
        {
            throw new NotSupportedException($"Invalid CREATE TABLE command. (Token: '{token}')");
        }

        token = r.Read();
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

        return new CreateTableQuery(schema, table) { IsTemporary = isTemporary };
    }
}
