//using System.Diagnostics.CodeAnalysis;

//namespace Carbunql.LexicalAnalyzer;


//public static partial class TokenParser
//{
//    private static readonly HashSet<string> SpecialValueKeywords = new HashSet<string>
//    {
//        "true",
//        "false",
//        "null",
//    };

//    [MemberNotNullWhen(returnValue: true)]
//    public static bool TryParseSpecialValueToken(ReadOnlyMemory<char> memory, ref int pos, ref int length, ref int parenthesisLevel, ref int bracketLevel, ref SqlToken tempToken, out SqlToken token)
//    {
//        token = default;
//        var start = pos;
//        var identifier = tempToken.FormattedValue;

//        if (!SpecialValueKeywords.Contains(identifier))
//        {
//            return false;
//        }

//        pos = tempToken.EndPosition;
//        token = new SqlToken(TokenType.Value, memory.Slice(start, pos - start), start, pos, parenthesisLevel, bracketLevel, identifier);
//        return true;
//    }
//}

//public static partial class TokenParser
//{
//    private static readonly HashSet<string> AlterationKeywords = new HashSet<string>
//    {
//        "table",
//        "view",
//        "index",
//        "schema",
//        "function",
//        "procedure",
//        "trigger",
//        "sequence"
//    };

//    [MemberNotNullWhen(returnValue: true)]
//    public static bool TryParseAlterIdentifierToken(ReadOnlyMemory<char> memory, ref int pos, ref int length, ref int parenthesisLevel, ref int bracketLevel, ref SqlToken tempToken, out SqlToken token)
//    {
//        token = default;
//        var start = pos;
//        var identifier = tempToken.FormattedValue;

//        if (identifier != "alter")
//        {
//            return false;
//        }

//        pos = tempToken.EndPosition;
//        var nextToken = Lexer.Tokenize(memory, pos, length, parenthesisLevel, bracketLevel).First();
//        var nextIdentifier = nextToken.FormattedValue.ToLowerInvariant();

//        if (!AlterationKeywords.Contains(nextIdentifier))
//        {
//            throw new FormatException($"Invalid syntax after 'alter': expected one of {string.Join(", ", AlterationKeywords)}.");
//        }

//        pos = nextToken.EndPosition;
//        token = new SqlToken(TokenType.Keyword, memory.Slice(start, pos - start), start, pos, parenthesisLevel, bracketLevel, "alter " + nextIdentifier);
//        return true;
//    }

//    private static readonly HashSet<string> CreationKeywords = new HashSet<string>
//    {
//        "table",
//        "view",
//        "index",
//        "schema",
//        "function",
//        "procedure",
//        "trigger",
//        "sequence"
//    };

//    [MemberNotNullWhen(returnValue: true)]
//    public static bool TryParseCreateIdentifierToken(ReadOnlyMemory<char> memory, ref int pos, ref int length, ref int parenthesisLevel, ref int bracketLevel, ref SqlToken tempToken, out SqlToken token)
//    {
//        token = default;
//        var start = pos;
//        var identifier = tempToken.FormattedValue;

//        if (identifier != "create")
//        {
//            return false;
//        }

//        pos = tempToken.EndPosition;
//        var nextToken = Lexer.Tokenize(memory, pos, length, parenthesisLevel, bracketLevel).First();
//        var nextIdentifier = nextToken.FormattedValue.ToLowerInvariant();

//        if (nextIdentifier == "or")
//        {
//            return HandleCreateOrReplace(memory, ref pos, ref length, ref parenthesisLevel, ref bracketLevel, ref nextToken, ref start, out token);
//        }

//        if (nextIdentifier == "unique")
//        {
//            return HandleCreateUniqueIndex(memory, ref pos, length, parenthesisLevel, ref bracketLevel, ref nextToken, ref start, out token);
//        }

//        if (!CreationKeywords.Contains(nextIdentifier))
//        {
//            throw new FormatException($"Invalid syntax after 'create': expected a valid keyword, but got '{nextIdentifier}'.");
//        }

//        pos = nextToken.EndPosition;
//        token = new SqlToken(TokenType.Keyword, memory.Slice(start, pos - start), start, pos, parenthesisLevel, bracketLevel, "create " + nextIdentifier);
//        return true;
//    }

