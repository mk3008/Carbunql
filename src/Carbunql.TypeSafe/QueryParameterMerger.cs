namespace Carbunql.TypeSafe;

using System;
using System.Collections.Generic;
using System.Linq;

public static class QueryParameterMerger
{
    public static List<QueryParameter> Merge(IEnumerable<QueryParameter> parameters)
    {
        // Create a dictionary ignoring case sensitivity
        var parameterDictionary = new Dictionary<string, QueryParameter>(StringComparer.OrdinalIgnoreCase);

        foreach (var param in parameters)
        {
            // Check if the parameter name already exists in the dictionary
            if (parameterDictionary.TryGetValue(param.ParameterName, out var existingParam))
            {
                // Throw an error if there is a matching parameter name but a different value
                if (!Equals(existingParam.Value, param.Value))
                {
                    throw new InvalidOperationException($"Parameter '{param.ParameterName}' already exists with a different value.");
                }
                // Do nothing if both the name and value match (ignore duplicates)
            }
            else
            {
                // Add the new parameter to the dictionary
                parameterDictionary[param.ParameterName] = param;
            }
        }

        // Create and return a list from the updated dictionary
        return parameterDictionary.Values.ToList();
    }
}
