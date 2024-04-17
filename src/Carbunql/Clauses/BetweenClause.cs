using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Clauses;

[MessagePackObject(keyAsPropertyName: true)]
public class BetweenClause : ValueBase
{
    public BetweenClause()
    {
        Value = null!;
        Start = null!;
        End = null!;
        IsNegative = false;
    }

    public BetweenClause(ValueBase value, ValueBase start, ValueBase end, bool isNegative)
    {
        Value = value;
        Start = start;
        End = end;
        IsNegative = isNegative;
    }

    public ValueBase Value { get; init; }

    public ValueBase Start { get; init; }

    public ValueBase End { get; init; }

    public bool IsNegative { get; init; }

    protected override IEnumerable<SelectQuery> GetInternalQueriesCore()
    {
        foreach (var item in Value.GetInternalQueries())
        {
            yield return item;
        }
        foreach (var item in Start.GetInternalQueries())
        {
            yield return item;
        }
        foreach (var item in End.GetInternalQueries())
        {
            yield return item;
        }
    }

    protected override IEnumerable<QueryParameter> GetParametersCore()
    {
        foreach (var item in Value.GetParameters())
        {
            yield return item;
        }
        foreach (var item in Start.GetParameters())
        {
            yield return item;
        }
        foreach (var item in End.GetParameters())
        {
            yield return item;
        }
    }

    protected override IEnumerable<PhysicalTable> GetPhysicalTablesCore()
    {
        foreach (var item in Value.GetPhysicalTables())
        {
            yield return item;
        }
        foreach (var item in Start.GetPhysicalTables())
        {
            yield return item;
        }
        foreach (var item in End.GetPhysicalTables())
        {
            yield return item;
        }
    }

    protected override IEnumerable<CommonTable> GetCommonTablesCore()
    {
        foreach (var item in Value.GetCommonTables())
        {
            yield return item;
        }
        foreach (var item in Start.GetCommonTables())
        {
            yield return item;
        }
        foreach (var item in End.GetCommonTables())
        {
            yield return item;
        }
    }

    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        foreach (var item in Value.GetTokens(parent)) yield return item;

        if (IsNegative) yield return Token.Reserved(this, parent, "not");

        yield return Token.Reserved(this, parent, "between");
        foreach (var item in Start.GetTokens(parent)) yield return item;
        yield return Token.Reserved(this, parent, "and");
        foreach (var item in End.GetTokens(parent)) yield return item;
    }
}