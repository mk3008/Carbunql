namespace Carbunql.LexicalAnalyzer;

public enum LexType : byte
{
    /// <summary>
    /// Unknown or unrecognized token type.
    /// </summary>
    Unknown = 0,

    CommentAfterWhiteSpaces,

    QueryTerminator = 1,

    //alter command
    AlterSchema,
    AlterTable,
    AlterView,
    AlterIndex,
    AlterFunction,
    AlterProcedure,
    AlterTrigger,
    AlterSequence,

    //create command
    CreateSchema,
    CreateTable,
    CreateTemporaryTable,
    CreateView,
    CreateIndex,
    CreateUniqueIndex,
    CreateFunction,
    CreateTrigger,
    CreateProcedure,
    CreateSequence,

    //insert, update, delete, merge command
    Insert,
    Update,
    Delete,
    Merge,


    Select,
    SelectDistinct,
    SelectDistinctOn,

    With,
    WithRecursive,

    PrefixNegation, // not, ~

    WildCard,
    Value,
    SchemaOrTableOrColumn,
    Column,
    Function,
    Letter,

    LineCommentStart = 1,
    BlockCommentStart = 2,
    HitCommentStart = 4,
    BlockCommentEnd = 3,
    Comment = 6,








    /// <summary>
    /// Terminator for SQL queries, represented by a semicolon (;).
    /// </summary>


    /// <summary>
    /// Separator for values, represented by a comma (,).
    /// </summary>
    ValueSeparator = 2,

    /// <summary>
    /// Separator for identifiers, represented by a period (.).
    /// Used to separate schema, table, and column names (e.g., table.column).
    /// </summary>
    IdentifierSeparator = 3,

    /// <summary>
    /// Left parenthesis, represented by an opening parenthesis (() used in expressions.
    /// </summary>
    LeftParen = 4,

    /// <summary>
    /// Right parenthesis, represented by a closing parenthesis ()) used in expressions.
    /// </summary>
    RightParen = 5,

    /// <summary>
    /// Left square bracket, represented by an opening bracket ([), often used for identifiers.
    /// </summary>
    LeftSquareBracket = 6,

    /// <summary>
    /// Right square bracket, represented by a closing bracket (]), often used for identifiers.
    /// </summary>
    RightSquareBracket = 7
}


