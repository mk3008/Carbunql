using Carbunql.Extensions;
using Cysharp.Text;

namespace Carbunql.Analysis;

public class LexReader : CharReader
{
	public LexReader(string text) : base(text)
	{
	}

	private IEnumerable<char> SpaceChars { get; set; } = " \r\n\t".ToArray();

	private IEnumerable<char> ForceBreakSymbols { get; set; } = ".,();[]".ToArray();

	private IEnumerable<char> BitwiseOperatorSymbols { get; set; } = "&|^#~".ToArray();

	private IEnumerable<char> ArithmeticOperatorSymbols { get; set; } = "+-*/%".ToArray();

	private IEnumerable<char> RegexOperatorSymbols { get; set; } = "!~*".ToArray();

	private IEnumerable<char> ComparisonOperatorSymbols { get; set; } = "<>!=".ToArray();

	private IEnumerable<char> PrefixSymbols { get; set; } = "?:@".ToArray();

	private IEnumerable<char> TypeConvertSymbols { get; set; } = ":".ToArray();

	private IEnumerable<char> SingleSymbols => ForceBreakSymbols.Union(BitwiseOperatorSymbols);

	private IEnumerable<char> MultipleSymbols => ArithmeticOperatorSymbols.Union(ComparisonOperatorSymbols).Union(RegexOperatorSymbols);

	private IEnumerable<char> AllSymbols => SingleSymbols.Union(MultipleSymbols).Union(PrefixSymbols).Union(SpaceChars).Union(RegexOperatorSymbols).Union(TypeConvertSymbols);

	public string ReadLex(bool skipSpace = true)
	{
		if (skipSpace) SkipSpace();

		using var sb = ZString.CreateStringBuilder();

		var fc = ReadChars().FirstOrDefault();
		if (fc == '\0') return string.Empty;

		sb.Append(fc);

		// ex. space
		if (fc == ' ')
		{
			sb.Append(ReadWhileSpace());
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
			sb.Append(ReadCharOrDefault('|'));
			return sb.ToString();
		}

		// ex. - or -- or -> or ->>
		if (fc == '-')
		{
			// --
			if (PeekOrDefaultChar() == '-')
			{
				sb.Append(ReadChar());
				return sb.ToString();
			}

			// -> or ->>
			if (PeekOrDefaultChar() == '>')
			{
				sb.Append(ReadChar());
				if (PeekOrDefaultChar() == '>')
				{
					sb.Append(ReadChar());
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
			if (PeekOrDefaultChar() == '>')
			{
				sb.Append(ReadChar());
				if (PeekOrDefaultChar() == '>')
				{
					sb.Append(ReadChar());
				}
				return sb.ToString();
			}

			// #
			return sb.ToString();
		}

		// ex. / or /*
		if (fc == '/')
		{
			sb.Append(ReadCharOrDefault('*'));
			return sb.ToString();
		}

		// ex. * or */
		if (fc == '*')
		{
			sb.Append(ReadCharOrDefault('/'));
			return sb.ToString();
		}

		// ex. . or , or (
		if (SingleSymbols.Contains(fc)) return sb.ToString();

		// ::
		if (fc == ':' && PeekOrDefaultChar() == ':')
		{
			sb.Append(ReadCharOrDefault(':'));
			return sb.ToString();
		}

		// ex. + or != 
		// ignore ::
		if ((fc != ':' && MultipleSymbols.Contains(fc)) || (fc == ':' && PeekOrDefaultChar() == ':'))
		{
			foreach (var item in ReadChars((x) => x != '/' && MultipleSymbols.Contains(x)))
			{
				sb.Append(item);
			}
			return sb.ToString();
		}

		// ex. 123.45
		if (fc.IsInteger())
		{
			foreach (var item in ReadChars((x) => "0123456789.".ToArray().Contains(x)))
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

		foreach (var item in ReadChars(whileFn))
		{
			sb.Append(item);
		}
		return sb.ToString();
	}

	public IEnumerable<string> ReadLexs(bool skipSpace = true)
	{
		var w = ReadLex(skipSpace);
		while (!string.IsNullOrEmpty(w))
		{
			yield return w;
			w = ReadLex(skipSpace);
		}
	}

	private string ReadWhileSpace()
	{
		using var sb = ZString.CreateStringBuilder();
		foreach (var item in ReadChars(x => SpaceChars.Contains(x)))
		{
			sb.Append(item);
		}
		return sb.ToString();
	}

	public void SkipSpace() => ReadWhileSpace();

	public string ReadUntilLineEnd()
	{
		using var sb = ZString.CreateStringBuilder();
		foreach (var item in ReadChars())
		{
			if (item != '\n' && item != '\r')
			{
				sb.Append(item);
				continue;
			}

			if (item == '\r')
			{
				var c = PeekOrDefaultChar();
				if (c != null && c.Value == '\n') ReadChar();
			}
			break;
		}
		return sb.ToString();
	}

	private string ReadUntilSingleQuote()
	{
		using var sb = ZString.CreateStringBuilder();
		foreach (var item in ReadChars())
		{
			sb.Append(item);
			if (item == '\'')
			{
				if (PeekOrDefaultChar() == '\'')
				{
					sb.Append(ReadChar());
					continue;
				}
				return sb.ToString();
			}
		}
		throw new SyntaxException("single quote is not closed.");
	}
}