//    private static bool HandleCreateOrReplace(ReadOnlyMemory<char> memory, ref int pos, ref int length, ref int parenthesisLevel, ref int bracketLevel, ref SqlToken nextToken, ref int start, out SqlToken token)
//    {
//        pos = nextToken.EndPosition;
//        nextToken = Lexer.Tokenize(memory, pos, length, parenthesisLevel, bracketLevel).First();
//        var nextIdentifier = nextToken.FormattedValue.ToLowerInvariant();

//        if (nextIdentifier == "replace")
//        {
//            pos = nextToken.EndPosition;
//            nextToken = Lexer.Tokenize(memory, pos, length, parenthesisLevel, bracketLevel).First();
//            nextIdentifier = nextToken.FormattedValue.ToLowerInvariant();

//            if (CreationKeywords.Contains(nextIdentifier))
//            {
//                pos = nextToken.EndPosition;
//                token = new SqlToken(TokenType.Keyword, memory.Slice(start, pos - start), start, pos, parenthesisLevel, bracketLevel, "create or replace " + nextIdentifier);
//                return true;
//            }
//        }

//        throw new FormatException($"Invalid syntax after 'create or': expected 'replace' or a valid keyword, but got '{nextIdentifier}'.");
//    }

//    private static bool HandleCreateUniqueIndex(ReadOnlyMemory<char> memory, ref int pos, ref int length, ref int parenthesisLevel, ref int bracketLevel, ref SqlToken nextToken, ref int start, out SqlToken token)
//    {
//        pos = nextToken.EndPosition;
//        nextToken = Lexer.Tokenize(memory, pos, length, parenthesisLevel, bracketLevel).First();
//        var nextIdentifier = nextToken.FormattedValue.ToLowerInvariant();

//        if (nextIdentifier == "index")
//        {
//            pos = nextToken.EndPosition;
//            token = new SqlToken(TokenType.Keyword, memory.Slice(start, pos - start), start, pos, parenthesisLevel, bracketLevel, "create unique index");
//            return true;
//        }

//        throw new FormatException($"Invalid syntax after 'create unique': expected 'index', but got '{nextIdentifier}'.");
//    }

//    private static readonly HashSet<string> DropKeywords = new HashSet<string>
//    {
//        "table",
//        "view",
//        "index",
//        "schema",
//        "function",
//        "procedure",
//        "trigger",
//        "sequence"
//    };

//    [MemberNotNullWhen(returnValue: true)]
//    public static bool TryParseDropIdentifierToken(ReadOnlyMemory<char> memory, ref int pos, ref int length, int parenthesisLevel, ref int bracketLevel, SqlToken tempToken, out SqlToken token)
//    {
//        token = default;
//        var start = pos;
//        var identifier = tempToken.FormattedValue;

//        if (identifier != "drop")
//        {
//            return false;
//        }

//        pos = tempToken.EndPosition;
//        var nextToken = Lexer.Tokenize(memory, pos, length, parenthesisLevel, bracketLevel).First();
//        var nextIdentifier = nextToken.FormattedValue.ToLowerInvariant();

//        if (nextIdentifier == "if")
//        {
//            pos = nextToken.EndPosition;
//            nextToken = Lexer.Tokenize(memory, pos, length, parenthesisLevel, bracketLevel).First();
//            nextIdentifier = nextToken.FormattedValue.ToLowerInvariant();

//            if (nextIdentifier == "exists")
//            {
//                pos = nextToken.EndPosition;
//                nextToken = Lexer.Tokenize(memory, pos, length, parenthesisLevel, bracketLevel).First();
//                nextIdentifier = nextToken.FormattedValue.ToLowerInvariant();

//                if (DropKeywords.Contains(nextIdentifier))
//                {
//                    pos = nextToken.EndPosition;
//                    token = new SqlToken(TokenType.Keyword, memory.Slice(start, pos - start), start, pos, parenthesisLevel, bracketLevel, "drop if exists " + nextIdentifier);
//                    return true;
//                }
//            }

//            throw new FormatException($"Invalid syntax after 'drop if': expected 'exists' and a valid keyword, but got '{nextIdentifier}'.");
//        }

//        if (!DropKeywords.Contains(nextIdentifier))
//        {
//            throw new FormatException($"Invalid syntax after 'drop': expected a valid keyword, but got '{nextIdentifier}'.");
//        }

//        pos = nextToken.EndPosition;
//        token = new SqlToken(TokenType.Keyword, memory.Slice(start, pos - start), start, pos, parenthesisLevel, bracketLevel, "drop " + nextIdentifier);
//        return true;
//    }

