using Carbunql.Clauses;
using Carbunql.Tables;
namespace Carbunql.Values;

/// <summary>
/// Represents a SQL function that does not require parentheses or arguments.
/// ex.curren_timestamp
/// </summary>
public class BareFunctionValue(string name) : ValueBase
{

    /// <summary>
    /// Gets the name of the SQL function.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Retrieves the internal queries of the function.
    /// </summary>
    /// <returns>An empty enumeration, as this function does not have internal queries.</returns>
    protected override IEnumerable<SelectQuery> GetInternalQueriesCore()
    {
        yield break;
    }

    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        yield return Token.Reserved(this, parent, Name);
    }

    /// <summary>
    /// Retrieves the query parameters of the function.
    /// </summary>
    /// <returns>An empty enumeration, as this function does not have parameters.</returns>
    protected override IEnumerable<QueryParameter> GetParametersCore()
    {
        yield break;
    }

    /// <summary>
    /// Retrieves the physical tables referenced by the function.
    /// </summary>
    /// <returns>An empty enumeration, as this function does not reference physical tables.</returns>
    protected override IEnumerable<PhysicalTable> GetPhysicalTablesCore()
    {
        yield break;
    }

    /// <summary>
    /// Retrieves the common tables referenced by the function.
    /// </summary>
    /// <returns>An empty enumeration, as this function does not reference common tables.</returns>
    protected override IEnumerable<CommonTable> GetCommonTablesCore()
    {
        yield break;
    }

    internal override IEnumerable<ColumnValue> GetColumnsCore()
    {
        yield break;
    }

    public override IEnumerable<ValueBase> GetValues()
    {
        yield return this;
    }
}
