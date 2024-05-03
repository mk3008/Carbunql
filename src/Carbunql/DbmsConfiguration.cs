using Carbunql.Building;

namespace Carbunql;

public static class DbmsConfiguration
{
    public static string PlaceholderIdentifier { get; set; } = ":";
    public static string CoalesceFunctionName { get; set; } = "coalesce";
    public static string BooleanDbType { get; set; } = "boolean";
    public static string CharDbType { get; set; } = "character";
    public static string SByteDbType { get; set; } = "smallint";
    public static string ByteDbType { get; set; } = "smallint"; // PostgreSQL does not have a type equivalent to byte, so it is mapped to smallint
    public static string Int16DbType { get; set; } = "smallint";
    public static string UInt16DbType { get; set; } = "integer"; // PostgreSQL does not have a type equivalent to ushort, so it is mapped to smallint
    public static string Int32DbType { get; set; } = "integer";
    public static string UInt32DbType { get; set; } = "bigint"; // PostgreSQL does not have a type equivalent to uint, so it is mapped to bigint
    public static string Int64DbType { get; set; } = "bigint";
    public static string UInt64DbType { get; set; } = "numeric"; // PostgreSQL does not have a type equivalent to ulong, so it is mapped to numeric
    public static string SingleDbType { get; set; } = "real";
    public static string DoubleDbType { get; set; } = "double precision";
    public static string DecimalDbType { get; set; } = "numeric";
    public static string DateTimeDbType { get; set; } = "timestamp";
    public static string StringDbType { get; set; } = "text";

    public static string Identity16DbType { get; set; } = "serial";
    public static string Identity32DbType { get; set; } = "bigserial";
    public static string Identity64DbType { get; set; } = "bigserial";

    /// <summary>
    /// Sets the configuration values for Oracle database.
    /// </summary>
    public static void SetOracleEnvironment()
    {
        PlaceholderIdentifier = ":"; // Set to Oracle placeholder
        CoalesceFunctionName = "NVL"; // Oracle equivalent of COALESCE function
        BooleanDbType = "NUMBER(1)"; // Oracle boolean type
        CharDbType = "CHAR"; // Oracle character type
        SByteDbType = "NUMBER(3)"; // Oracle equivalent for smallint
        ByteDbType = "NUMBER(3)"; // Mapped to smallint if no direct equivalent for byte in Oracle
        Int16DbType = "NUMBER(5)"; // Oracle equivalent for smallint
        UInt16DbType = "NUMBER(10)"; // Mapped to integer if no direct equivalent for ushort in Oracle
        Int32DbType = "NUMBER(10)"; // Oracle integer type
        UInt32DbType = "NUMBER(19)"; // Mapped to bigint if no direct equivalent for uint in Oracle
        Int64DbType = "NUMBER(19)"; // Oracle bigint type
        UInt64DbType = "NUMBER"; // Mapped to numeric if no direct equivalent for ulong in Oracle
        SingleDbType = "FLOAT"; // Oracle float type
        DoubleDbType = "DOUBLE PRECISION"; // Oracle double precision type
        DecimalDbType = "NUMBER"; // Oracle numeric type
        DateTimeDbType = "TIMESTAMP"; // Oracle timestamp type
        StringDbType = "VARCHAR2"; // Oracle string type

        Identity16DbType = "NUMBER(5)"; // Oracle equivalent for serial
        Identity32DbType = "NUMBER(10)"; // Oracle equivalent for bigserial
        Identity64DbType = "NUMBER(19)"; // Oracle equivalent for bigserial
    }

    /// <summary>
    /// Sets the configuration values for SQL Server database.
    /// </summary>
    public static void SetSqlServerEnvironment()
    {
        PlaceholderIdentifier = "@"; // Set to SQL Server placeholder
        CoalesceFunctionName = "COALESCE"; // SQL Server equivalent of COALESCE function
        BooleanDbType = "BIT"; // SQL Server boolean type
        CharDbType = "CHAR"; // SQL Server character type
        SByteDbType = "SMALLINT"; // SQL Server smallint type
        ByteDbType = "TINYINT"; // SQL Server tinyint type
        Int16DbType = "SMALLINT"; // SQL Server smallint type
        UInt16DbType = "INT"; // Mapped to int if no direct equivalent for ushort in SQL Server
        Int32DbType = "INT"; // SQL Server int type
        UInt32DbType = "BIGINT"; // Mapped to bigint if no direct equivalent for uint in SQL Server
        Int64DbType = "BIGINT"; // SQL Server bigint type
        UInt64DbType = "NUMERIC(20,0)"; // Mapped to numeric if no direct equivalent for ulong in SQL Server
        SingleDbType = "REAL"; // SQL Server real type
        DoubleDbType = "FLOAT"; // SQL Server float type
        DecimalDbType = "NUMERIC"; // SQL Server numeric type
        DateTimeDbType = "DATETIME"; // SQL Server datetime type
        StringDbType = "NVARCHAR(MAX)"; // SQL Server string type

        Identity16DbType = "SMALLINT"; // SQL Server equivalent for serial
        Identity32DbType = "INT"; // SQL Server equivalent for bigserial
        Identity64DbType = "BIGINT"; // SQL Server equivalent for bigserial
    }

