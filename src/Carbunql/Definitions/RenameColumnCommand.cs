using Carbunql.Clauses;
using Carbunql.Tables;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Definitions;

public class RenameColumnCommand : IAlterCommand
{
    public RenameColumnCommand(ITable t, string oldColumnName, string newColumnName)
    {
        OldColumnName = oldColumnName;
        NewColumnName = newColumnName;
        Schema = t.Schema;
        Table = t.Table;
    }

    public string OldColumnName { get; set; }

    public string NewColumnName { get; set; }

    public string Schema { get; init; }

    public string Table { get; init; } = string.Empty;

    public IEnumerable<CommonTable> GetCommonTables()
    {
        yield break;
    }

    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        yield break;
    }

    public IEnumerable<QueryParameter> GetParameters()
    {
        yield break;
    }

    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        yield break;
    }

    public IEnumerable<Token> GetTokens(Token? parent)
    {
        yield return new Token(this, parent, "rename", isReserved: true);
        yield return new Token(this, parent, "column", isReserved: true);
        yield return new Token(this, parent, OldColumnName);
        yield return new Token(this, parent, "to", isReserved: true);
        yield return new Token(this, parent, NewColumnName);
    }

    public bool TrySet(TableDefinitionClause clause)
    {
        // Do not normalize as it will be treated as a Drop or Add.
        return false;
    }

    public bool TryToIndex([MaybeNullWhen(false)] out CreateIndexQuery query)
    {
        query = default;
        return false;
    }
}