//    private static readonly HashSet<string> ByKeywords = new HashSet<string>
//    {
//        "group",
//        "order",
//        "partition"
//    };

//    [MemberNotNullWhen(returnValue: true)]
//    public static bool TryParseByKeywordToken(ReadOnlyMemory<char> memory, ref int pos, ref int length, ref int parenthesisLevel, ref int bracketLevel, ref SqlToken tempToken, out SqlToken token)
//    {
//        token = default;
//        var start = pos;
//        var identifier = tempToken.FormattedValue;

//        if (!ByKeywords.Contains(identifier))
//        {
//            return false;
//        }

//        pos = tempToken.EndPosition;
//        var nextToken = Lexer.Tokenize(memory, pos, length, parenthesisLevel, bracketLevel).First();
//        var nextIdentifier = nextToken.FormattedValue;

//        if (nextIdentifier != "by")
//        {
//            throw new FormatException($"Expected 'by', but got '{nextIdentifier}'.");
//        }

//        pos = nextToken.EndPosition;
//        token = new SqlToken(TokenType.Keyword, memory.Slice(start, pos - start), start, pos, parenthesisLevel, bracketLevel, identifier + " " + nextIdentifier);
//        return true;
//    }

//    private static readonly HashSet<string> KeyKeywords = new HashSet<string>
//    {
//        "primary",
//        "foreign"
//    };

//    [MemberNotNullWhen(returnValue: true)]
//    public static bool TryParseKeyKeywordToken(ReadOnlyMemory<char> memory, ref int pos, ref int length, ref int parenthesisLevel, ref int bracketLevel, ref SqlToken tempToken, out SqlToken token)
//    {
//        token = default;
//        var start = pos;
//        var identifier = tempToken.FormattedValue;

//        if (!KeyKeywords.Contains(identifier))
//        {
//            return false;
//        }

//        pos = tempToken.EndPosition;
//        var nextToken = Lexer.Tokenize(memory, pos, length, parenthesisLevel, bracketLevel).First();
//        var nextIdentifier = nextToken.FormattedValue;

//        if (nextIdentifier != "key")
//        {
//            throw new FormatException($"Expected 'key', but got '{nextIdentifier}'.");
//        }

//        pos = nextToken.EndPosition;
//        token = new SqlToken(TokenType.Keyword, memory.Slice(start, pos - start), start, pos, parenthesisLevel, bracketLevel, identifier + " " + nextIdentifier);
//        return true;
//    }

//    private static readonly HashSet<string> NullsSortKeywords = new HashSet<string>
//    {
//        "first",
//        "last"
//    };

//    [MemberNotNullWhen(returnValue: true)]
//    public static bool TryParseNullsSortKeywordToken(ReadOnlyMemory<char> memory, ref int pos, ref int length, ref int parenthesisLevel, ref int bracketLevel, ref SqlToken tempToken, out SqlToken token)
//    {
//        token = default;
//        var start = pos;
//        var identifier = tempToken.FormattedValue;

//        if (identifier == "nulls")
//        {
//            return false;
//        }

//        pos = tempToken.EndPosition;
//        var nextToken = Lexer.Tokenize(memory, pos, length, parenthesisLevel, bracketLevel).First();
//        var nextIdentifier = nextToken.FormattedValue;

//        if (!NullsSortKeywords.Contains(nextIdentifier))
//        {
//            throw new FormatException($"Expected 'key', but got '{nextIdentifier}'.");
//        }

//        pos = nextToken.EndPosition;
//        token = new SqlToken(TokenType.Keyword, memory.Slice(start, pos - start), start, pos, parenthesisLevel, bracketLevel, identifier + " " + nextIdentifier);
//        return true;
//    }

//    private static readonly HashSet<string> JoinKeywords = new HashSet<string>
//    {
//        "inner",
//        "cross"
//    };

//    [MemberNotNullWhen(returnValue: true)]
//    public static bool TryParseJoinKeywordToken(ReadOnlyMemory<char> memory, ref int pos, ref int length, ref int parenthesisLevel, ref int bracketLevel, ref SqlToken tempToken, out SqlToken token)
//    {
//        token = default;
//        var start = pos;
//        var identifier = tempToken.FormattedValue;

//        if (!OuterJoinKeywords.Contains(identifier))
//        {
//            return false;
//        }

