using Carbunql.Values;
using MessagePack;

namespace Carbunql.Building.Test;

public partial class SerializeTest
{
    [Fact]
    public void SelectQueryTest()
    {
        var sq = SelectQuery.Parse("select a.column_1 as c1 from table_a as a");

        var json = MessagePackSerializer.Serialize(sq);
        Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

        var actual = MessagePackSerializer.Deserialize<SelectQuery>(json);
        Output.WriteLine(actual.ToText());

        Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
    }

    [Fact]
    public void JoinQuery()
    {
        var sq = SelectQuery.Parse("select a.* from table_a as a inner join table_b as b on a.id = b.id");

        var json = MessagePackSerializer.Serialize(sq);
        Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

        var actual = MessagePackSerializer.Deserialize<SelectQuery>(json);
        Output.WriteLine(actual.ToText());

        Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
    }

    [Fact]
    public void OperatableQuery()
    {
        var sq = new OperatableQuery("union", SelectQuery.Parse("select 1"));

        var json = MessagePackSerializer.Serialize(sq);
        Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

        var actual = MessagePackSerializer.Deserialize<OperatableQuery>(json);
        Output.WriteLine(actual.ToText());

        Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
    }

    [Fact]
    public void UnionQuery()
    {
        var sq = SelectQuery.Parse("select 1 as v1 union select 2 as v1 union all select 3 as v1");

        var json = MessagePackSerializer.Serialize(sq);
        Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

        var actual = MessagePackSerializer.Deserialize<SelectQuery>(json);
        Output.WriteLine(actual.ToText());

        Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
    }

    [Fact]
    public void ValueCollectionList()
    {
        var sq = new List<ValueCollection>
        {
            new ValueCollection()
            {
                new LiteralValue(1),
                new LiteralValue(2),
            },
            new ValueCollection()
            {
                new LiteralValue(3),
                new LiteralValue(4),
            }
        };

        var json = MessagePackSerializer.Serialize(sq);
        Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

        var actual = MessagePackSerializer.Deserialize<List<ValueCollection>>(json);
        Assert.Equal(2, actual.Count);
    }

    [Fact]
    public void ValuesQuery()
    {
        var sq = new ValuesQuery(new[,] { { "a1", "b1", "c1" }, { "a2", "b2", "c2" } });

        var json = MessagePackSerializer.Serialize(sq);
        Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

        var actual = MessagePackSerializer.Deserialize<ValuesQuery>(json);
        Output.WriteLine(actual.ToText());

        Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
    }

    [Fact]
    public void SampleQuery()
    {
        var sq = SelectQuery.Parse(@"
WITH
    dat (
        line_id, name, unit_price, quantity, tax_rate
    ) AS (
        VALUES
            (1, 'apple', 105, 5, 0.07),
            (2, 'orange', 203, 3, 0.07),
            (3, 'banana', 233, 9, 0.07),
            (4, 'tea', 309, 7, 0.08),
            (5, 'coffee', 555, 9, 0.08),
            (6, 'cola', 456, 2, 0.08)
    ),
    detail AS (
        SELECT
            q.*,
            TRUNC(q.price * (1 + q.tax_rate)) - q.price AS tax,
            q.price * (1 + q.tax_rate) - q.price AS raw_tax
        FROM
            (
                SELECT
                    dat.*,
                    (dat.unit_price * dat.quantity) AS price
                FROM
                    dat
            ) AS q
    ),
    tax_summary AS (
        SELECT
            d.tax_rate,
            TRUNC(SUM(raw_tax)) AS total_tax
        FROM
            detail AS d
        GROUP BY
            d.tax_rate
    )
SELECT
    line_id,
    name,
    unit_price,
    quantity,
    tax_rate,
    price,
    price + tax AS tax_included_price,
    tax
FROM
    (
        SELECT
            line_id,
            name,
            unit_price,
            quantity,
            tax_rate,
            price,
            tax + adjust_tax AS tax
        FROM
            (
                SELECT
                    q.*,
                    CASE
                        WHEN q.total_tax - q.cumulative >= q.priority THEN 1
                        ELSE 0
                    END AS adjust_tax
                FROM
                    (
                        SELECT
                            d.*,
                            s.total_tax,
                            SUM(d.tax) OVER(
                                PARTITION BY
                                    d.tax_rate
                            ) AS cumulative,
                            ROW_NUMBER() OVER(
                                PARTITION BY
                                    d.tax_rate
                                ORDER BY
                                    d.raw_tax % 1 DESC,
                                    d.line_id
                            ) AS priority
                        FROM
                            detail AS d
                            INNER JOIN tax_summary AS s ON d.tax_rate = s.tax_rate
                    ) AS q
            ) AS q
    ) AS q
ORDER BY
    line_id");

        var json = MessagePackSerializer.Serialize(sq);
        Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

        var actual = MessagePackSerializer.Deserialize<SelectQuery>(json);
        Output.WriteLine(actual.ToText());

        Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
    }

    [Fact]
    public void SampleQuery_WindowFunction()
    {
        var sq = SelectQuery.Parse(@"
with
v (id, name, value) as (
    values
    (1, 'a', 10)
    , (2, 'a', 20)
    , (3, 'b', 50)
    , (4, 'c', 70)
)
select  
    id
    , name
    , value
    , string_agg(id::text, ',') filter (where v.name = 'a') over (partition by name order by value) as text
from
    v");

        var json = MessagePackSerializer.Serialize(sq);
        Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

        var actual = MessagePackSerializer.Deserialize<SelectQuery>(json);
        Output.WriteLine(actual.ToText());

        Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
    }

    [Fact]
    public void WindowClause()
    {
        var sql = @"
SELECT sum(salary) OVER w, avg(salary) OVER w
  FROM empsalary
  WINDOW w AS (PARTITION BY depname ORDER BY salary DESC)
";
        var sq = SelectQuery.Parse(sql);

        var json = MessagePackSerializer.Serialize(sq);
        Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

        var actual = MessagePackSerializer.Deserialize<SelectQuery>(json);
        Output.WriteLine(actual.ToText());

        Assert.Equal(TruncateControlString(sql), TruncateControlString(actual!.ToText()));
    }
}
