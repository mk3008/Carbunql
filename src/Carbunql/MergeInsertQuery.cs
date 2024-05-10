using Carbunql.Clauses;
using Carbunql.Tables;
using Carbunql.Values;

namespace Carbunql;

/// <summary>
/// Represents a merge insert query.
/// </summary>
public class MergeInsertQuery : IQueryCommandable
{
    /// <summary>
    /// Gets or sets the datasource for the merge insert query.
    /// </summary>
    public ValueCollection? Datasource { get; set; }

    /// <summary>
    /// Gets or sets the destination for the merge insert query.
    /// </summary>
    public ValueCollection? Destination { get; set; }

    /// <summary>
    /// Retrieves the parameters of the merge insert query.
    /// </summary>
    /// <returns>The parameters of the merge insert query.</returns>
    public virtual IEnumerable<QueryParameter> GetParameters()
    {
        if (Destination != null)
        {
            foreach (var item in Destination.GetParameters())
            {
                yield return item;
            }
        }
        if (Datasource != null)
        {
            foreach (var item in Datasource.GetParameters())
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Retrieves the internal queries associated with the merge insert query.
    /// </summary>
    /// <returns>The internal queries associated with the merge insert query.</returns>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        if (Datasource != null)
        {
            foreach (var item in Datasource.GetInternalQueries())
            {
                yield return item;
            }
        }
        if (Destination != null)
        {
            foreach (var item in Destination.GetInternalQueries())
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Retrieves the physical tables associated with the merge insert query.
    /// </summary>
    /// <returns>The physical tables associated with the merge insert query.</returns>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        if (Datasource != null)
        {
            foreach (var item in Datasource.GetPhysicalTables())
            {
                yield return item;
            }
        }
        if (Destination != null)
        {
            foreach (var item in Destination.GetPhysicalTables())
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Retrieves the common tables associated with the merge insert query.
    /// </summary>
    /// <returns>The common tables associated with the merge insert query.</returns>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        if (Datasource != null)
        {
            foreach (var item in Datasource.GetCommonTables())
            {
                yield return item;
            }
        }
        if (Destination != null)
        {
            foreach (var item in Destination.GetCommonTables())
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Retrieves the tokens associated with the merge insert query.
    /// </summary>
    /// <param name="parent">The parent token.</param>
    /// <returns>The tokens associated with the merge insert query.</returns>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        if (Destination == null) yield break;
        if (Datasource == null) yield break;

        foreach (var item in GetInsertTokens(parent)) yield return item;
        foreach (var item in GetValuesTokens(parent)) yield return item;
    }

    private IEnumerable<Token> GetInsertTokens(Token? parent)
    {
        if (Destination == null) yield break;

        var t = Token.Reserved(this, parent, "insert");
        yield return t;
        var bracket = Token.ReservedBracketStart(this, parent);
        yield return bracket;
        foreach (var item in Destination.GetTokens(bracket)) yield return item;
        yield return Token.ReservedBracketEnd(this, parent);
    }

    private IEnumerable<Token> GetValuesTokens(Token? parent)
    {
        if (Datasource == null) yield break;

        var t = Token.Reserved(this, parent, "values");
        yield return t;
        var bracket = Token.ReservedBracketStart(this, parent);
        yield return bracket;
        foreach (var item in Datasource.GetTokens(bracket)) yield return item;
        yield return Token.ReservedBracketEnd(this, parent);
    }
}
