using Carbunql.Extensions;
using Cysharp.Text;

namespace Carbunql.Analysis;

/// <summary>
/// LexReader class extended for parsing SQL with comment handling.
/// </summary>
public class SqlLexReader : LexReader
{
    /// <summary>
    /// Constructor for the SqlLexReader class.
    /// </summary>
    /// <param name="text">The SQL text to be parsed.</param>
    public SqlLexReader(string text) : base(text)
    {
    }

    /// <summary>
    /// Collection of tokens indicating SQL comments.
    /// </summary>
    private static IEnumerable<string> CommentTokens { get; set; } = new string[] { "--", "/*" };

    /// <summary>
    /// Reads the next lexeme, skipping any SQL comments.
    /// </summary>
    /// <returns>The next lexeme after skipping any comments.</returns>
    protected override string ReadLex()
    {
        var lex = base.ReadLex();

        // Skip comment blocks
        while (lex.IsEqualNoCase(CommentTokens))
        {
            if (lex == "--")
            {
                // Line comment
                ReadUntilLineEnd();
            }
            else
            {
                // Block comment
                ReadUntilCloseBlockComment();
            }
            lex = base.ReadLex();
        }

        return lex;
    }

    /// <summary>
    /// Reads characters until the closing symbol of a block comment is found.
    /// </summary>
    /// <returns>The content of the block comment.</returns>
    private string ReadUntilCloseBlockComment()
    {
        var err = "Block comment is not closed";

        using var sb = ZString.CreateStringBuilder();

        foreach (var lex in Reads())
        {
            if (lex == null) throw new SyntaxException(err);

            sb.Append(lex);
            if (lex.IsEqualNoCase("*/"))
            {
                return sb.ToString();
            }
            if (lex.IsEqualNoCase("/*"))
            {
                sb.Append(ReadUntilCloseBlockComment());
            }
        }

        throw new SyntaxException(err);
    }
}
