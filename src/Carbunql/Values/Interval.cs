using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Values;

[MessagePackObject(keyAsPropertyName: true)]
public class Interval : ValueBase
{
    public Interval(ValueBase argument)
    {
        Argument = argument;
    }

    public ValueBase Argument { get; set; }

    /// <inheritdoc/>
    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        var t = new Token(this, parent, "interval", true);
        yield return t;
        foreach (var item in Argument.GetTokens(t))
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    protected override IEnumerable<SelectQuery> GetInternalQueriesCore()
    {
        foreach (var item in Argument.GetInternalQueries())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    protected override IEnumerable<QueryParameter> GetParametersCore()
    {
        return Argument.GetParameters();
    }

    /// <inheritdoc/>
    protected override IEnumerable<PhysicalTable> GetPhysicalTablesCore()
    {
        foreach (var item in Argument.GetPhysicalTables())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    protected override IEnumerable<CommonTable> GetCommonTablesCore()
    {
        foreach (var item in Argument.GetCommonTables())
        {
            yield return item;
        }
    }
}
