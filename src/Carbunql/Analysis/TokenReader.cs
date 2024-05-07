using Carbunql.Extensions;
using Cysharp.Text;

namespace Carbunql.Analysis;

/// <summary>
/// Abstract class for reading tokens.
/// </summary>
public abstract class TokenReader
{
    /// <summary>
    /// Initializes a new instance of the TokenReader class with the specified text.
    /// </summary>
    /// <param name="text">The text to be read.</param>
    public TokenReader(string text)
    {
        Reader = new SqlLexReader(text);
    }

    private static IEnumerable<string> JoinLexs { get; set; } = new string[] { "inner", "cross" };

    private static IEnumerable<string> ByLexs { get; set; } = new string[] { "group", "partition", "order" };

    private static IEnumerable<string> OuterJoinLexs { get; set; } = new string[] { "left", "right" };

    private static IEnumerable<string> NullsSortTokens { get; set; } = new string[] { "first", "last" };

    private static IEnumerable<string> KeyLexs { get; set; } = new string[] { "primary", "foreign" };

    private SqlLexReader Reader { get; set; }

    /// <summary>
    /// Gets or sets the current bracket level.
    /// </summary>
    public int CurrentBracketLevel { get; private set; } = 0;

    /// <summary>
    /// Reads the next token.
    /// </summary>
    /// <returns>The next token.</returns>
    public virtual string Read()
    {
        var lex = Reader.Read();

        if (string.IsNullOrEmpty(lex)) return string.Empty;

        if (lex == "(")
        {
            CurrentBracketLevel++;
            return lex;
        }

        if (lex == ")")
        {
            CurrentBracketLevel--;
            return lex;
        }

        using var sb = ZString.CreateStringBuilder();
        sb.Append(lex);

        // Explore possible two-word tokens
        if (lex.IsEqualNoCase("is"))
        {
            var next = string.Empty;

            if (Reader.TryRead("not", out next))
            {
                sb.Append(" " + next);
            }

            if (Reader.TryRead("distinct", out next))
            {
                // is distinct from, is not distinct from
                sb.Append(" " + next);
                sb.Append(" " + Reader.Read("from"));
                return sb.ToString();
            }

            // is, is not
            return sb.ToString();
        }

        if (lex.IsEqualNoCase(JoinLexs))
        {
            sb.Append(" " + Reader.Read("join"));
            return sb.ToString();
        }

        if (lex.IsEqualNoCase(ByLexs))
        {
            sb.Append(" " + Reader.Read("by"));
            return sb.ToString();
        }

        if (lex.IsEqualNoCase(OuterJoinLexs))
        {
            if (Reader.TryPeek('('))
            {
                return lex;
            }

            Reader.TryRead("outer", out _);

            sb.Append(" " + Reader.Read("join"));
            return sb.ToString();
        }

        if (lex.IsEqualNoCase("nulls"))
        {
            sb.Append(" " + Reader.Read(NullsSortTokens));
            return sb.ToString();
        }

        if (lex.IsEqualNoCase("union"))
        {
            if (Reader.TryRead("all", out var all))
            {
                sb.Append(" " + all);
            }
            return sb.ToString();
        }

        if (lex.IsEqualNoCase("not"))
        {
            if (Reader.TryRead("materialized", out var materialized))
            {
                sb.Append(" " + materialized);
                return sb.ToString();
            }

            if (Reader.TryRead("null", out var value))
            {
                sb.Append(" " + value);
                return sb.ToString();
            }

            return sb.ToString();
        }

        if (lex.IsEqualNoCase("double"))
        {
            if (Reader.TryRead("precision", out var precision))
            {
                sb.Append(" " + precision);
            }
            return sb.ToString();
        }

        if (lex.IsEqualNoCase("at") || lex.IsEqualNoCase("without"))
        {
            sb.Append(" " + Reader.Read("time"));
            sb.Append(" " + Reader.Read("zone"));
            return sb.ToString();
        }

        if (lex.IsEqualNoCase("create"))
        {
            if (Reader.TryRead("temporary", out var precision))
            {
                sb.Append(" " + precision);
                sb.Append(" " + Reader.Read("table"));
                return sb.ToString();
            }

            if (Reader.TryRead("table", out var table))
            {
                sb.Append(" " + table);
                return sb.ToString();
            }

            if (Reader.TryRead("unique", out var unique))
            {
                sb.Append(" " + unique);
                sb.Append(" " + Reader.Read("index"));
                return sb.ToString();
            }

            if (Reader.TryRead("index", out var index))
            {
                sb.Append(" " + index);
                return sb.ToString();
            }
        }

        if (lex.IsEqualNoCase("alter"))
        {
            if (Reader.TryRead("table", out var table))
            {
                sb.Append(" " + table);
                return sb.ToString();
            }
            if (Reader.TryRead("column", out var column))
            {
                sb.Append(" " + column);
                return sb.ToString();
            }
            throw new NotSupportedException(Reader.Read());
        }

        if (lex.IsEqualNoCase(new[] { "delete", "update" }))
        {
            if (Reader.TryRead("cascade", out var cascade))
            {
                //delete(update) cascade
                sb.Append(" " + cascade);
                return sb.ToString();
            }
            if (Reader.TryRead("set", out var s))
            {
                //delete(update) set null
                var n = Reader.Read("null");
                sb.Append(" " + s + " " + n);
                return sb.ToString();
            }
            if (Reader.TryRead("no", out var no))
            {
                //delete(update) no action
                var action = Reader.Read("action");
                sb.Append(" " + no + " " + action);
                return sb.ToString();
            }
            if (Reader.TryRead("restrict", out var restrict))
            {
                //delete(update) restrict
                sb.Append(" " + restrict);
                return sb.ToString();
            }
            return sb.ToString();
        }

        if (lex.IsEqualNoCase(new[] { "insert" }))
        {
            if (Reader.TryRead("into", out var into))
            {
                //insert into
                sb.Append(" " + into);
                return sb.ToString();
            }
            return sb.ToString();
        }
        //if (lex.IsEqualNoCase("generated"))
        //{
        //	if (Reader.TryRead("always", out var always))
        //	{
        //		sb.Append(" " + always);
        //		sb.Append(" " + Reader.Read("as"));
        //		sb.Append(" " + Reader.Read("identity"));
        //		return sb.ToString();
        //	}
        //	else if (Reader.TryRead("by", out var by))
        //	{
        //		sb.Append(" " + by);
        //		sb.Append(" " + Reader.Read("default"));
        //		sb.Append(" " + Reader.Read("as"));
        //		sb.Append(" " + Reader.Read("identity"));
        //		return sb.ToString();
        //	}
        //	return sb.ToString();
        //}

        if (lex.IsEqualNoCase(KeyLexs))
        {
            sb.Append(" " + Reader.Read("key"));
            return sb.ToString();
        }
        return lex;
    }
}