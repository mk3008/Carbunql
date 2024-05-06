using Carbunql.Building;
using Carbunql.Definitions;

namespace Carbunql.Annotations;

/// <summary>
/// Creates a primary key constraint for the specified type.
/// </summary>
/// <param name="type">The type for which the primary key constraint is created.</param>
/// <returns>The created primary key constraint.</returns>
public static class PrimaryKeyConstraintFactory
{
    /// <summary>
    /// Creates a primary key constraint for the specified type.
    /// </summary>
    /// <param name="type">The type for which the primary key constraint is created.</param>
    /// <returns>The created primary key constraint.</returns>
    public static PrimaryKeyConstraint Create(Type type)
    {
        var atr = (TableAttribute?)Attribute.GetCustomAttribute(type, typeof(TableAttribute));

        if (atr != null)
        {
            var columns = type.GetProperties()
                .Where(prop => atr.PrimaryKeyProperties.Contains(prop.Name))
                .Select(prop => new PrimaryKeyMap(prop.Name.ToSnakeCase().ToLower(), prop.Name)).ToList();

            return Create(type, atr.Schema, atr.Table, columns, atr.ConstraintName);
        }

        return Create(type, string.Empty, string.Empty, Array.Empty<PrimaryKeyMap>(), string.Empty);
    }

    /// <summary>
    /// Creates a primary key constraint with the specified parameters.
    /// </summary>
    /// <param name="type">The type for which the primary key constraint is created.</param>
    /// <param name="schema">The schema name.</param>
    /// <param name="table">The table name.</param>
    /// <param name="columns">The columns of the primary key.</param>
    /// <param name="constraintName">The name of the primary key constraint.</param>
    /// <returns>The created primary key constraint.</returns>
    public static PrimaryKeyConstraint Create(Type type, string schema, string table, IEnumerable<PrimaryKeyMap> columns, string constraintName)
    {
        if (string.IsNullOrEmpty(table))
        {
            schema = DbmsConfiguration.ConvertToDefaultSchemaNameLogic(type);
        }
        if (string.IsNullOrEmpty(table))
        {
            table = DbmsConfiguration.ConvertToDefaultTableNameLogic(type);
        }
        if (!columns.Any())
        {
            var map = new PrimaryKeyMap(
                    DbmsConfiguration.ConvertToDefaultPrimaryKeyColumnLogic(table),
                    DbmsConfiguration.ConvertToDefaultPrimaryKeyPropertyLogic(type)
            );
            columns = new List<PrimaryKeyMap> { map };
        }
        if (string.IsNullOrEmpty(constraintName))
        {
            constraintName = DbmsConfiguration.ConvertToDefaultPrimaryKeyConstraintNameLogic(table);
        }

        var costraint = new PrimaryKeyConstraint(schema, table, columns)
        {
            ConstraintName = constraintName,
        };

        return costraint;
    }
}
