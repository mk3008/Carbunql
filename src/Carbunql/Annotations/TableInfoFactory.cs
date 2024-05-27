namespace Carbunql.Annotations;

public static class TableInfoFactory
{
    public static TableInfo Create(Type type)
    {
        var atr = (TableAttribute?)Attribute.GetCustomAttribute(type, typeof(TableAttribute));

        string schema;
        string table;

        if (string.IsNullOrEmpty(atr?.Schema))
        {
            schema = DbmsConfiguration.ConvertToDefaultSchemaNameLogic(type);
        }
        else
        {
            schema = atr.Schema;
        }
        if (string.IsNullOrEmpty(atr?.Table))
        {
            table = DbmsConfiguration.ConvertToDefaultTableNameLogic(type);
        }
        else
        {
            table = atr.Table;
        }

        return new TableInfo(table) { Schema = schema };
    }
}