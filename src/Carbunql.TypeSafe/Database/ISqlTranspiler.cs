namespace Carbunql.TypeSafe.Dialect;

/// <summary>
/// Interface for defining SQL dialect-specific operations and types.
/// </summary>
public interface ISqlTranspiler
{
    /// <summary>
    /// Gets the symbol used for parameters in the SQL dialect.
    /// </summary>
    /// <returns>The parameter symbol.</returns>
    string GetPrameterSymbol();

    /// <summary>
    /// Converts a key to a parameter name specific to the SQL dialect.
    /// </summary>
    /// <param name="key">The key to be converted.</param>
    /// <returns>The parameter name.</returns>
    string ToParaemterName(string key);

    /// <summary>
    /// Gets the command to retrieve the current date and time.
    /// </summary>
    /// <returns>The command to get the current date and time.</returns>
    string GetNowCommand();

    /// <summary>
    /// Gets the command to retrieve the current timestamp.
    /// </summary>
    /// <returns>The command to get the current timestamp.</returns>
    string GetCurrentTimestampCommand();

    /// <summary>
    /// Gets the SQL coalesce command, which returns the first non-null value.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>The coalesce command.</returns>
    string GetCoalesceCommand(string left, string right);

    /// <summary>
    /// Gets the SQL modulo command.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>The modulo command.</returns>
    string GetModuloCommand(string left, string right);

    /// <summary>
    /// Gets the SQL truncate command, which truncates a number to an integer.
    /// </summary>
    /// <param name="args">The arguments for the truncate command.</param>
    /// <returns>The truncate command.</returns>
    string GetTruncateCommand(IEnumerable<string> args);

    /// <summary>
    /// Gets the SQL floor command, which returns the largest integer less than or equal to a specified number.
    /// </summary>
    /// <param name="args">The arguments for the floor command.</param>
    /// <returns>The floor command.</returns>
    string GetFloorCommand(IEnumerable<string> args);

    /// <summary>
    /// Gets the SQL ceiling command, which returns the smallest integer greater than or equal to a specified number.
    /// </summary>
    /// <param name="args">The arguments for the ceiling command.</param>
    /// <returns>The ceiling command.</returns>
    string GetCeilingCommand(IEnumerable<string> args);

    /// <summary>
    /// Gets the SQL round command, which rounds a number to the nearest integer.
    /// </summary>
    /// <param name="args">The arguments for the round command.</param>
    /// <returns>The round command.</returns>
    string GetRoundCommand(IEnumerable<string> args);

    /// <summary>
    /// Gets the SQL cast statement, which converts a value to a specified type.
    /// </summary>
    /// <param name="value">The value to be converted.</param>
    /// <param name="type">The target type.</param>
    /// <returns>The cast statement.</returns>
    string GetCastStatement(string value, Type type);

    /// <summary>
    /// Gets the database type for boolean values.
    /// </summary>
    string BooleanDbType { get; }

    /// <summary>
    /// Gets the database type for char values.
    /// </summary>
    string CharDbType { get; }

    /// <summary>
    /// Gets the database type for signed byte values.
    /// </summary>
    string SByteDbType { get; }

    /// <summary>
    /// Gets the database type for byte values.
    /// </summary>
    string ByteDbType { get; }

    /// <summary>
    /// Gets the database type for 16-bit integer values.
    /// </summary>
    string Int16DbType { get; }

    /// <summary>
    /// Gets the database type for unsigned 16-bit integer values.
    /// </summary>
    string UInt16DbType { get; }

    /// <summary>
    /// Gets the database type for 32-bit integer values.
    /// </summary>
    string Int32DbType { get; }

    /// <summary>
    /// Gets the database type for unsigned 32-bit integer values.
    /// </summary>
    string UInt32DbType { get; }

    /// <summary>
    /// Gets the database type for 64-bit integer values.
    /// </summary>
    string Int64DbType { get; }

    /// <summary>
    /// Gets the database type for unsigned 64-bit integer values.
    /// </summary>
    string UInt64DbType { get; }

    /// <summary>
    /// Gets the database type for single-precision floating-point values.
    /// </summary>
    string SingleDbType { get; }

    /// <summary>
    /// Gets the database type for double-precision floating-point values.
    /// </summary>
    string DoubleDbType { get; }

    /// <summary>
    /// Gets the database type for decimal values.
    /// </summary>
    string DecimalDbType { get; }

    /// <summary>
    /// Gets the database type for DateTime values.
    /// </summary>
    string DateTimeDbType { get; }

    /// <summary>
    /// Gets the database type for string values.
    /// </summary>
    string StringDbType { get; }

    /// <summary>
    /// Gets the database type for identity columns with 16-bit integer values.
    /// </summary>
    string Identity16DbType { get; }

    /// <summary>
    /// Gets the database type for identity columns with 32-bit integer values.
    /// </summary>
    string Identity32DbType { get; }

    /// <summary>
    /// Gets the database type for identity columns with 64-bit integer values.
    /// </summary>
    string Identity64DbType { get; }
}


public static class ISqlDialectExtension
{
    public static string ToDbType(this ISqlTranspiler source, Type propertyType)
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
