using Cysharp.Text;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Analysis;

/// <summary>
/// Class for reading text on a Lexeme (Lex) basis.
/// Reading can only be performed in the forward direction and cannot be reversed.
/// </summary>
public abstract class LexReader : IDisposable
{
	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="text">The text to be read.</param>
	public LexReader(string text)
	{
		Text = text;
		TextLength = Text.Length;
	}

	private int Index { get; set; } = 0;

	private string Text { get; init; }

	private int TextLength { get; init; }

	//private bool IsEndOfText => TextLength <= Index;

	public bool TryPeek(char expect)
	{
		if (TryPeekChar(0, out var c) && c == expect)
		{
			return true;
		}
		return false;
	}

	private bool TryPeekChar([MaybeNullWhen(false)] out char c)
	{
		return TryPeekChar(0, out c);
	}

	private bool TryPeekChar(int shift, [MaybeNullWhen(false)] out char c)
	{
		c = default;
		var s = Peek(shift, 1);
		if (!string.IsNullOrEmpty(s))
		{
			c = s.First();
			return true;
		}
		return false;
	}

	private string Peek(int shift, int length)
	{
		if (TextLength == 0) return string.Empty;

		var len = Math.Min(TextLength - Index - shift, length);
		if (len <= 0) return string.Empty;

		return Text.Substring(Index + shift, len);
	}

	private string Peek(int length)
	{
		return Peek(0, length);
	}

	/// <summary>
	/// Reads characters using the CharReader and retrieves Lexemes.
	/// Since this function directly reads the text, peeking is not possible.
	/// It is recommended to obtain Lexemes through the Peek or Read functions.
	/// </summary>
	/// <returns>The retrieved Lexeme.</returns>
	protected virtual string ReadLex()
	{
		SkipSpace();

		if (!TryPeekChar(out var fc))
		{
			return string.Empty;
		}

		using var sb = ZString.CreateStringBuilder();

		// ex. 'text'
		if (fc == '\'')
		{
			sb.Append(Read(1));
			sb.Append(ReadUntilSingleQuote());
			return sb.ToString();
		}

		// ex. | or ||
		if (fc == '|')
		{
			if (TryRead("||", out var c))
			{
				sb.Append(c);
				return sb.ToString();
			}
			sb.Append(Read(1));
			return sb.ToString();
		}

		// ex. - or -- or -> or ->>
		if (fc == '-')
		{
			var lex = string.Empty;

			if (TryRead("->>", out lex))
			{
				sb.Append(lex);
				return sb.ToString();
			}

			if (TryRead("->", out lex))
			{
				sb.Append(lex);
				return sb.ToString();
			}


			if (TryRead("--", out lex))
			{
				sb.Append(lex);
				return sb.ToString();
			}

			sb.Append(Read(1));
			return sb.ToString();
		}

		// ex. # or #> or #>>
		if (fc == '#')
		{
			var lex = string.Empty;

			if (TryRead("#>>", out lex))
			{
				sb.Append(lex);
				return sb.ToString();
			}

			if (TryRead("#>", out lex))
			{
				sb.Append(lex);
				return sb.ToString();
			}

			sb.Append(Read(1));
			return sb.ToString();
		}

		// ex. / or /*
		if (fc == '/')
		{
			var lex = string.Empty;
			if (TryRead("/*", out lex))
			{
				sb.Append(lex);
				return sb.ToString();
			}

			sb.Append(Read(1));
			return sb.ToString();
		}

		// ex. * or */
		if (fc == '*')
		{
			var lex = string.Empty;
			if (TryRead("*/", out lex))
			{
				sb.Append(lex);
				return sb.ToString();
			}

			sb.Append(Read(1));
			return sb.ToString();
		}

		// ex. :: (Postgres cast symbol)
		if (fc == ':')
		{
			var lex = string.Empty;
			if (TryRead("::", out lex))
			{
				sb.Append(lex);
				return sb.ToString();
			}
			// continue
		}

		// ForceBreakSymbols
		if (".,();[]".Contains(fc))
		{
			sb.Append(Read(1));
			return sb.ToString();
		}

		// BitwiseOperatorSymbols
		if ("&|^#~".Contains(fc))
		{
			sb.Append(Read(1));
			return sb.ToString();
		}

		// ex. 123.45
		if ("0123456789".Contains(fc))
		{
			return ReadWhile("0123456789.");
		}

		// operator
		if ("+-%<>!=".Contains(fc))
		{
			return ReadWhile("+-*/%<>!=?:@.&|^#~");
		}

		// ex. @@ (MySQL system variable prefix)
		if (fc == '@')
		{
			var lex = string.Empty;
			if (TryRead("@@", out lex))
			{
				sb.Append(lex);
			}
			else
			{
				sb.Append(Read(1));
			}
		}
		else
		{
			sb.Append(Read(1));
		}
		sb.Append(ReadNotWhile("+-*/%<>!=?:@.&|^#~ \r\n\t.,();[]|"));

		return sb.ToString();
	}