//        pos = tempToken.EndPosition;
//        var nextToken = Lexer.Tokenize(memory, pos, length, parenthesisLevel, bracketLevel).First();
//        var nextIdentifier = nextToken.FormattedValue;

//        if (nextIdentifier != "join")
//        {
//            throw new FormatException($"Expected 'join', but got '{nextIdentifier}'.");
//        }

//        pos = nextToken.EndPosition;
//        token = new SqlToken(TokenType.Keyword, memory.Slice(start, pos - start), start, pos, parenthesisLevel, bracketLevel, identifier + " " + nextIdentifier);
//        return true;
//    }

//    private static readonly HashSet<string> OuterJoinKeywords = new HashSet<string>
//    {
//        "left",
//        "right",
//    };

//    [MemberNotNullWhen(returnValue: true)]
//    public static bool TryParseOuterJoinKeywordToken(ReadOnlyMemory<char> memory, ref int pos, ref int length, ref int parenthesisLevel, ref int bracketLevel, ref SqlToken tempToken, out SqlToken token)
//    {
//        token = default;
//        var start = pos;
//        var identifier = tempToken.FormattedValue;

//        if (!OuterJoinKeywords.Contains(identifier))
//        {
//            return false;
//        }

//        pos = tempToken.EndPosition;
//        var nextToken = Lexer.Tokenize(memory, pos, length, parenthesisLevel, bracketLevel).First();
//        var nextIdentifier = nextToken.FormattedValue;

//        if (nextIdentifier == "outer")
//        {
//            // Throw away the 'outer' keyword
//            pos = nextToken.EndPosition;
//            nextToken = Lexer.Tokenize(memory, pos, length, parenthesisLevel, bracketLevel).First();
//            nextIdentifier = nextToken.FormattedValue;
//        }

//        if (nextIdentifier != "join")
//        {
//            throw new FormatException($"Expected 'join', but got '{nextIdentifier}'.");
//        }

//        pos = nextToken.EndPosition;
//        token = new SqlToken(TokenType.Keyword, memory.Slice(start, pos - start), start, pos, parenthesisLevel, bracketLevel, identifier + " " + nextIdentifier);
//        return true;
//    }
//}

//public static partial class TokenParser
//{
//    private static readonly HashSet<string> TextOperators = new HashSet<string>
//    {
//        "and", "or", "is", "not"
//    };

//    [MemberNotNullWhen(returnValue: true)]
//    public static bool TryParseTextOperatorToken(ReadOnlyMemory<char> memory, int pos, int length, int parenthesisLevel, ref int bracketLevel, ref SqlToken tempToken, out SqlToken token)
//    {
//        var sql = memory.Span;
//        var start = pos;
//        token = default;
//        var identifier = tempToken.FormattedValue;

//        if (identifier == "is")
//        {
//            pos = tempToken.EndPosition;
//            var nextToken = Lexer.Tokenize(memory, pos, length, parenthesisLevel, bracketLevel).First();
//            var nextIdentifier = nextToken.FormattedValue.ToLowerInvariant();

//            if (nextIdentifier == "not")
//            {
//                pos = nextToken.EndPosition;
//                token = new SqlToken(TokenType.Operator, memory.Slice(start, pos - start), start, pos, parenthesisLevel, bracketLevel, "is not");
//                return true;
//            }

//            token = new SqlToken(TokenType.Operator, memory.Slice(start, pos - start), start, pos, parenthesisLevel, bracketLevel, "is");
//            return true;
//        }

//        // and, or, not
//        if (TextOperators.Contains(identifier))
//        {
//            token = new SqlToken(TokenType.Operator, memory.Slice(start, pos - start), start, pos, parenthesisLevel, bracketLevel);
//            return true;
//        }

//        return false;
//    }

//    private static readonly HashSet<char> SymbolOperators = new HashSet<char>
//    {
//        '+', '-', '*', '/', '%', '~', '#', '=', '>', '<', '!', '&', '|', '^'
//    };

//    [MemberNotNullWhen(returnValue: true)]
//    public static bool TryParseSymbolOperatorToken(ReadOnlyMemory<char> memory, ref int pos, ref int length, ref int parenthesisLevel, ref int bracketLevel, out SqlToken token)
//    {
//        var start = pos;
//        var sql = memory.Span;
//        token = default;

//        while (pos < length && SymbolOperators.Contains(sql[pos]))
//        {
//            pos++;
//        }

