using Carbunql;
using Carbunql.Postgres.Linq;
using Carbunql.Postgres.Test;
using Xunit.Abstractions;
using static Carbunql.Postgres.Linq.Sql;

namespace Carbunql.Postgres.Test.Linq;

public class Demo
{
	private readonly QueryCommandMonitor Monitor;

	public Demo(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
		Output = output;
	}

	private ITestOutputHelper Output { get; set; }

	public record struct sale(int sales_id, int article_id, int quantity);
	public record struct article(int article_id, int category_id, string article_name, int price);
	public record struct category(int category_id, string category_name);

	[Fact]
	public void Test()
	{
		var q = from s in FromTable<sale>()
				from a in InnerJoinTable<article>(x => s.article_id == x.article_id)
				from c in InnerJoinTable<category>(x => a.category_id == x.category_id)
				select new
				{
					c.category_name,
					a.article_name,
					s.quantity
				};
		var sq = q.ToSelectQuery();
		Output.WriteLine(sq.ToText());

		/*
SELECT
    c.category_name,
    a.article_name,
    s.quantity
FROM
    sale AS s
    INNER JOIN article AS a ON s.article_id = a.article_id
    INNER JOIN category AS c ON a.category_id = c.category_id
		*/
	}

}
