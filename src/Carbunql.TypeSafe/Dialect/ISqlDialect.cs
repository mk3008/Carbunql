namespace Carbunql.TypeSafe.Dialect;

public interface ISqlDialect
{
    string GetPrameterSymbol();

    string ToParaemterName(string key);

    string GetNowCommand();

    string GetCurrentTimestampCommand();

    string GetCoalesceCommand(string left, string right);

    string GetModuloCommand(string left, string right);

    string GetTruncateCommand(IEnumerable<string> args);

    string GetFloorCommand(IEnumerable<string> args);

    string GetCeilingCommand(IEnumerable<string> args);

    string GetRoundCommand(IEnumerable<string> args);

    string GetCastStatement(string value, Type type);

    string BooleanDbType { get; }
    string CharDbType { get; }
    string SByteDbType { get; }
    string ByteDbType { get; }
    string Int16DbType { get; }
    string UInt16DbType { get; }
    string Int32DbType { get; }
    string UInt32DbType { get; }
    string Int64DbType { get; }
    string UInt64DbType { get; }
    string SingleDbType { get; }
    string DoubleDbType { get; }
    string DecimalDbType { get; }
    string DateTimeDbType { get; }
    string StringDbType { get; }
    string Identity16DbType { get; }
    string Identity32DbType { get; }
    string Identity64DbType { get; }
}

public static class ISqlDialectExtension
{
    public static string ToDbType(this ISqlDialect source, Type propertyType)
    {
        if (propertyType == typeof(bool) || Nullable.GetUnderlyingType(propertyType) == typeof(bool))
        {
            return source.BooleanDbType;
        }
        else if (propertyType == typeof(char) || Nullable.GetUnderlyingType(propertyType) == typeof(char))
        {
            return source.CharDbType;
        }
        else if (propertyType == typeof(sbyte) || Nullable.GetUnderlyingType(propertyType) == typeof(sbyte))
        {
            return source.SByteDbType;
        }
        else if (propertyType == typeof(byte) || Nullable.GetUnderlyingType(propertyType) == typeof(byte))
        {
            return source.ByteDbType;
        }
        else if (propertyType == typeof(short) || Nullable.GetUnderlyingType(propertyType) == typeof(short))
        {
            return source.Int16DbType;
        }
        else if (propertyType == typeof(ushort) || Nullable.GetUnderlyingType(propertyType) == typeof(ushort))
        {
            return source.UInt16DbType;
        }
        else if (propertyType == typeof(int) || Nullable.GetUnderlyingType(propertyType) == typeof(int))
        {
            return source.Int32DbType;
        }
        else if (propertyType == typeof(uint) || Nullable.GetUnderlyingType(propertyType) == typeof(uint))
        {
            return source.UInt32DbType;
        }
        else if (propertyType == typeof(long) || Nullable.GetUnderlyingType(propertyType) == typeof(long))
        {
            return source.Int64DbType;
        }
        else if (propertyType == typeof(ulong) || Nullable.GetUnderlyingType(propertyType) == typeof(ulong))
        {
            return source.UInt64DbType;
        }
        else if (propertyType == typeof(float) || Nullable.GetUnderlyingType(propertyType) == typeof(float))
        {
            return source.SingleDbType;
        }
        else if (propertyType == typeof(double) || Nullable.GetUnderlyingType(propertyType) == typeof(double))
        {
            return source.DoubleDbType;
        }
        else if (propertyType == typeof(decimal) || Nullable.GetUnderlyingType(propertyType) == typeof(decimal))
        {
            return source.DecimalDbType;
        }
        else if (propertyType == typeof(DateTime) || Nullable.GetUnderlyingType(propertyType) == typeof(DateTime))
        {
            return source.DateTimeDbType;
        }
        else if (propertyType == typeof(string) || Nullable.GetUnderlyingType(propertyType) == typeof(string))
        {
            return source.StringDbType;
        }
        else
        {
            throw new ArgumentException($"Unsupported property type :{propertyType}");
        }
    }
}
