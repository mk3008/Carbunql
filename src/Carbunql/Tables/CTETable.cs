using Carbunql.Clauses;
using MessagePack;

namespace Carbunql.Tables;

/// <summary>
/// Represents a virtual table, which wraps a subquery and behaves like a table.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class CTETable(CommonTable commonTable) : TableBase
{

    /// <summary>
    /// Gets or sets the query associated with the virtual table.
    /// </summary>
    public CommonTable CommonTable = commonTable;

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
    public override IList<string> GetColumnNames()
    {
        return CommonTable.GetSelectQuery().GetColumnNames().ToList();
    }

    /// <inheritdoc/>
    public override bool IsSelectQuery => true;

    /// <inheritdoc/>
    public override SelectQuery GetSelectQuery()
    {
        return CommonTable.GetSelectQuery();
    }

    /// <inheritdoc/>
    public override IEnumerable<SelectQuery> GetInternalQueries()
    {
        yield break;
    }

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
}
