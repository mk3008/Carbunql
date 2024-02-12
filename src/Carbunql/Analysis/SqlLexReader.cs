using Carbunql.Extensions;
using Cysharp.Text;

namespace Carbunql.Analysis;

/// <summary>
/// LexReader class with SQL comment disabling.
/// </summary>
public class SqlLexReader : LexReader
{
	public SqlLexReader(string text) : base(text)
	{
	}

	private static IEnumerable<string> CommentTokens { get; set; } = new string[] { "--", "/*" };

	protected override string ReadLex()
	{
		var lex = base.ReadLex();

		//skip comment block
		while (lex.IsEqualNoCase(CommentTokens))
		{
			if (lex == "--")
			{
				//line comment
				ReadUntilLineEnd();
			}
			else
			{
				//block comment
				ReadUntilCloseBlockComment();
			}
			lex = base.ReadLex();
		}

		return lex;
	}

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
