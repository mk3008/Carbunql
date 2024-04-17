using Carbunql.Extensions;
using Cysharp.Text;

namespace Carbunql;

public class CommandTextBuilder
{
    public CommandTextBuilder(TokenFormatLogic formatter)
    {
        Formatter = formatter;
    }

    public CommandTextBuilder()
    {
        Formatter = new TokenFormatLogic();
    }

    public Action<string>? Logger { get; set; }

    public TokenFormatLogic Formatter { get; init; }

    private Token? PrevToken { get; set; }

    private List<(Token Token, int Level)> IndentLevels { get; set; } = new();

    private int Level { get; set; }

    private Dictionary<int, string> SpacerCache = new();

    public void Init()
    {
        Level = 0;
        IndentLevels.Clear();
        PrevToken = null;
    }

    public string Execute(IQueryCommandable cmd)
    {
        return Execute(cmd.GetTokens());
    }

    public string Execute(IEnumerable<Token> tokens)
    {
        Init();

        using var sb = ZString.CreateStringBuilder();

        foreach (var t in tokens)
        {
            if (PrevToken == null || t.Parents().Count() == PrevToken.Parents().Count())
            {
                foreach (var item in GetTokenTexts(t)) sb.Append(item);
                continue;
            }

            if (t.Parents().Count() > PrevToken.Parents().Count())
            {
                if (t.Parent != null && Formatter.IsIncrementIndentOnBeforeWriteToken(t.Parent))
                {
                    Logger?.Invoke($"increment indent and line break on before : {t.Text}");
                    Level++;
                    IndentLevels.Add((t.Parent, Level));
                    sb.Append(GetLineBreakText());
                }
                foreach (var item in GetTokenTexts(t)) sb.Append(item);
                continue;
            }

            if (Formatter.IsDecrementIndentOnBeforeWriteToken(t))
            {
                var q = IndentLevels.Where(x => x.Token != null && x.Token.Equals(t.Parent)).Select(x => x.Level);
                var lv = 0;
                if (q.Any()) lv = q.First();
                if (lv != Level)
                {
                    Logger?.Invoke($"decrement indent and line break on before : {t.Text}");
                    Level = lv;
                    sb.Append(GetLineBreakText());
                }
                else
                {
                    Logger?.Invoke($"*Indentation is invalid because the levels are the same : {t.Text}");
                }
            }

            foreach (var item in GetTokenTexts(t)) sb.Append(item);
        }
        return sb.ToString();
    }

    private IEnumerable<string> GetTokenTexts(Token? token)
    {
        if (token == null) yield break;
        if (string.IsNullOrEmpty(token.Text)) yield break;

        if (PrevToken != null && Formatter.IsLineBreakOnBeforeWriteToken(token))
        {
            yield return GetLineBreakText();
        }
        yield return GetTokenTextCore(token);
        if (Formatter.IsLineBreakOnAfterWriteToken(token))
        {
            yield return GetLineBreakText();
        }
    }

    private string GetTokenTextCore(Token token)
    {
        using var sb = ZString.CreateStringBuilder();

        //save indent level
        if (token.Parent != null)
        {
            var q = IndentLevels.Where(x => x.Token != null && x.Token.Equals(token.Parent)).Select(x => x.Level);
            if (!q.Any()) IndentLevels.Add((token.Parent, Level));
        }

        if (token.NeedsSpace(PrevToken)) sb.Append(' ');

        PrevToken = token;

        if (token.IsReserved)
        {
            sb.Append(token.Text.ToUpper());
        }
        else
        {
            sb.Append(token.Text);
        }

        return sb.ToString();
    }

    private string GetLineBreakText()
    {
        //init previous token
        PrevToken = null;

        // NOTE:
        // For performance improvement, remove unnecessary indent information
        // However, keep one for the starting position of brackets
        var startIndent = IndentLevels.Where(x => x.Level == Level).FirstOrDefault();
        IndentLevels.RemoveAll(x => Level <= x.Level && x != startIndent);

        //line break
        if (!SpacerCache.ContainsKey(Level))
        {
            SpacerCache[Level] = (Level * 4).ToSpaceString();
        }
        return "\r\n" + SpacerCache[Level];
    }
}