    /// <summary>
    /// Sets the configuration values for MySQL database.
    /// </summary>
    public static void SetMySqlEnvironment()
    {
        PlaceholderIdentifier = "?"; // Set to MySQL placeholder
        CoalesceFunctionName = "COALESCE"; // MySQL equivalent of COALESCE function
        BooleanDbType = "TINYINT(1)"; // MySQL boolean type
        CharDbType = "CHAR"; // MySQL character type
        SByteDbType = "TINYINT"; // MySQL tinyint type
        ByteDbType = "TINYINT UNSIGNED"; // MySQL tinyint unsigned type
        Int16DbType = "SMALLINT"; // MySQL smallint type
        UInt16DbType = "INT UNSIGNED"; // MySQL int unsigned type
        Int32DbType = "INT"; // MySQL int type
        UInt32DbType = "BIGINT UNSIGNED"; // MySQL bigint unsigned type
        Int64DbType = "BIGINT"; // MySQL bigint type
        UInt64DbType = "DECIMAL(20,0)"; // MySQL decimal type
        SingleDbType = "FLOAT"; // MySQL float type
        DoubleDbType = "DOUBLE"; // MySQL double type
        DecimalDbType = "DECIMAL"; // MySQL decimal type
        DateTimeDbType = "DATETIME"; // MySQL datetime type
        StringDbType = "TEXT"; // MySQL text type

        Identity16DbType = "SMALLINT"; // MySQL equivalent for serial
        Identity32DbType = "INT"; // MySQL equivalent for bigserial
        Identity64DbType = "BIGINT"; // MySQL equivalent for bigserial
    }

    /// <summary>
    /// Sets the configuration values for SQLite database.
    /// </summary>
    public static void SetSqliteEnvironment()
    {
        PlaceholderIdentifier = "@"; // Set to SQLite placeholder
        CoalesceFunctionName = "COALESCE"; // SQLite equivalent of COALESCE function
        BooleanDbType = "INTEGER"; // SQLite boolean type
        CharDbType = "TEXT"; // SQLite text type
        SByteDbType = "INTEGER"; // SQLite integer type
        ByteDbType = "INTEGER"; // SQLite integer type
        Int16DbType = "INTEGER"; // SQLite integer type
        UInt16DbType = "INTEGER"; // SQLite integer type
        Int32DbType = "INTEGER"; // SQLite integer type
        UInt32DbType = "INTEGER"; // SQLite integer type
        Int64DbType = "INTEGER"; // SQLite integer type
        UInt64DbType = "INTEGER"; // SQLite integer type
        SingleDbType = "REAL"; // SQLite real type
        DoubleDbType = "REAL"; // SQLite real type
        DecimalDbType = "NUMERIC"; // SQLite numeric type
        DateTimeDbType = "TEXT"; // SQLite text type for storing datetime
        StringDbType = "TEXT"; // SQLite text type

        Identity16DbType = "INTEGER"; // SQLite equivalent for serial
        Identity32DbType = "INTEGER"; // SQLite equivalent for bigserial
        Identity64DbType = "INTEGER"; // SQLite equivalent for bigserial
    }

