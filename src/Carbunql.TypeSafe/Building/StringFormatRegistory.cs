namespace Carbunql.TypeSafe.Building;

internal class StringFormatRegistory(IConstantRegistry innerManager) : IConstantRegistry
{
    public readonly IConstantRegistry InnerManager = innerManager;

    public IEnumerable<KeyValuePair<string, object>> Parameters => InnerManager.Parameters;

    public int Count() => InnerManager.Count();

    public string Add(string key, object? value)
    {
        var v = ConverToDbDateFormat(value!.ToString()!);
        return InnerManager.Add(key, v);
    }

    private string ConverToDbDateFormat(string csharpFormat)
    {
        var replacements = new Dictionary<string, string>
        {
            {"yyyy", "YYYY"},
            {"MM", "MM"},
            {"dd", "DD"},
            {"HH", "HH24"},
            {"mm", "MI"},
            {"ss", "SS"},
            {"ffffff", "US"},
            {"fff", "MS"}
        };

        string dbformat = csharpFormat;

        foreach (var pair in replacements)
        {
            dbformat = dbformat.Replace(pair.Key, pair.Value);
        }

        return dbformat;
    }
}

