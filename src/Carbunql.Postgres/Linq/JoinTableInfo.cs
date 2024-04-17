using Carbunql.Clauses;

namespace Carbunql.Postgres.Linq;

public class JoinTableInfo
{
    public JoinTableInfo(SelectableTable tableInfo, string relation)
    {
        Table = tableInfo;
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

    public JoinTableInfo(SelectableTable tableInfo, string relation, ValueBase condition)
    {
        Table = tableInfo;
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

    public SelectableTable Table { get; set; }

    public string Relation { get; private set; }

    public ValueBase? Condition { get; set; }
}
