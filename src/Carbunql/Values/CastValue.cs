using Carbunql.Analysis.Parser;
using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Values;

/// <summary>
/// Represents a cast value.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class CastValue : ValueBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CastValue"/> class.
    /// </summary>
    public CastValue()
    {
        Inner = null!;
        Symbol = null!;
        Type = null!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CastValue"/> class with the specified inner value, symbol, and type.
    /// </summary>
    /// <param name="inner">The inner value.</param>
    /// <param name="symbol">The symbol.</param>
    /// <param name="type">The type value.</param>
    public CastValue(ValueBase inner, string symbol, ValueBase type)
    {
        Inner = inner;
        Symbol = symbol;
        Type = type;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CastValue"/> class with the specified inner value, symbol, and type.
    /// </summary>
    /// <param name="inner">The inner value.</param>
    /// <param name="symbol">The symbol.</param>
    /// <param name="type">The type.</param>
    public CastValue(ValueBase inner, string symbol, string type)
    {
        Inner = inner;
        Symbol = symbol;
        Type = ValueParser.Parse(type);
    }

    /// <summary>
    /// Gets or sets the inner value.
    /// </summary>
    public ValueBase Inner { get; init; }

    /// <summary>
    /// Gets or sets the symbol.
    /// </summary>
    public string Symbol { get; init; }

    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    public ValueBase Type { get; init; }

    /// <inheritdoc/>
    protected override IEnumerable<SelectQuery> GetInternalQueriesCore()
    {
        foreach (var item in Inner.GetInternalQueries())
        {
            yield return item;
        }
        foreach (var item in Type.GetInternalQueries())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        if (Symbol.IsEqualNoCase("as"))
        {
            yield return Token.Reserved(this, parent, "cast");

            var bracket = Token.ExpressionBracketStart(this, parent);
            yield return bracket;
            foreach (var item in Inner.GetTokens(bracket)) yield return item;
            yield return Token.Reserved(this, bracket, Symbol);
            foreach (var item in Type.GetTokens(bracket)) yield return item;
            yield return Token.ExpressionBracketEnd(this, parent);
        }
        else
        {
            foreach (var item in Inner.GetTokens(parent)) yield return item;
            yield return Token.Reserved(this, parent, Symbol);
            foreach (var item in Type.GetTokens(parent)) yield return item;
        }
    }

    /// <inheritdoc/>
    protected override IEnumerable<QueryParameter> GetParametersCore()
    {
        foreach (var item in Inner.GetParameters())
        {
            yield return item;
        }
        foreach (var item in Type.GetParameters())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    protected override IEnumerable<PhysicalTable> GetPhysicalTablesCore()
    {
        foreach (var item in Inner.GetPhysicalTables())
        {
            yield return item;
        }
        foreach (var item in Type.GetPhysicalTables())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    protected override IEnumerable<CommonTable> GetCommonTablesCore()
    {
        foreach (var item in Inner.GetCommonTables())
        {
            yield return item;
        }
        foreach (var item in Type.GetCommonTables())
        {
            yield return item;
        }
    }

    internal override IEnumerable<ColumnValue> GetColumnsCore()
    {
        foreach (var item in Inner.GetColumns())
        {
            yield return item;
        }
    }
}
