namespace Carbunql.Fluent;

public static class SelectQueryParameterExtensions
{
    /// <summary>
    /// Adds a parameter to the query. This method is obsolete and will be removed in a future version.
    /// Please use the <see cref="Parameter"/> method instead.
    /// </summary>
    /// <param name="query">The query to which the parameter will be added.</param>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    /// <returns>The modified query with the parameter added.</returns>
    [Obsolete("AddParameter is obsolete. Please use the 'Parameter' method instead.")]
    public static SelectQuery AddParameter(this SelectQuery query, string name, object? value)
    {
        var prm = new QueryParameter(name, value);
        query.Parameters.Add(prm);
        return query;
    }

    /// <summary>
    /// Adds a parameter to the query.
    /// </summary>
    /// <param name="query">The query to which the parameter will be added.</param>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    /// <returns>The modified query with the parameter added.</returns>
    public static SelectQuery Parameter(this SelectQuery query, string name, object? value)
    {
        var prm = new QueryParameter(name, value);
        query.Parameters.Add(prm);
        return query;
    }

    /// <summary>
    /// Adds a parameter to the query and applies a function that uses the parameter's name.
    /// </summary>
    /// <param name="query">The query to which the parameter will be added.</param>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    /// <param name="func">A function that takes the parameter's name and returns a modified query.</param>
    /// <returns>The result of applying the function to the parameter's name.</returns>
    public static SelectQuery Parameter(this SelectQuery query, string name, object? value, Func<string, SelectQuery> func)
    {
        var prm = new QueryParameter(name, value);
        query.Parameters.Add(prm);
        return func(prm.ParameterName);
    }

    /// <summary>
    /// Adds a parameter to the query and applies a function that uses both the query and the parameter's name.
    /// </summary>
    /// <param name="query">The query to which the parameter will be added.</param>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    /// <param name="func">A function that takes the query and the parameter's name, and returns a modified query.</param>
    /// <returns>The result of applying the function to the query and the parameter's name.</returns>
    public static SelectQuery Parameter(this SelectQuery query, string name, object? value, Func<SelectQuery, string, SelectQuery> func)
    {
        var prm = new QueryParameter(name, value);
        query.Parameters.Add(prm);
        return func(query, prm.ParameterName);
    }
}
