using Carbunql.Tables;
using Carbunql.Values;

namespace Carbunql.Clauses;

public class InsertClause : IQueryCommandable
{
    public InsertClause(PhysicalTable table)
    {
        Table = table;
    }

    public PhysicalTable Table { get; init; }

    public ValueCollection? ColumnAliases { get; init; }

    public IEnumerable<CommonTable> GetCommonTables()
    {
        foreach (var item in Table.GetCommonTables())
        {
            yield return item;
        }
    }

    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        foreach (var item in Table.GetInternalQueries())
        {
            yield return item;
        }
    }

    public IEnumerable<QueryParameter> GetParameters()
    {
        return Table.GetParameters();
    }

    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        foreach (var item in Table.GetPhysicalTables())
        {
            yield return item;
        }
    }

    public IEnumerable<Token> GetTokens(Token? parent)
    {
        var t = Token.Reserved(this, parent, "insert into");
        yield return t;
        foreach (var item in Table.GetTokens(t)) yield return item;

        if (ColumnAliases != null)
        {
            var bracket = Token.ReservedBracketStart(this, t);
            yield return bracket;
            foreach (var item in ColumnAliases.GetTokens(bracket)) yield return item;
            yield return Token.ReservedBracketEnd(this, t);
        }
    }
}