using System.Reflection;

namespace Carbunql;

public static class TableJoinEnumReader
{
    private static Dictionary<string, CommandAttribute> Cache { get; set; } = new();

    public static Dictionary<string, CommandAttribute> GetCommandAttributes()
    {
        lock (Cache)
        {
            if (Cache.Any()) return Cache;

            foreach (var name in Enum.GetNames(typeof(TableJoin)))
            {
                var field = typeof(TableJoin).GetField(name);
                if (field == null) continue;
                var atr = field.GetCustomAttribute<CommandAttribute>();
                if (atr == null) continue;
                Cache.Add(name, atr);
            }
            return Cache;
        }
    }

    public static string GetName(TableJoin item)
    {
        var name = Enum.GetName(typeof(TableJoin), item);
        if (name == null) throw new Exception();
        return name;
    }

    public static CommandAttribute GetCommandAttribute(TableJoin item)
    {
        return GetCommandAttributes().Where(x => x.Key == GetName(item)).Select(x => x.Value).First();
    }
}