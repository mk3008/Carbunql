﻿using Carbunql.Analysis;
using Carbunql.Analysis.Parser;
using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;
using Carbunql.Values;
using MessagePack;

namespace Carbunql;

[MessagePackObject(keyAsPropertyName: true)]
public class ValuesQuery : ReadQuery
{
    public ValuesQuery()
    {
    }

    public ValuesQuery(List<ValueCollection> rows)
    {
        Rows = rows;
    }

    public ValuesQuery(string query)
    {
        var q = ValuesQueryParser.Parse(query);
        Rows = q.Rows;
        OperatableQueries = q.OperatableQueries;
        OrderClause = q.OrderClause;
        LimitClause = q.LimitClause;
    }


    [Obsolete("This feature has been deprecated. Consider building it outside of class.")]
    public ValuesQuery(IEnumerable<object> matrix)
    {
        foreach (var row in matrix)
        {
            var lst = new List<ValueBase>();

            foreach (var column in row.GetType().GetProperties().Where(x => x.CanRead && x.CanWrite).Select(x => x.GetValue(row)))
            {
                if (column == null || column.ToString() == null)
                {
                    lst.Add(new LiteralValue("null"));
                }
                else
                {
                    var v = $"\"{column}\"";
                    lst.Add(ValueParser.Parse(column.ToString()!));
                }
            }
            Rows.Add(new ValueCollection(lst));
        }
    }

    [Obsolete("This feature has been deprecated. Consider building it outside of class.")]
    public ValuesQuery(IEnumerable<object> matrix, string placeholderIndentifer)
    {
        var r = 0;

        foreach (var row in matrix)
        {
            var lst = new List<ValueBase>();
            var c = 0;
            foreach (var column in row.GetType().GetProperties().Where(x => x.CanRead && x.CanWrite).Select(x => x.GetValue(row)))
            {
                var name = $"r{r}c{c}";
                var v = placeholderIndentifer + name;
                lst.Add(ValueParser.Parse(v));
                AddParameter(name, column);
                c++;
            }
            Rows.Add(new ValueCollection(lst));
            r++;
        }
    }

    [Obsolete("This feature has been deprecated. Consider building it outside of class.")]
    public ValuesQuery(string[,] matrix)
    {
        for (int row = 0; row < matrix.GetLength(0); row++)
        {
            var lst = new List<ValueBase>();

            for (int column = 0; column < matrix.GetLength(1); column++)
            {
                var v = $"\"{matrix[row, column]}\"";
                lst.Add(ValueParser.Parse(v));
            }
            Rows.Add(new ValueCollection(lst));
        }
    }

    [Obsolete("This feature has been deprecated. Consider building it outside of class.")]
    public ValuesQuery(IEnumerable<IEnumerable<string>> matrix)
    {
        foreach (var row in matrix)
        {
            var lst = new List<ValueBase>();
            foreach (var column in row)
            {
                var v = $"\"{column}\"";
                lst.Add(ValueParser.Parse(v));
            }
            Rows.Add(new ValueCollection(lst));
        }
    }

    [Obsolete("This feature has been deprecated. Consider building it outside of class.")]
    public ValuesQuery(string[,] matrix, string placeholderIndentifer)
    {
        for (int row = 0; row < matrix.GetLength(0); row++)
        {
            var lst = new List<ValueBase>();

            for (int column = 0; column < matrix.GetLength(1); column++)
            {
                var name = $"r{row}c{column}";
                var v = placeholderIndentifer + name;
                lst.Add(ValueParser.Parse(v));
                AddParameter(name, matrix[row, column]);
            }
            Rows.Add(new ValueCollection(lst));
        }
    }