//        if (start == pos)
//        {
//            return false;
//        }

//        token = new SqlToken(TokenType.Operator, memory.Slice(start, pos - start), start, pos, parenthesisLevel, bracketLevel);
//        return true;
//    }
//}

//public static partial class TokenParser
//{
//    [MemberNotNullWhen(returnValue: true)]
//    public static bool TryParseHintCommentToken(ReadOnlyMemory<char> memory, ref int pos, ref int length, ref int parenthesisLevel, ref int bracketLevel, out SqlToken startToken, out SqlToken commentToken, out SqlToken endToken)
//    {
//        int start = pos;
//        var sql = memory.Span;

//        startToken = default;
//        commentToken = default;
//        endToken = default;

//        // /*+
//        if (pos + 2 < length && sql[pos] == '/' && sql[pos + 1] == '*' && sql[pos + 2] == '+')
//        {
//            pos += 3;
//            startToken = new SqlToken(TokenType.HintCommentStart, memory.Slice(start, pos - start), start, pos, parenthesisLevel, bracketLevel, "/*+");
//            start = pos;
//        }
//        else
//        {
//            return false;
//        }

//        while (pos + 1 < length)
//        {
//            if (sql[pos] == '*' && sql[pos + 1] == '/')
//            {
//                commentToken = new SqlToken(TokenType.Comment, memory.Slice(start, pos - start), start, pos, parenthesisLevel, bracketLevel);
//                start = pos;
//                pos += 2;
//                endToken = new SqlToken(TokenType.HintCommentEnd, memory.Slice(start, pos - start), start, pos, parenthesisLevel, bracketLevel, "*/");
//                return true;
//            }
//            pos++;
//        }

//        throw new FormatException("Hint comment not closed with '*/'.");
//    }

//    [MemberNotNullWhen(returnValue: true)]
//    public static bool TryParseBlockCommentToken(ReadOnlyMemory<char> memory, ref int pos, ref int length, ref int parenthesisLevel, ref int bracketLevel, out SqlToken startToken, out SqlToken commentToken, out SqlToken endToken)
//    {
//        int start = pos;
//        var sql = memory.Span;

//        startToken = default;
//        commentToken = default;
//        endToken = default;

//        // /*
//        if (pos + 1 < length && sql[pos] == '/' && sql[pos + 1] == '*')
//        {
//            pos += 2;
//            startToken = new SqlToken(TokenType.BlockCommentStart, memory.Slice(start, pos - start), start, pos, parenthesisLevel, bracketLevel, "/*");
//            start = pos;
//        }
//        else
//        {
//            return false;
//        }

//        while (pos + 1 < length)
//        {
//            if (sql[pos] == '*' && sql[pos + 1] == '/')
//            {
//                commentToken = new SqlToken(TokenType.Comment, memory.Slice(start, pos - start), start, pos, parenthesisLevel, bracketLevel);
//                start = pos;
//                pos += 2;
//                endToken = new SqlToken(TokenType.BlockCommentEnd, memory.Slice(start, pos - start), start, pos, parenthesisLevel, bracketLevel, "*/");
//                return true;
//            }
//            pos++;
//        }

//        throw new FormatException("Block comment not closed with '*/'.");
//    }

//    private static readonly HashSet<char> LineTerminals = new HashSet<char> {
//        '\r', '\n'
//    };

//    [MemberNotNullWhen(returnValue: true)]
//    public static bool TryParseLineCommentToken(ReadOnlyMemory<char> memory, int pos, int length, int parenthesisLevel, ref int bracketLevel, out SqlToken token, out SqlToken comment)
//    {
//        int start = pos;
//        var sql = memory.Span;
//        token = default;
//        comment = default;

//        if (pos + 1 < length && sql[pos] == '-' && sql[pos + 1] == '-')
//        {
//            pos += 2;
//            token = new SqlToken(TokenType.LineCommentStart, memory.Slice(start, pos - start), start, pos, parenthesisLevel, bracketLevel, "--");
//            start = pos;
//        }
//        else
//        {
//            return false;
//        }

//        while (pos < length)
//        {
//            char current = sql[pos];

//            if (!LineTerminals.Contains(current))
//            {
//                break;
//            }
//            pos++;
//        }

//        token = new SqlToken(TokenType.Comment, memory.Slice(start, pos - start), start, pos, parenthesisLevel, bracketLevel);
//        return true;
//    }
//}

