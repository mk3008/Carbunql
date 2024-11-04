namespace Carbunql.LexicalAnalyzer;

public enum LexType : byte
{
    /// <summary>
    /// Unknown or unrecognized token type.
    /// </summary>
    Unknown = 0,

    CommentAfterWhiteSpaces,

    QueryTerminator,

    As,

    Operator,

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
    SchemaOrTable,
    SchemaOrTableOrColumn,
    Column,
    Function,
    Letter,

    LineCommentStart,
    BlockCommentStart,
    HitCommentStart,
    BlockCommentEnd,
    Comment,








    /// <summary>
    /// Terminator for SQL queries, represented by a semicolon (;).
    /// </summary>


    /// <summary>
    /// Separator for values, represented by a comma (,).
    /// </summary>
    ValueSeparator,

    /// <summary>
    /// Separator for identifiers, represented by a period (.).
    /// Used to separate schema, table, and column names (e.g., table.column).
    /// </summary>
    IdentifierSeparator,

    /// <summary>
    /// Left parenthesis, represented by an opening parenthesis (() used in expressions.
    /// </summary>
    LeftParen,

    /// <summary>
    /// Right parenthesis, represented by a closing parenthesis ()) used in expressions.
    /// </summary>
    RightParen,

    /// <summary>
    /// Left square bracket, represented by an opening bracket ([), often used for identifiers.
    /// </summary>
    LeftSquareBracket,

    /// <summary>
    /// Right square bracket, represented by a closing bracket (]), often used for identifiers.
    /// </summary>
    RightSquareBracket
}


