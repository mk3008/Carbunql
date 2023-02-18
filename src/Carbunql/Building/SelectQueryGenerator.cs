using Carbunql.Values;

namespace Carbunql.Building;

public static class SelectQueryGenerator
{
	public static SelectQuery FromObject(object obj, string parameterSufix = ":")
	{
		var sq = new SelectQuery();
		foreach (var item in obj.GetType().GetProperties().ToList().Where(x => x.CanRead))
		{
			var value = item.GetValue(obj);
			var key = parameterSufix + item.Name;
			var c = new ParameterValue(key, value);
			sq.Select(c).As(item.Name);
		}
		return sq;
	}
}