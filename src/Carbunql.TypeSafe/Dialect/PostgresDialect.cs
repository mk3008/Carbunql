using Carbunql.Building;

namespace Carbunql.TypeSafe.Dialect;

public class PostgresDialect : ISqlDialect
{
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
        return $"trunc({string.Join(",", args)})";//引数は1
    }

    public string GetFloorCommand(IEnumerable<string> args)
    {
        return $"floor({string.Join(",", args)})";//引数は1
    }

    public string GetCeilingCommand(IEnumerable<string> args)
    {
        return $"ceil({string.Join(",", args)})";//引数は1
    }

    public string GetRoundCommand(IEnumerable<string> args)
    {
        return $"round({string.Join(",", args)})";//引数は1、または2
    }

    public string GetCastStatement(string value, Type type)
    {
        return $"cast({value} as {this.ToDbType(type)})";
    }
}