	/// <summary>
	/// Method to read a single Lexeme.
	/// </summary>
	/// <returns>The read Lexeme.</returns>
	private string Read(int length)
	{
		var s = Peek(length);
		Index += length;
		return s;
	}

	/// <summary>
	/// Method to read a Lexeme, but an exception will be thrown if it does not match the expected value.
	/// </summary>
	/// <param name="expect">The expected Lexeme.</param>
	/// <returns>The read Lexeme.</returns>
	/// <exception cref="Exception">Thrown when the actual Lexeme does not match the expected value.</exception>
	public string Read(string expect)
	{
		SkipSpace();

		var length = expect.Length;
		var actual = Peek(length);

		if (TryMatch(actual, expect, out var lex))
		{
			Index += actual.Length;
			return lex;
		}

		throw new Exception($"expect : '{expect}', actual : '{actual.TrimEnd()}'");
	}

	/// <summary>
	/// Method to read a Lexeme. However, an exception is thrown if the read Lexeme does not match any of the expected values in the list.
	/// </summary>
	/// <param name="expects">List of expected Lexemes.</param>
	/// <returns>The read Lexeme.</returns>
	/// <exception cref="Exception">Thrown when the actual Lexeme does not match any of the expected values.</exception>
	public string Read(IEnumerable<string> expects)
	{
		SkipSpace();

		var length = expects.Max(x => x.Length);
		var actual = Peek(length);

		if (TryMatch(actual, expects, out var lex))
		{
			Index += lex.Length;
			return lex;
		}

		//debug
		throw new Exception($"expect : '{string.Join(",", expects)}', actual : '{actual.TrimEnd()}'");
	}

	/// <summary>
	/// Attempts to read the specified Lexeme if it matches the expected value.
	/// </summary>
	/// <param name="expect">The expected Lexeme.</param>
	/// <param name="lex">The read Lexeme.</param>
	/// <returns>True if a match is found, false otherwise.</returns>
	public bool TryRead(string expect, [MaybeNullWhen(false)] out string lex)
	{
		return TryRead(new[] { expect }, out lex);
	}

	/// <summary>
	/// Attempts to read a Lexeme if it matches any of the expected values in the provided list.
	/// </summary>
	/// <param name="expects">List of expected Lexemes.</param>
	/// <param name="lex">The read Lexeme.</param>
	/// <returns>True if a match is found, false otherwise.</returns>
	public bool TryRead(IEnumerable<string> expects, [MaybeNullWhen(false)] out string lex)
	{
		lex = default;

		SkipSpace();

		var length = expects.Max(x => x.Length);
		var actual = Peek(length);

		if (TryMatch(actual, expects, out lex))
		{
			Index += lex.Length;
			return true;
		}
		return false;
	}

	public string Read() => Reads().First();

