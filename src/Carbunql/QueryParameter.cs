namespace Carbunql;

/// <summary>
/// Represents a parameter of a query.
/// </summary>
public class QueryParameter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QueryParameter"/> class with the specified parameter name and value.
    /// </summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    public QueryParameter(string parameterName, object? value)
    {
        ParameterName = parameterName;
        Value = value;
    }

    /// <summary>
    /// Gets or sets the name of the parameter.
    /// </summary>
    public string ParameterName { get; set; }

    /// <summary>
    /// Gets or sets the value of the parameter.
    /// </summary>
    public object? Value { get; set; }
}
