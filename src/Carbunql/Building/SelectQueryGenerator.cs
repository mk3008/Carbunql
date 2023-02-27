using Carbunql.Values;

namespace Carbunql.Building;

public static class SelectQueryGenerator
{
	public static SelectQuery FromItem<T>(T item, string parameterSufix = ":", Func<string, string>? keyFormatter = null)
	{
		return FromList<T>(new[] { item }, parameterSufix, keyFormatter);
	}

	public static SelectQuery FromList<T>(IEnumerable<T> lst, string parameterSufix = ":", Func<string, string>? keyFormatter = null)
	{
		keyFormatter ??= (string x) => x;

		var props = typeof(T).GetProperties().Where(x => x.CanRead).Select(x => x.Name).ToList();
		var vq = ValuesQueryGenerator.FromList(lst, props, parameterSufix, keyFormatter);

		var aliases = new List<string>();
		props.ForEach(x => aliases.Add(keyFormatter(x)));

		return vq.ToSelectQuery(aliases);
	}
}