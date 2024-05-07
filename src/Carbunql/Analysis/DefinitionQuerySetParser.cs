using Carbunql.Definitions;
using Carbunql.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Analysis;

/// <summary>
/// Provides functionality to parse a set of definition queries in SQL.
/// </summary>
public static class DefinitionQuerySetParser
{
    /// <summary>
    /// Parses the specified definition query string and returns a list of DefinitionQuerySet objects.
    /// </summary>
    /// <param name="text">The definition query string.</param>
    /// <returns>A list of DefinitionQuerySet objects.</returns>
    public static DefinitionQuerySetList Parse(string text)
    {
        var r = new SqlTokenReader(text);
        var dic = new Dictionary<string, DefinitionQuerySet>();

        while (TryParse(r, out var t))
        {
            if (!r.Peek().IsEndToken())
            {
                throw new NotSupportedException($"Parsing terminated despite the presence of unparsed tokens. (Token: '{r.Peek()}')");
            }

            if (!dic.ContainsKey(t.GetTableFullName()))
            {
                dic.Add(t.GetTableFullName(), new DefinitionQuerySet(t));
            }
            var qs = dic[t.GetTableFullName()];

            if (t is CreateTableQuery table)
            {
                qs.CreateTableQuery = table;
            }
            else if (t is AlterTableQuery alter)
            {
                qs.AddAlterTableQuery(alter);
            }
            else if (t is CreateIndexQuery index)
            {
                qs.AddAlterIndexQuery(index);
            }
            else
            {
                throw new NotSupportedException($"Unsupported query type: {t.GetType().Name}");
            }
        }

        var lst = new DefinitionQuerySetList();
        dic.ForEach(x => lst.Add(x.Value));
        return lst;
    }

    /// <summary>
    /// Tries to parse the next query from the token reader.
    /// </summary>
    /// <param name="r">The SqlTokenReader instance.</param>
    /// <param name="t">The parsed table object.</param>
    /// <returns>True if parsing is successful, otherwise false.</returns>
    private static bool TryParse(SqlTokenReader r, [MaybeNullWhen(false)] out ITable t)
    {
        t = default;
        if (r.TryReadNextQuery(out var token))
        {
            if (token.IsEqualNoCase("create table"))
            {
                t = CreateTableQueryParser.Parse(r);
                return true;
            }
            else if (token.IsEqualNoCase("alter table"))
            {
                t = AlterTableQueryParser.Parse(r);
                return true;
            }
            else if (token.IsEqualNoCase("create index") || token.IsEqualNoCase("create unique index"))
            {
                t = CreateIndexQueryParser.Parse(r);
                return true;
            }
        }
        return false;
    }
}
