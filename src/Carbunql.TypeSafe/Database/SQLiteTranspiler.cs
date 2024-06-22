using Carbunql.Building;

namespace Carbunql.TypeSafe.Dialect;

public class SQLiteTranspiler : ISqlTranspiler
{
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