    public static string ToDbType(Type propertyType)
    {
        if (propertyType == typeof(bool) || Nullable.GetUnderlyingType(propertyType) == typeof(bool))
        {
            return BooleanDbType;
        }
        else if (propertyType == typeof(char) || Nullable.GetUnderlyingType(propertyType) == typeof(char))
        {
            return CharDbType;
        }
        else if (propertyType == typeof(sbyte) || Nullable.GetUnderlyingType(propertyType) == typeof(sbyte))
        {
            return SByteDbType;
        }
        else if (propertyType == typeof(byte) || Nullable.GetUnderlyingType(propertyType) == typeof(byte))
        {
            return ByteDbType;
        }
        else if (propertyType == typeof(short) || Nullable.GetUnderlyingType(propertyType) == typeof(short))
        {
            return Int16DbType;
        }
        else if (propertyType == typeof(ushort) || Nullable.GetUnderlyingType(propertyType) == typeof(ushort))
        {
            return UInt16DbType;
        }
        else if (propertyType == typeof(int) || Nullable.GetUnderlyingType(propertyType) == typeof(int))
        {
            return Int32DbType;
        }
        else if (propertyType == typeof(uint) || Nullable.GetUnderlyingType(propertyType) == typeof(uint))
        {
            return UInt32DbType;
        }
        else if (propertyType == typeof(long) || Nullable.GetUnderlyingType(propertyType) == typeof(long))
        {
            return Int64DbType;
        }
        else if (propertyType == typeof(ulong) || Nullable.GetUnderlyingType(propertyType) == typeof(ulong))
        {
            return UInt64DbType;
        }
        else if (propertyType == typeof(float) || Nullable.GetUnderlyingType(propertyType) == typeof(float))
        {
            return SingleDbType;
        }
        else if (propertyType == typeof(double) || Nullable.GetUnderlyingType(propertyType) == typeof(double))
        {
            return DoubleDbType;
        }
        else if (propertyType == typeof(decimal) || Nullable.GetUnderlyingType(propertyType) == typeof(decimal))
        {
            return DecimalDbType;
        }
        else if (propertyType == typeof(DateTime) || Nullable.GetUnderlyingType(propertyType) == typeof(DateTime))
        {
            return DateTimeDbType;
        }
        else if (propertyType == typeof(string) || Nullable.GetUnderlyingType(propertyType) == typeof(string))
        {
            return StringDbType;
        }
        else
        {
            throw new ArgumentException("Unsupported property type");
        }
    }

    public static string ToIdentityDbType(Type propertyType)
    {
        if (propertyType == typeof(sbyte) || propertyType == typeof(byte) || propertyType == typeof(short))
        {
            return Identity16DbType;
        }
        else if (propertyType == typeof(int) || propertyType == typeof(uint))
        {
            return Identity32DbType;
        }
        else if (propertyType == typeof(long) || propertyType == typeof(ulong))
        {
            return Identity64DbType;
        }
        else
        {
            throw new ArgumentException("Unsupported property type for identity column");
        }
    }

    private static bool IsPrimaryKeyColumn(string table, string column)
    {
        var pk = ConvertToDefaultPrimaryKeyColumnLogic(table);

        if (string.Equals(pk, column, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }
        return false;
    }

    public static Func<string, string, bool> IsPrimaryKeyColumnLogic { get; set; } = IsPrimaryKeyColumn;

    private static string GetDefaultAutoNumberDefinition()
    {
        /*
            Postgres :(empty)
            MySQL    :AUTO_INCREMENT 
            SQLServer:IDENTITY(1,1)
            Oracle   :GENERATED BY DEFAULT AS IDENTITY
        */
        return string.Empty;
    }

    public static Func<string> GetDefaultAutoNumberDefinitionLogic { get; set; } = GetDefaultAutoNumberDefinition;

    private static string ConvertToDefaultColumnName(string propertyName)
    {
        return propertyName.ToSnakeCase().ToLower();
    }

    public static Func<string, string> ConvertToDefaultColumnNameLogic { get; set; } = ConvertToDefaultColumnName;

    private static string ConvertToDefaultTableName(Type classType)
    {
        return classType.Name.ToSnakeCase().ToLower();
    }

    public static Func<Type, string> ConvertToDefaultTableNameLogic { get; set; } = ConvertToDefaultTableName;

    private static string ConvertToDefaultSchemaName(Type classType)
    {
        return string.Empty;
    }

    public static Func<Type, string> ConvertToDefaultSchemaNameLogic { get; set; } = ConvertToDefaultSchemaName;

    private static string ConvertToDefaultPrimaryKeyColumn(string table)
    {
        return table + "_id";
    }

    public static Func<string, string> ConvertToDefaultPrimaryKeyColumnLogic { get; set; } = ConvertToDefaultPrimaryKeyColumn;

    private static string ConvertToDefaultPrimaryKeyConstraintName(string table)
    {
        return table + "_pkey";
    }

    public static Func<string, string> ConvertToDefaultPrimaryKeyConstraintNameLogic { get; set; } = ConvertToDefaultPrimaryKeyConstraintName;
}