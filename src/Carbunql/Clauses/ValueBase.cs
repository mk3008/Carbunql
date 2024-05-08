using Carbunql.Analysis.Parser;
using Carbunql.Extensions;
using Carbunql.Tables;
using Carbunql.Values;
using MessagePack;

namespace Carbunql.Clauses;

/// <summary>
/// Represents the base class for SQL values used in queries.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
[Union(0, typeof(LiteralValue))]
[Union(1, typeof(AsArgument))]
[Union(2, typeof(BetweenClause))]
[Union(3, typeof(BracketValue))]
[Union(4, typeof(CaseExpression))]
[Union(5, typeof(CastValue))]
[Union(6, typeof(ColumnValue))]
[Union(7, typeof(FromArgument))]
[Union(8, typeof(FunctionValue))]
[Union(9, typeof(InClause))]
[Union(10, typeof(NegativeValue))]
[Union(11, typeof(ParameterValue))]
[Union(12, typeof(QueryContainer))]
[Union(13, typeof(ValueCollection))]
public abstract class ValueBase : IQueryCommandable
{
    /// <summary>
    /// Gets the default name for the value.
    /// </summary>
    /// <returns>The default name, which is an empty string.</returns>
    public virtual string GetDefaultName() => string.Empty;

    /// <summary>
    /// Gets or sets the operatable value associated with this value.
    /// </summary>
    public OperatableValue? OperatableValue { get; set; }

    /// <summary>
    /// Adds an operatable value with the specified operator and value.
    /// </summary>
    /// <param name="operator">The operator to add.</param>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance of <see cref="ValueBase"/>.</returns>
    public ValueBase AddOperatableValue(string @operator, ValueBase value)
    {
        if (OperatableValue != null)
        {
            OperatableValue.Value.AddOperatableValue(@operator, value);
            return this;
        }
        OperatableValue = new OperatableValue(@operator, value);
        return this;
    }

    /// <summary>
    /// Gets or sets the recommended name for the value.
    /// </summary>
    public string RecommendedName { get; set; } = string.Empty;

    /// <summary>
    /// Adds an operatable value with the specified operator and value.
    /// </summary>
    /// <param name="operator">The operator to add.</param>
    /// <param name="value">The value to add as a string.</param>
    /// <returns>The current instance of <see cref="ValueBase"/>.</returns>
    public ValueBase AddOperatableValue(string @operator, string value)
    {
        return AddOperatableValue(@operator, ValueParser.Parse(value));
    }

    /// <summary>
    /// Retrieves all operators associated with this value.
    /// </summary>
    /// <returns>An enumerable collection of operators.</returns>
    public IEnumerable<string> GetOperators()
    {
        if (OperatableValue == null) yield break;
        yield return OperatableValue.Operator;
        foreach (var item in OperatableValue.Value.GetOperators()) yield return item;
    }

    /// <summary>
    /// Retrieves all query parameters associated with this value.
    /// </summary>
    /// <returns>An enumerable collection of query parameters.</returns>
    public virtual IEnumerable<QueryParameter> GetParameters()
    {
        foreach (var item in GetParametersCore())
        {
            yield return item;
        }
        var q = OperatableValue?.GetParameters();
        if (q != null)
        {
            foreach (var item in q)
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Retrieves all internal queries associated with this value.
    /// </summary>
    /// <returns>An enumerable collection of internal queries.</returns>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        foreach (var item in GetInternalQueriesCore())
        {
            yield return item;
        }
        if (OperatableValue != null)
        {
            foreach (var item in OperatableValue.GetInternalQueries())
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Retrieves all physical tables associated with this value.
    /// </summary>
    /// <returns>An enumerable collection of physical tables.</returns>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        foreach (var item in GetPhysicalTablesCore())
        {
            yield return item;
        }
        if (OperatableValue != null)
        {
            foreach (var item in OperatableValue.GetPhysicalTables())
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Retrieves all common tables associated with this value.
    /// </summary>
    /// <returns>An enumerable collection of common tables.</returns>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        foreach (var item in GetCommonTablesCore())
        {
            yield return item;
        }
        if (OperatableValue != null)
        {
            foreach (var item in OperatableValue.GetCommonTables())
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Retrieves all values associated with this value.
    /// </summary>
    /// <returns>An enumerable collection of values.</returns>
    public virtual IEnumerable<ValueBase> GetValues()
    {
        yield return this;

        if (OperatableValue != null)
        {
            foreach (var item in OperatableValue.Value.GetValues())
            {
                yield return item;
            }
        }
    }
    /// <summary>
    /// Gets the core query parameters associated with this value.
    /// </summary>
    /// <returns>An enumerable collection of core query parameters.</returns>
    protected abstract IEnumerable<QueryParameter> GetParametersCore();

    /// <summary>
    /// Gets the core internal queries associated with this value.
    /// </summary>
    /// <returns>An enumerable collection of core internal queries.</returns>
    protected abstract IEnumerable<SelectQuery> GetInternalQueriesCore();

    /// <summary>
    /// Gets the core physical tables associated with this value.
    /// </summary>
    /// <returns>An enumerable collection of core physical tables.</returns>
    protected abstract IEnumerable<PhysicalTable> GetPhysicalTablesCore();

    /// <summary>
    /// Gets the core common tables associated with this value.
    /// </summary>
    /// <returns>An enumerable collection of core common tables.</returns>
    protected abstract IEnumerable<CommonTable> GetCommonTablesCore();

    /// <summary>
    /// Retrieves the current tokens associated with this value.
    /// </summary>
    /// <param name="parent">The parent token.</param>
    /// <returns>An enumerable collection of tokens.</returns>
    public abstract IEnumerable<Token> GetCurrentTokens(Token? parent);

    /// <summary>
    /// Gets all tokens associated with this value.
    /// </summary>
    /// <param name="parent">The parent token.</param>
    /// <returns>An enumerable collection of tokens.</returns>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        foreach (var item in GetCurrentTokens(parent)) yield return item;

        if (OperatableValue != null)
        {
            foreach (var item in OperatableValue.GetTokens(parent)) yield return item;
        }
    }

    /// <summary>
    /// Converts this value to a bracketed value.
    /// </summary>
    /// <returns>A new bracketed value.</returns>
    public BracketValue ToBracket()
    {
        return new BracketValue(this);
    }

    /// <summary>
    /// Converts this value to a where clause.
    /// </summary>
    /// <returns>A new where clause.</returns>
    public WhereClause ToWhereClause()
    {
        return new WhereClause(this);
    }

    /// <summary>
    /// Converts this value to its textual representation.
    /// </summary>
    /// <returns>The textual representation of this value.</returns>
    public string ToText()
    {
        return GetTokens(null).ToText();
    }
}