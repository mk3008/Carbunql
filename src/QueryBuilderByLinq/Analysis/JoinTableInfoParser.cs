using System.Linq.Expressions;

namespace QueryBuilderByLinq.Analysis;

public class JoinTableInfoParser
{
	public static List<JoinTableInfo> Parse(Expression exp)
	{
		return new List<JoinTableInfo>();
	}
}
