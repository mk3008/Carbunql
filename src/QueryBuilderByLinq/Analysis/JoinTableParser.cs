using System.Linq.Expressions;

namespace QueryBuilderByLinq.Analysis;

public class JoinTableParser
{
	public static List<JoinTableInfo> Parse(Expression exp)
	{
		return new List<JoinTableInfo>();
	}
}
