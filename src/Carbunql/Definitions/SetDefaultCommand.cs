using Carbunql.Clauses;
using Carbunql.Tables;
using Carbunql.Values;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Definitions;

public class SetDefaultCommand : IAlterCommand
{
    public SetDefaultCommand(ITable t, string columnName, string defaultValue)
    {
        ColumnName = columnName;
        DefaultValue = new LiteralValue(defaultValue);
        Schema = t.Schema;
        Table = t.Table;
    }

    public SetDefaultCommand(ITable t, string columnName, ValueBase defaultValue)
    {
        ColumnName = columnName;
        DefaultValue = defaultValue;
        Schema = t.Schema;
        Table = t.Table;
    }

    public string ColumnName { get; set; }

    public ValueBase DefaultValue { get; set; }

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
        yield return new Token(this, parent, "alter column", isReserved: true);
        yield return new Token(this, parent, ColumnName);
        yield return new Token(this, parent, "set", isReserved: true);
        yield return new Token(this, parent, "default", isReserved: true);
        foreach (var item in DefaultValue.GetTokens(parent))
        {
            yield return item;
        }
    }

    public bool TrySet(TableDefinitionClause clause)
    {
        var c = clause.OfType<ColumnDefinition>().Where(x => x.ColumnName == ColumnName).First();
        c.DefaultValue = DefaultValue;
        return true;
    }

    public bool TryToIndex([MaybeNullWhen(false)] out CreateIndexQuery query)
    {
        query = default;
        return false;
    }
}