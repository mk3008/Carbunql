using System.Linq.Expressions;

namespace QueryBuilderByLinq.Analysis;

public class SelectColumnInfoParser
{
	public static List<JoinTableInfo> Parse(Expression exp)
	{
		return new List<JoinTableInfo>();
	}
}
