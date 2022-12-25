namespace Carbunql.Extensions;

public static class MaterializedTypeExtension
{
	public static string ToCommandText(this Materialized source)
	{
		switch (source)
		{
			case Materialized.Undefined:
				return string.Empty;
			case Materialized.Materialized:
				return "materialized";
			case Materialized.NotMaterialized:
				return "not materialized";
		}
		throw new NotSupportedException();
	}
}