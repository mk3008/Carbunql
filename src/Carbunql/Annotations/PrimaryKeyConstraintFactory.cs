using Carbunql.Building;
using Carbunql.Definitions;

namespace Carbunql.Annotations;

public static class PrimaryKeyConstraintFactory
{
    public static PrimaryKeyConstraint Create<T>()
    {
        var atr = (TableAttribute?)Attribute.GetCustomAttribute(typeof(T), typeof(TableAttribute));

        if (atr != null)
        {
            var columns = typeof(T).GetProperties()
                .Where(prop => atr.PrimaryKeyProperties.Contains(prop.Name))
                .Select(prop => prop.Name.ToSnakeCase().ToLower()).ToList();

            return Create<T>(atr.Schema, atr.Table, columns, atr.ConstraintName);
        }

        return Create<T>(string.Empty, string.Empty, Array.Empty<string>(), string.Empty);
    }

    public static PrimaryKeyConstraint Create<T>(string schema, string table, IEnumerable<string> columns, string constraintName)
    {
        if (string.IsNullOrEmpty(table))
        {
            schema = DbmsConfiguration.ConvertToDefaultSchemaNameLogic(typeof(T));
        }
        if (string.IsNullOrEmpty(table))
        {
            table = DbmsConfiguration.ConvertToDefaultTableNameLogic(typeof(T));
        }
        if (!columns.Any())
        {
            columns = new List<string> { DbmsConfiguration.ConvertToDefaultPrimaryKeyColumnLogic(table) };
        }
        if (string.IsNullOrEmpty(constraintName))
        {
            constraintName = DbmsConfiguration.ConvertToDefaultPrimaryKeyConstraintNameLogic(table);
        }

        var costraint = new PrimaryKeyConstraint(schema, table)
        {
            ConstraintName = constraintName,
            ColumnNames = columns.ToList()
        };

        return costraint;
    }
}
