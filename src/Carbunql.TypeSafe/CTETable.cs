using Carbunql.Clauses;
using Carbunql.Tables;

namespace Carbunql.TypeSafe;

public class CTETable(CommonTable ct) : TableBase
{
    public CommonTable CommonTable { get; init; } = ct;

    /// <inheritdoc/>
    public override IEnumerable<Token> GetTokens(Token? parent)
    {
        yield return new Token(this, parent, CommonTable.Alias);
    }

    /// <inheritdoc/>
    public override IEnumerable<QueryParameter> GetParameters()
    {
        yield break;
    }

    /// <inheritdoc/>
    public override IList<string> GetColumnNames() => CommonTable.GetColumnNames().ToList();

    /// <inheritdoc/>
    public override IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        yield break;
    }

    /// <inheritdoc/>
    public override IEnumerable<CommonTable> GetCommonTables()
    {
        yield break;
    }

    /// <inheritdoc/>
    public override IEnumerable<SelectQuery> GetInternalQueries()
    {
        yield break;
    }
}