//public static partial class TokenParser
//{
//    [MemberNotNullWhen(returnValue: true)]
//    public static bool TryParseNumericToken(ReadOnlyMemory<char> memory, int pos, int length, int parenthesisLevel, ref int bracketLevel, out SqlToken token)
//    {
//        token = default;
//        int start = pos;
//        bool hasDecimalPoint = false;

//        var sql = memory.Span;

//        if (pos >= length || (!char.IsDigit(sql[pos]) && sql[pos] != '.'))
//        {
//            return false;
//        }

//        while (pos < length)
//        {
//            char current = sql[pos];

//            if (char.IsDigit(current))
//            {
//                pos++;
//            }
//            else if (current == '.' && !hasDecimalPoint)
//            {
//                hasDecimalPoint = true;
//                pos++;
//            }
//            else
//            {
//                break;
//            }
//        }

//        token = new SqlToken(TokenType.Identifier, memory.Slice(start, pos - start), start, pos, parenthesisLevel, bracketLevel);
//        return true;
//    }

//    [MemberNotNullWhen(returnValue: true)]
//    public static bool TryParseSingleQuoteIdentifierToken(ReadOnlyMemory<char> memory, int pos, int length, int parenthesisLevel, ref int bracketLevel, out SqlToken token)
//    {
//        int start = pos;
//        var sql = memory.Span;
//        token = default;

//        if (pos < length && sql[pos] == '\'')
//        {
//            pos++;

//            while (pos < length)
//            {
//                char current = sql[pos];

//                if (sql[pos - 1] != '\\' && current == '\'')
//                {
//                    token = new SqlToken(TokenType.Value, memory.Slice(start, pos - start), start, pos, parenthesisLevel, bracketLevel);
//                    return true;
//                }

//                pos++;
//            }

//            throw new FormatException("Single quote identifier not closed.");
//        }

//        return false;
//    }

//    [MemberNotNullWhen(returnValue: true)]
//    public static bool TryParseDoubleQuoteIdentifierToken(ReadOnlyMemory<char> memory, int pos, int length, int parenthesisLevel, ref int bracketLevel, out SqlToken token)
//    {
//        int start = pos;
//        var sql = memory.Span;
//        token = default;

//        if (pos < length && sql[pos] == '"')
//        {
//            while (pos < length)
//            {
//                char current = sql[pos];

//                if (current == '"')
//                {
//                    // escape check
//                    if (pos + 1 < length && sql[pos + 1] == '"')
//                    {
//                        //skip escape char and escaped double quote
//                        pos += 2;
//                        continue;
//                    }

//                    token = new SqlToken(TokenType.Identifier, memory.Slice(start, pos - start), start, pos, parenthesisLevel, bracketLevel);
//                    return true;
//                }
//                pos++;
//            }

//            throw new FormatException("Double quote identifier not closed.");
//        }

//        return false;
//    }
//}

//public static partial class TokenParser
//{
//    [MemberNotNullWhen(returnValue: true)]
//    public static bool TryParseIntervalTypeToken(ReadOnlyMemory<char> memory, ref int pos, int length, int parenthesisLevel, ref int bracketLevel, string lowercaseIdentifier, out SqlToken token)
//    {
//        token = default;

//        // Check if the identifier is 'interval'
//        if (lowercaseIdentifier != "interval")
//        {
//            return false;
//        }

//        var start = pos;
//        pos += lowercaseIdentifier.Length;

//        // Read the interval value (e.g., '1 day', '2 hours')
//        var intervalValue = ReadLowercaseLetterIdentifier(memory, pos, length);

//        // Expecting a valid format for interval (string literal)
//        if (!intervalValue.StartsWith("'") || !intervalValue.EndsWith("'"))
//        {
//            throw new FormatException("Invalid interval format. Expecting a string literal enclosed in single quotes.");
//        }

//        pos += intervalValue.Length;

//        // Create the token for the INTERVAL type
//        token = new SqlToken(TokenType.Identifier, memory.Slice(start, pos - start), start, pos, parenthesisLevel, bracketLevel, "interval " + intervalValue);
//        return true;
//    }

//    private static readonly HashSet<string> DbTypesOfDate = new HashSet<string>
//    {
//        "time",
//        "timestamp",
//        "datetime",
//        "datetime2", // SQL Server
//        "smalldatetime", // SQL Server
//        "date",
//    };