    [Obsolete("This feature has been deprecated. Consider building it outside of class.")]
    public ValuesQuery(IEnumerable<IEnumerable<string>> matrix, string placeholderIndentifer)
    {
        var r = 0;
        foreach (var row in matrix)
        {
            var c = 0;
            var lst = new List<ValueBase>();
            foreach (var column in row)
            {
                var name = $"r{r}c{c}";
                var v = placeholderIndentifer + name;
                lst.Add(ValueParser.Parse(v));
                AddParameter(name, column);
                c++;
            }
            Rows.Add(new ValueCollection(lst));
            r++;
        }
    }

    public List<ValueCollection> Rows { get; init; } = new();

    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        var clause = Token.Reserved(this, parent, "values");
        yield return clause;

        var isFirst = true;
        foreach (var item in Rows)
        {
            if (isFirst)
            {
                isFirst = false;
            }
            else
            {
                yield return Token.Comma(this, clause);
            }
            var bracket = Token.ReservedBracketStart(this, clause);
            yield return bracket;
            foreach (var token in item.GetTokens(bracket)) yield return token;
            yield return Token.ReservedBracketEnd(this, clause);
        }
    }

    public override WithClause? GetWithClause() => null;

    public override SelectClause? GetSelectClause() => null;

    public override IEnumerable<SelectQuery> GetInternalQueries()
    {
        foreach (var row in Rows)
        {
            foreach (var item in row.GetInternalQueries())
            {
                yield return item;
            }
        }
    }

    public override IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        foreach (var row in Rows)
        {
            foreach (var item in row.GetPhysicalTables())
            {
                yield return item;
            }
        }
    }

    public override IEnumerable<CommonTable> GetCommonTables()
    {
        foreach (var row in Rows)
        {
            foreach (var item in row.GetCommonTables())
            {
                yield return item;
            }
        }
    }

    public override SelectQuery GetOrNewSelectQuery()
    {
        return ToSelectQuery();
    }

    public override IEnumerable<QueryParameter> GetInnerParameters()
    {
        foreach (var item in Rows)
        {
            foreach (var p in item.GetParameters())
            {
                yield return p;
            }
        }
    }

    public SelectQuery ToPlainSelectQuery()
    {
        var lst = GetDefaultColumnAliases();
        return ToPlainSelectQuery(lst);
    }

    public SelectQuery ToPlainSelectQuery(IEnumerable<string> columnAlias)
    {
        var columns = columnAlias.ToList();
        SelectQuery? sq = null;

        foreach (var row in Rows)
        {
            if (sq == null)
            {
                sq = row.ToPlainSelectQuery(columns);
            }
            else
            {
                sq.AddOperatableValue("union all", row.ToPlainSelectQuery(columns));
            }
        }
        if (sq == null) throw new InvalidOperationException();

        return sq;
    }

    public SelectQuery ToSelectQuery()
    {
        var lst = GetDefaultColumnAliases();
        return ToSelectQuery(lst);
    }

    public SelectQuery ToSelectQuery(IEnumerable<string> columnAlias)
    {
        var sq = new SelectQuery();
        var f = sq.From(ToSelectableTable(columnAlias));

        foreach (var item in columnAlias) sq.Select(f, item);

        sq.OrderClause = OrderClause;
        sq.LimitClause = LimitClause;

        sq.Parameters = Parameters;

        return sq;
    }

    private List<string> GetDefaultColumnAliases()
    {
        if (!Rows.Any() || Rows.First().Count == 0) throw new Exception();
        var cnt = Rows.First().Count;

        var lst = new List<string>();
        cnt.ForEach(x => lst.Add("c" + x));

        return lst;
    }

    public override SelectableTable ToSelectableTable(IEnumerable<string>? columnAliases)
    {
        var vt = new VirtualTable(this);
        if (columnAliases == null)
        {
            var lst = GetDefaultColumnAliases();
            return vt.ToSelectable("v", lst);
        }
        else
        {
            return vt.ToSelectable("v", columnAliases);
        }
    }

    public override IEnumerable<string> GetColumnNames()
    {
        return Enumerable.Empty<string>();
    }
}