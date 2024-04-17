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
    }
}