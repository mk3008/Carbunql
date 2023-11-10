using Carbunql.Clauses;

namespace QueryBuilderByLinq.Analysis;

public class JoinTableInfo
{
	public JoinTableInfo(TableInfo tableInfo, string relation)
	{
		TableInfo = tableInfo;
		switch (relation)
		{
			case nameof(Sql.InnerJoinTable):
				Relation = "inner join";
				break;
			case nameof(Sql.LeftJoinTable):
				Relation = "left join";
				break;
			case nameof(Sql.CrossJoinTable):
				Relation = "cross join";
				break;
			default:
				throw new NotSupportedException();
		}
	}

	public JoinTableInfo(TableInfo tableInfo, string relation, ValueBase condition)
	{
		TableInfo = tableInfo;
		switch (relation)
		{
			case nameof(Sql.InnerJoinTable):
				Relation = "inner join";
				break;
			case nameof(Sql.LeftJoinTable):
				Relation = "left join";
				break;
			case nameof(Sql.CrossJoinTable):
				Relation = "cross join";
				break;
			default:
				throw new NotSupportedException();
		}
		Condition = condition;
	}

	public TableInfo TableInfo { get; set; }

	public string Relation { get; private set; }

	public ValueBase? Condition { get; set; }
}
