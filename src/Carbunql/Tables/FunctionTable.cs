using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Tables;

/// <summary>
/// Represents a function table.
/// </summary>
public class FunctionTable : TableBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FunctionTable"/> class with the specified name.
    /// </summary>
    /// <param name="name">The name of the function table.</param>
    public FunctionTable(string name)
    {
        Name = name;
        Argument = new ValueCollection();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FunctionTable"/> class with the specified name and arguments.
    /// </summary>
    /// <param name="name">The name of the function table.</param>
    /// <param name="args">The arguments of the function table.</param>
    public FunctionTable(string name, ValueBase args)
    {
        Name = name;
        Argument = new ValueCollection { args };
    }

    /// <summary>
    /// Gets or sets the name of the function associated with the function table.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Gets or sets the arguments of the function associated with the function table.
    /// </summary>
    public ValueCollection Argument { get; init; }

    /// <inheritdoc/>
    public override IEnumerable<Token> GetTokens(Token? parent)
    {
        yield return Token.Reserved(this, parent, Name);

        var bracket = Token.ReservedBracketStart(this, parent);
        yield return bracket;
        foreach (var item in Argument.GetTokens(bracket)) yield return item;
        yield return Token.ReservedBracketEnd(this, parent);
    }

    /// <summary>
    /// Gets or sets the list of parameters associated with the function.
    /// </summary>
    public List<QueryParameter> Parameters { get; set; } = new();

    /// <inheritdoc/>
    public override IEnumerable<QueryParameter> GetParameters()
    {
        return Parameters;
    }

    /// <inheritdoc/>
    public override IEnumerable<SelectQuery> GetInternalQueries()
    {
        foreach (var item in Argument.GetInternalQueries())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    public override IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        foreach (var item in Argument.GetPhysicalTables())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    public override IEnumerable<CommonTable> GetCommonTables()
    {
        foreach (var item in Argument.GetCommonTables())
        {
            yield return item;
        }
    }
}
