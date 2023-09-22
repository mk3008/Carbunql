using Carbunql.Analysis.Parser;
using Carbunql.Tables;
using Carbunql.Values;
using Cysharp.Text;

namespace Carbunql.Extensions;

public static class StringExtension
{
	public static bool StartsWith(this string source, IEnumerable<string> prefixes)
	{
		foreach (var item in prefixes)
		{
			if (source.StartsWith(item)) return true;
		}
		return false;
	}

	public static bool IsEqualNoCase(this string? source, string? text)
	{
		if (source == null && text == null) return true;
		if (source == null) return false;
		return string.Equals(source, text, StringComparison.CurrentCultureIgnoreCase);
	}

	public static bool IsEqualNoCase(this string? source, IEnumerable<string?> texts)
	{
		return texts.Where(x => source.IsEqualNoCase(x)).Any();
	}

	public static bool IsEqualNoCase(this string? source, Predicate<string?> fn)
	{
		return fn(source);
	}

	public static bool IsNumeric(this string source)
	{
		if (string.IsNullOrEmpty(source)) return false;
		return source.First().IsInteger();
	}

	public static string InsertIndent(this string source, string separator = "\r\n", int spaceCount = 4)
	{
		if (string.IsNullOrEmpty(source)) return source;

		var indent = spaceCount.ToSpaceString();

		return indent + source.Replace(separator, $"{separator}{indent}");
	}

	public static string Join(this string source, string jointext, IEnumerable<string>? items, string splitter = ",", string? startDecorate = null, string? endDecorate = null)
	{
		if (items == null || !items.Any()) return source;

		using var sb = ZString.CreateStringBuilder();
		sb.Append(source + jointext);
		sb.Append(items.ToString(splitter, startDecorate, endDecorate));
		return sb.ToString();
	}

	public static string ToString(this IEnumerable<string> source, string splitter = ",", string? startDecorate = null, string? endDecorate = null)
	{
		if (!source.Any()) return string.Empty;

		using var sb = ZString.CreateStringBuilder();
		var isFirst = true;

		if (startDecorate != null) sb.Append(startDecorate);
		foreach (var item in source)
		{
			if (isFirst)
			{
				isFirst = false;
			}
			else
			{
				sb.Append(splitter);
			}
			sb.Append(item);
		}
		if (endDecorate != null) sb.Append(endDecorate);

		return sb.ToString();
	}

	public static PhysicalTable ToPhysicalTable(this string source)
	{
		return new PhysicalTable(source);
	}

	public static ValueCollection ToValueCollection(this IEnumerable<string> source)
	{
		var v = new ValueCollection();
		foreach (var item in source) v.Add(ValueParser.Parse(item));
		return v;
	}
}