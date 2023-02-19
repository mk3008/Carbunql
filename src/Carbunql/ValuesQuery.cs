using Carbunql.Analysis;
using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;
using Carbunql.Values;

namespace Carbunql;

public class ValuesQuery : ReadQuery
{
    public ValuesQuery(List<ValueCollection> rows)
    {
        Rows = rows;
    }

    public ValuesQuery(string query)
    {
        var q = ValuesQueryParser.Parse(query);
        Rows = q.Rows;
        OperatableQuery = q.OperatableQuery;
        OrderClause = q.OrderClause;
        LimitClause = q.LimitClause;
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

    public override SelectQuery GetOrNewSelectQuery()
    {
        return ToSelectQuery();
    }

    public override IDictionary<string, object?> GetInnerParameters()
    {
        var prm = EmptyParameters.Get();
        Rows.ForEach(x => prm = prm.Merge(x.GetParameters()));
        return prm;
    }

    public SelectQuery ToSelectQuery()
    {
        if (!Rows.Any() || Rows.First().Count() == 0) throw new Exception();
        var cnt = Rows.First().Count();

        var columnAlias = new ValueCollection();
        cnt.ForEach(x => columnAlias.Add(new LiteralValue("c" + x)));

        return ToSelectQuery(columnAlias);
    }

    public SelectQuery ToSelectQuery(ValueCollection columnAlias)
    {
        var sq = new SelectQuery();

        var vt = new VirtualTable(this);
        var f = sq.From(vt.ToSelectable("v", columnAlias));

        foreach (var item in columnAlias) sq.Select(f, item.ToText());

        sq.OrderClause = OrderClause;
        sq.LimitClause = LimitClause;

        sq.Parameters = Parameters;

        return sq;
    }
}