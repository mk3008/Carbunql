using Carbunql.Analysis.Parser;
using Carbunql.Extensions;

namespace Carbunql.Analysis;

public static class CreateTableQueryParser
{
    public static CreateTableQuery Parse(string text)
    {
        var r = new SqlTokenReader(text);
        var q = Parse(r);

        if (!r.Peek().IsEndToken())
        {
            throw new NotSupportedException($"Parsing terminated despite the presence of unparsed tokens.(token:'{r.Peek()}')");
        }

        return q;
    }

    static IEnumerable<string> ConstraintTokens => new[] { "primary key", "unique", "foreign key", "check", "not null", "constraint" };

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
            throw new NotSupportedException($"Token:{token}");
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