	/// <summary>
	/// Method to read Lexemes. Enumerates the read Lexemes.
	/// </summary>
	/// <returns>Enumerable of the read Lexemes.</returns>
	public IEnumerable<string> Reads()
	{
		var w = ReadLex();
		if (string.IsNullOrEmpty(w))
		{
			yield return string.Empty;
			yield break;
		}
		while (!string.IsNullOrEmpty(w))
		{
			yield return w;
			w = ReadLex();
		}
	}

	private void SkipSpace()
	{
		var s = Peek(1);
		while ((s == " " || s == "\r" || s == "\n" || s == "\t"))
		{
			Index++;
			s = Peek(1);
		}
	}

	/// <summary>
	/// Reads until the end of the line. Primarily used for obtaining line comments.
	/// </summary>
	/// <returns>The read string.</returns>
	protected string ReadUntilLineEnd()
	{
		var shift = 0;

		var s = Peek(shift, 1);
		while (!(s == "\r" || s == "\n" || string.IsNullOrEmpty(s)))
		{
			shift++;
			s = Peek(shift, 1);
		}

		var lex = Read(shift);
		Index++;

		if (s == "\n")
		{
			Index++;
		}

		return lex;
	}

	private string ReadUntilSingleQuote()
	{
		var shift = 0;

		var s = Peek(shift, 1);
		while (!string.IsNullOrEmpty(s))
		{
			if (s != "'")
			{
				shift++;
				s = Peek(shift, 1);
				continue;
			}

			if (s == "'" && Peek(shift + 1, 1) == "'")
			{
				shift += 2;
				s = Peek(shift, 1);
				continue;
			}

			break;
		}

		if (s != "'") throw new SyntaxException("single quote is not closed.");

		shift++;
		return Read(shift);
	}

	private string ReadWhile(string charArrayString)
	{
		var shift = 0;

		var s = Peek(shift, 1);
		while (!string.IsNullOrEmpty(s) && charArrayString.Contains(s.First()))
		{
			shift++;
			s = Peek(shift, 1);
		}

		if (shift == 0) return string.Empty;
		return Read(shift);
	}

	private string ReadNotWhile(string charArrayString)
	{
		var shift = 0;

		var s = Peek(shift, 1);
		while (!string.IsNullOrEmpty(s) && !charArrayString.Contains(s.First()))
		{
			shift++;
			s = Peek(shift, 1);
		}

		if (shift == 0) return string.Empty;
		return Read(shift);
	}

	private bool TryMatch(string actualLex, IEnumerable<string> expectLexs, [MaybeNullWhen(false)] out string lex)
	{
		lex = default;

		foreach (var expectLex in expectLexs)
		{
			if (TryMatch(actualLex, expectLex, out lex))
			{
				return true;
			}
		}
		return false;
	}

	private bool TryMatch(string actualLex, string expectLex, [MaybeNullWhen(false)] out string lex)
	{
		lex = default;

		var specials = new[] { "--", "/*", "::", "@@" };
		var symbols = " \r\r\n\t.,();[]'";
		var numbers = "0123456789";

		if (actualLex.StartsWith(expectLex, StringComparison.OrdinalIgnoreCase))
		{
			//NOTE
			//It is necessary to evaluate whether the string simply obtained
			//without passing through ReadLex is a lexeme.

			//Adopt if it exactly matches a special lexeme
			if (specials.Contains(actualLex))
			{
				lex = expectLex;
				return true;
			}

			//Verify next occurrence of character
			var c = Peek(expectLex.Length, 1);
			if (string.IsNullOrEmpty(c) || symbols.Contains(c) || numbers.Contains(c))
			{
				lex = expectLex;
				return true;
			}

			//build error message
			var s = ReadLex();
			throw new SyntaxException($"expect : '{expectLex}', actual : '{s}'");
		}
		return false;
	}

	public void Dispose()
	{
		//((IDisposable)Reader).Dispose();
	}
}