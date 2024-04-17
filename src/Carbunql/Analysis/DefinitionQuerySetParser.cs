using Carbunql.Definitions;
using Carbunql.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Analysis;

public static class DefinitionQuerySetParser
{
    public static DefinitionQuerySetList Parse(string text)
    {
        var r = new SqlTokenReader(text);
        var dic = new Dictionary<string, DefinitionQuerySet>();

        while (TryParse(r, out var t))
        {
            if (!r.Peek().IsEndToken())
            {
                throw new NotSupportedException($"Parsing terminated despite the presence of unparsed tokens.(token:'{r.Peek()}')");
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
                throw new NotSupportedException();
            }
        }

        var lst = new DefinitionQuerySetList();
        dic.ForEach(x => lst.Add(x.Value));
        return lst;
    }

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