//    [MemberNotNullWhen(returnValue: true)]
//    public static bool TryParseDateTimeTypeToken(ReadOnlyMemory<char> memory, int pos, int length, int parenthesisLevel, int bracketLevel, string lowercaseIdentifier, out SqlToken token)
//    {
//        var start = pos;
//        pos += lowercaseIdentifier.Length;
//        token = default;

//        // Validate the specific date/time identifiers
//        if (!DbTypesOfDate.Contains(lowercaseIdentifier))
//        {
//            return false;
//        }

//        // Read the next identifier
//        var nextIdentifier = ReadLowercaseLetterIdentifier(memory, pos, length);
//        pos += nextIdentifier.Length;

//        // Handle (time | timestamp) with time zone or without time zone
//        if (nextIdentifier == "with" || nextIdentifier == "without")
//        {
//            nextIdentifier = ReadLowercaseLetterIdentifier(memory, pos, length);
//            pos += nextIdentifier.Length;

//            if (nextIdentifier != "time")
//            {
//                throw new FormatException($"Invalid time token: expected 'time' after '{lowercaseIdentifier}' with or without.");
//            }

//            nextIdentifier = ReadLowercaseLetterIdentifier(memory, pos, length);
//            pos += nextIdentifier.Length;

//            if (nextIdentifier != "zone")
//            {
//                throw new FormatException($"Invalid time token: expected 'zone' after 'time'.");
//            }
//        }

//        // Check for optional parentheses (e.g., time(100) or timestamp(6))
//        var nextToken = Lexer.Tokenize(memory, pos, length, parenthesisLevel, bracketLevel).First();

//        if (nextToken.Type == TokenType.LeftParen)
//        {
//            pos = GetClosingParenthesisPosition(memory, ref pos, ref length, ref parenthesisLevel, ref bracketLevel);
//        }

//        // Create the DbType token
//        token = new SqlToken(TokenType.Identifier, memory.Slice(start, pos - start), start, pos, parenthesisLevel, bracketLevel);
//        return true;
//    }
//}

//public static partial class TokenParser
//{
//    internal static string ReadLowercaseLetterIdentifier(ReadOnlyMemory<char> memory, int pos, int length)
//    {
//        var sql = memory.Span;
//        var start = pos;

//        if (pos >= length || !char.IsLetter(sql[pos]))
//        {
//            throw new InvalidOperationException("Invalid identifier: the first character must be a letter (A-Z, a-z).");
//        }

//        while (pos < length && (char.IsLetterOrDigit(sql[pos]) || sql[pos] == '_'))
//        {
//            pos++;
//        }

//        if (start == pos)
//        {
//            throw new InvalidOperationException("Invalid identifier: no valid characters found after the initial letter.");
//        }

//        return memory.Slice(start, pos - start).ToString().ToLowerInvariant();
//    }

//    private static int GetClosingParenthesisPosition(ReadOnlyMemory<char> memory, ref int pos, ref int length, ref int parenthesisLevel, ref int bracketLevel)
//    {
//        int currentLevel = parenthesisLevel - 1;
//        var nextToken = Lexer.Tokenize(memory, pos, length, parenthesisLevel, bracketLevel).First();

//        do
//        {
//            pos = nextToken.EndPosition;
//            nextToken = Lexer.Tokenize(memory, pos, length, parenthesisLevel, bracketLevel).First();
//        } while (currentLevel == nextToken.ParenthesisLevel && pos < length);

//        // Ensure matched parentheses
//        if (currentLevel != nextToken.ParenthesisLevel)
//        {
//            throw new ArgumentException("Unmatched parentheses in type definition.");
//        }

//        return pos;
//    }

//    private static int GetClosingBracketPosition(ReadOnlyMemory<char> memory, ref int pos, ref int length, ref int parenthesisLevel, ref int bracketLevel)
//    {
//        int currentLevel = bracketLevel - 1;
//        var nextToken = Lexer.Tokenize(memory, pos, length, parenthesisLevel, bracketLevel).First();

//        do
//        {
//            pos = nextToken.EndPosition;
//            nextToken = Lexer.Tokenize(memory, pos, length, parenthesisLevel, bracketLevel).First();
//        } while (currentLevel == nextToken.BracketLevel && pos < length);

//        if (currentLevel != nextToken.BracketLevel)
//        {
//            throw new ArgumentException("Unmatched brackets in type definition.");
//        }

//        return pos;
//    }
//}