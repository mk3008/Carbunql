using Carbunql.Clauses;

namespace Carbunql.Building;

public static class CommonTableExtension
{
	public static CommonTable As(this CommonTable source, string alias)
	{
		source.SetAlias(alias);
		return source;
	}
}