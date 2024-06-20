using Carbunql.Building;

namespace Carbunql.TypeSafe;

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

public class PostgresDialect : ISqlDialect
{
    public string PlaceholderIdentifier { get; } = ":";
    public string CoalesceFunctionName { get; } = "coalesce";
    public string BooleanDbType { get; } = "boolean";
    public string CharDbType { get; } = "character";
    public string SByteDbType { get; } = "smallint";
    public string ByteDbType { get; } = "smallint"; // PostgreSQL does not have a type equivalent to byte, so it is mapped to smallint
    public string Int16DbType { get; } = "smallint";
    public string UInt16DbType { get; } = "integer"; // PostgreSQL does not have a type equivalent to ushort, so it is mapped to smallint
    public string Int32DbType { get; } = "integer";
    public string UInt32DbType { get; } = "bigint"; // PostgreSQL does not have a type equivalent to uint, so it is mapped to bigint
    public string Int64DbType { get; } = "bigint";
    public string UInt64DbType { get; } = "numeric"; // PostgreSQL does not have a type equivalent to ulong, so it is mapped to numeric
    public string SingleDbType { get; } = "real";
    public string DoubleDbType { get; } = "double precision";
    public string DecimalDbType { get; } = "numeric";
    public string DateTimeDbType { get; } = "timestamp";
    public string StringDbType { get; } = "text";
    public string Identity16DbType { get; } = "serial";
    public string Identity32DbType { get; } = "bigserial";
    public string Identity64DbType { get; } = "bigserial";

    public string GetPrameterSymbol()
    {
        return ":";
    }

    public string ToParaemterName(string key)
    {
        return $"{GetPrameterSymbol()}{key.ToLowerSnakeCase()}";
    }

    public string GetNowCommand()
    {
        return "now()";
    }

    public string GetCurrentTimestampCommand()
    {
        return "current_timestamp";
    }

    public string GetCoalesceCommand(string left, string right)
    {
        return $"coalesce({left}, {right})";
    }

    public string GetModuloCommand(string left, string right)
    {
        return left + " % " + right;
    }

    public string GetTruncateCommand(IEnumerable<string> args)
    {
        return $"trunc({string.Join(",", args)})";
    }

    public string GetFloorCommand(IEnumerable<string> args)
    {
        return $"floor({string.Join(",", args)})";
    }

    public string GetCeilingCommand(IEnumerable<string> args)
    {
        return $"ceil({string.Join(",", args)})";
    }

    public string GetRoundCommand(IEnumerable<string> args)
    {
        return $"round({string.Join(",", args)})";
    }

    public string GetCastStatement(string value, Type type)
    {
        return $"cast({value} as {this.ToDbType(type)})";
    }
}

public class SQLiteDialect : ISqlDialect
{
    public string PlaceholderIdentifier { get; } = "@";
    public string CoalesceFunctionName { get; } = "coalesce";
    public string BooleanDbType { get; } = "integer"; // SQLite does not have a boolean type, so it is mapped to integer
    public string CharDbType { get; } = "text";
    public string SByteDbType { get; } = "integer";
    public string ByteDbType { get; } = "integer"; // SQLite uses integer for byte
    public string Int16DbType { get; } = "integer";
    public string UInt16DbType { get; } = "integer"; // SQLite uses integer for ushort
    public string Int32DbType { get; } = "integer";
    public string UInt32DbType { get; } = "integer"; // SQLite uses integer for uint
    public string Int64DbType { get; } = "integer";
    public string UInt64DbType { get; } = "integer"; // SQLite uses integer for ulong
    public string SingleDbType { get; } = "real";
    public string DoubleDbType { get; } = "real";
    public string DecimalDbType { get; } = "real"; // SQLite uses real for decimal
    public string DateTimeDbType { get; } = "text"; // SQLite uses text for datetime
    public string StringDbType { get; } = "text";
    public string Identity16DbType { get; } = "integer"; // SQLite uses integer with AUTOINCREMENT
    public string Identity32DbType { get; } = "integer"; // SQLite uses integer with AUTOINCREMENT
    public string Identity64DbType { get; } = "integer"; // SQLite uses integer with AUTOINCREMENT

    public string GetPrameterSymbol()
    {
        return "@";
    }

    public string ToParaemterName(string key)
    {
        return $"{GetPrameterSymbol()}{key.ToLowerSnakeCase()}";
    }

    public string GetNowCommand()
    {
        return "datetime('now')";
    }

    public string GetCurrentTimestampCommand()
    {
        return "current_timestamp";
    }

    public string GetCoalesceCommand(string left, string right)
    {
        return $"coalesce({left}, {right})";
    }

    public string GetModuloCommand(string left, string right)
    {
        return left + " % " + right;
    }

    public string GetTruncateCommand(IEnumerable<string> args)
    {
        var lst = args.ToList();
        if (lst.Count == 1)
        {
            return $"cast({lst[0]} as integer)";
        }
        else if (lst.Count == 2)
        {
            // SQLite does not have a built-in TRUNC function with two arguments.
            // However, we can implement truncation by combining math operations.
            return $"cast({lst[0]} * (10 ^ {lst[1]}) as integer) / (10 ^ {lst[1]})";
        }
        else
        {
            throw new ArgumentException("Invalid number of arguments for TRUNC function");
        }
    }

    public string GetFloorCommand(IEnumerable<string> args)
    {
        return $"floor({string.Join(",", args)})";
    }

    public string GetCeilingCommand(IEnumerable<string> args)
    {
        return $"ceil({string.Join(",", args)})";
    }

    public string GetRoundCommand(IEnumerable<string> args)
    {
        return $"round({string.Join(",", args)})";
    }

    public string GetCastStatement(string value, Type type)
    {
        return $"cast({value} as {this.ToDbType(type)})";
    }
}
