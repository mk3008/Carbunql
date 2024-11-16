using System.Diagnostics.CodeAnalysis;

namespace Carbunql.LexicalAnalyzer;

public static partial class Lexer
{
    private static HashSet<string> SingleWordDbTypes = new HashSet<string>
    {
        // 日付・時間型
        "date",
        "time",
        "datetime",        // MySQL, SQL Server
        //"timestamp",       // PostgreSQL, SQL Server
        "datetime2",       // SQL Server
        "interval",        // PostgreSQL

        // 数値型
        "int",
        "bigint",
        "smallint",
        "tinyint",         // MySQL
        "serial",          // PostgreSQL
        "decimal",
        "numeric",
        "float",
        "real",            // MySQL, SQL Server
        //"double",
        "binary_float",    // Oracle
        "binary_double",   // Oracle
        "money",           // PostgreSQL, SQL Server

        // 文字型
        "text",
        "varchar",
        "char",
        "nvarchar",        // SQL Server
        "nchar",           // SQL Server
        "longtext",        // MySQL
        "clob",            // Oracle, SQL Server
        //"character",       // PostgreSQL

        // バイナリ型
        "blob",            // SQLite, MySQL
        "longblob",        // MySQL
        "binary",          // SQL Server
        "bytea",           // PostgreSQL

        // その他
        "boolean",
        "json",            // PostgreSQL, MySQL, SQL Server
        "uuid",            // PostgreSQL, SQL Server, MySQL
        "xml",             // SQL Server, Oracle
        "xmltype",         // Oracle
        "enum",            // MySQL
        "set"              // MySQL
    };

    [MemberNotNullWhen(true)]
    public static bool TryParseDbType(ReadOnlyMemory<char> memory, int position, out Lex lex, out int endPosition)
    {
        lex = default;
        var start = position;
        endPosition = start;

        if (!TryGetCharacterEndPosition(memory, position, out var pos))
        {
            return false;
        }

        var length = pos - start;

        // timestamp
        if (length == 9 && memory.EqualsWordIgnoreCase(position, "timestamp", out position))
        {
            memory.SkipWhiteSpacesAndComment(ref position);

            if (memory.EqualsWordIgnoreCase(position, "with", out position))
            {
                memory.SkipWhiteSpacesAndComment(ref position);
                if (!memory.EqualsWordIgnoreCase(position, "time", out position))
                {
                    throw new FormatException();
                }

                memory.SkipWhiteSpacesAndComment(ref position);
                if (!memory.EqualsWordIgnoreCase(position, "zone", out position))
                {
                    throw new FormatException();
                }

                lex = new Lex(memory, LexType.Type, start, position - start, "timestamp with time zone");
            }
            else if (memory.EqualsWordIgnoreCase(position, "without", out position))
            {
                memory.SkipWhiteSpacesAndComment(ref position);
                if (!memory.EqualsWordIgnoreCase(position, "time", out position))
                {
                    throw new FormatException();
                }

                memory.SkipWhiteSpacesAndComment(ref position);
                if (!memory.EqualsWordIgnoreCase(position, "zone", out position))
                {
                    throw new FormatException();
                }

                lex = new Lex(memory, LexType.Type, start, position - start, "timestamp without time zone");
            }
            else
            {
                lex = new Lex(memory, LexType.Type, start, position - start, "timestamp");
            }
            endPosition = position;
            return true;
        }

        // character
        if (length == 9 && memory.EqualsWordIgnoreCase(position, "character", out position))
        {
            memory.SkipWhiteSpacesAndComment(ref position);

            if (memory.EqualsWordIgnoreCase(position, "varying", out position))
            {
                lex = new Lex(memory, LexType.Type, start, position - start, "character varying");
            }
            else
            {
                lex = new Lex(memory, LexType.Type, start, position - start, "character");
            }
            endPosition = position;
            return true;
        }

        // double
        if (length == 6 && memory.EqualsWordIgnoreCase(position, "double", out position))
        {
            memory.SkipWhiteSpacesAndComment(ref position);

            if (memory.EqualsWordIgnoreCase(position, "precision", out position))
            {
                lex = new Lex(memory, LexType.Type, start, position - start, "double precision");
            }
            else
            {
                lex = new Lex(memory, LexType.Type, start, position - start, "double");
            }
            endPosition = position;
            return true;
        }

        // other
        foreach (var keyword in SingleWordDbTypes.Where(x => x.Length == length))
        {
            if (memory.EqualsWordIgnoreCase(position, keyword, out position))
            {
                lex = new Lex(memory, LexType.Type, start, position - start);
                endPosition = position;
                return true;
            }
        }
        return false;
    }
}
