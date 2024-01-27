using Carbunql.Extensions;
using Cysharp.Text;

namespace Carbunql.Analysis;

/// <summary>
/// Class for reading text on a Lexeme (Lex) basis.
/// Reading can only be performed in the forward direction and cannot be reversed.
/// </summary>
public class LexReader : IDisposable
{
	private bool disposedValue;

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="text">The text to be read.</param>
	public LexReader(string text)
	{
		Reader = new CharReader(text);
	}

	private CharReader Reader { get; init; }

	private static IEnumerable<char> NumericSymbols { get; set; } = "0123456789.".ToArray();

	private static IEnumerable<char> SpaceChars { get; set; } = " \r\n\t".ToArray();

	private static IEnumerable<char> ForceBreakSymbols { get; set; } = ".,();[]".ToArray();

	private static IEnumerable<char> BitwiseOperatorSymbols { get; set; } = "&|^#~".ToArray();

	private static IEnumerable<char> ArithmeticOperatorSymbols { get; set; } = "+-*/%".ToArray();

	private static IEnumerable<char> RegexOperatorSymbols { get; set; } = "!~*".ToArray();

	private static IEnumerable<char> ComparisonOperatorSymbols { get; set; } = "<>!=".ToArray();

	private static IEnumerable<char> PrefixSymbols { get; set; } = "?:@".ToArray();

	private static IEnumerable<char> TypeConvertSymbols { get; set; } = ":".ToArray();

	private static IEnumerable<char> SingleSymbols => ForceBreakSymbols.Union(BitwiseOperatorSymbols);

	private static IEnumerable<char> MultipleSymbols => ArithmeticOperatorSymbols.Union(ComparisonOperatorSymbols).Union(RegexOperatorSymbols);

	private static IEnumerable<char> AllSymbols => SingleSymbols.Union(MultipleSymbols).Union(PrefixSymbols).Union(SpaceChars).Union(RegexOperatorSymbols).Union(TypeConvertSymbols);

	private string Read(bool skipSpace = true)
	{
		if (skipSpace) SkipSpace();

		using var sb = ZString.CreateStringBuilder();

		if (!Reader.TryRead(out var fc))
		{
			return string.Empty;
		};

		sb.Append(fc);

		// ex. space
		if (fc == ' ')
		{
			foreach (var item in Reader.Reads(x => SpaceChars.Contains(x)))
			{
				sb.Append(item);
			}
			return sb.ToString();
		}

		// ex. 'text'
		if (fc == '\'')
		{
			sb.Append(ReadUntilSingleQuote());
			return sb.ToString();
		}

		// ex. | or ||
		if (fc == '|')
		{
			if (Reader.TryRead('|', out var c))
			{
				sb.Append(c);
			}
			return sb.ToString();
		}

		// ex. - or -- or -> or ->>
		if (fc == '-')
		{
			// --
			if (Reader.TryRead('-', out var hyphen))
			{
				sb.Append(hyphen);
				return sb.ToString();
			}

			// -> or ->>
			if (Reader.TryRead('>', out var geaterThan))
			{
				sb.Append(geaterThan);
				if (Reader.TryRead('>', out geaterThan))
				{
					sb.Append(geaterThan);
				}
				return sb.ToString();
			}

			// -
			return sb.ToString();
		}

		// ex. # or #> or #>>
		if (fc == '#')
		{
			// #> or #>>
			if (Reader.TryRead('>', out var greaterThan))
			{
				sb.Append(greaterThan);
				if (Reader.TryRead('>', out greaterThan))
				{
					sb.Append(greaterThan);
				}
				return sb.ToString();
			}

			// #
			return sb.ToString();
		}

		// ex. / or /*
		if (fc == '/')
		{
			if (Reader.TryRead('*', out var asterisk))
			{
				sb.Append(asterisk);
			}
			return sb.ToString();
		}

		// ex. * or */
		if (fc == '*')
		{
			if (Reader.TryRead('/', out var slash))
			{
				sb.Append(slash);
			}
			return sb.ToString();
		}

		// ex. @@ (MySQL system variable)
		if (fc == '@')
		{
			if (Reader.TryRead('@', out var atmark))
			{
				sb.Append(atmark);
			}
			// continue
		}

		var x = SingleSymbols;
		// ex. . or , or (
		if (SingleSymbols.Contains(fc)) return sb.ToString();

		// ::
		if (fc == ':' && Reader.TryRead(':', out var colon))
		{
			sb.Append(colon);
			return sb.ToString();
		}

		// ex. + or != 
		if (MultipleSymbols.Contains(fc))
		{
			foreach (var item in Reader.Reads((x) => x != '/' && MultipleSymbols.Contains(x)))
			{
				sb.Append(item);
			}
			return sb.ToString();
		}

		// ex. 123.45
		if (fc.IsInteger())
		{
			foreach (var item in Reader.Reads(x => NumericSymbols.Contains(x)))
			{
				sb.Append(item);
			}
			return sb.ToString();
		}

		var whileFn = (char c) =>
		{
			if (AllSymbols.Contains(c)) return false;
			return true;
		};

		foreach (var item in Reader.Reads(whileFn))
		{
			sb.Append(item);
		}
		return sb.ToString();
	}

	/// <summary>
	/// Continuously reads until the end.
	/// </summary>
	/// <param name="skipSpace">Indicates whether to ignore spaces at the beginning of the reading. If true, spaces will be ignored.</param>
	/// <returns>The read Lexemes.</returns>
	public IEnumerable<string> Reads(bool skipSpace = true)
	{
		var w = Read(skipSpace);
		while (!string.IsNullOrEmpty(w))
		{
			yield return w;
			w = Read(skipSpace);
		}
	}

	private void SkipSpace()
	{
		foreach (var _ in Reader.Reads(x => SpaceChars.Contains(x)))
		{
		}
	}

	/// <summary>
	/// Reads until the end of the line. Primarily used for obtaining line comments.
	/// </summary>
	/// <returns>The read string.</returns>
	public string ReadUntilLineEnd()
	{
		using var sb = ZString.CreateStringBuilder();
		foreach (var item in Reader.Reads())
		{
			if (item != '\n' && item != '\r')
			{
				sb.Append(item);
				continue;
			}

			if (item == '\r')
			{
				Reader.TryRead('\n', out _);
			}
			break;
		}
		return sb.ToString();
	}

	private string ReadUntilSingleQuote()
	{
		using var sb = ZString.CreateStringBuilder();
		foreach (var item in Reader.Reads())
		{
			sb.Append(item);
			if (item == '\'')
			{
				if (Reader.TryRead('\'', out var c))
				{
					sb.Append(c);
					continue;
				}
				return sb.ToString();
			}
		}
		throw new SyntaxException("single quote is not closed.");
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				// TODO: マネージド状態を破棄します (マネージド オブジェクト)
				Reader.Dispose();
			}

			// TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
			// TODO: 大きなフィールドを null に設定します
			disposedValue = true;
		}
	}

	// // TODO: 'Dispose(bool disposing)' にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします
	// ~CharReader()
	// {
	//     // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
	//     Dispose(disposing: false);
	// }

	public void Dispose()
	{
		// このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}