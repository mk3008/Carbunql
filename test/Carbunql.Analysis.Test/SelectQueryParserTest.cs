using Xunit.Abstractions;

namespace Carbunql.Analysis.Test;

public class SelectQueryParserTest
{
    private readonly QueryCommandMonitor Monitor;

    public SelectQueryParserTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void SortSample()
    {
        var text = @"
select
    *
from 
    table_a a
order by 
    a.name nulls first,
    a.val desc,
    a.table_a_id";

        var item = QueryParser.Parse(text) as SelectQuery;
        if (item == null) throw new Exception();

        Monitor.Log(item);

        Assert.NotNull(item.SelectClause);
        Assert.Single(item.SelectClause);
        Assert.NotNull(item.OrderClause);
        Assert.Equal(3, item.OrderClause!.Count());
    }

    [Fact]
    public void RelationSample()
    {
        var text = @"
select
    a.table_a_id as id,
    3.14 as val,
    (a.val + b.val) * 2 as calc, 
    b.table_b_id,
    c.table_c_id
from 
    table_a a
    inner join table_b b on a.table_a_id = b.table_a_id and b.visible = true
    left join table_c c on a.table_a_id = c.table_a_id";

        var item = QueryParser.Parse(text) as SelectQuery;
        if (item == null) throw new Exception();
        Monitor.Log(item);

        Assert.NotNull(item.SelectClause);
        Assert.Equal(5, item.SelectClause!.Count);
        Assert.Equal(2, item.FromClause!.Relations!.Count());
    }

    [Fact]
    public void GroupSample()
    {
        var text = @"
select
    a.name,
    a.sub_name,
    sum(a.val) as val
from 
    table_a a
group by
    a.name,
    a.sub_name
having
    sum(a.val) > 0";

        var item = QueryParser.Parse(text) as SelectQuery;
        if (item == null) throw new Exception();
        Monitor.Log(item);

        Assert.NotNull(item.SelectClause);
        Assert.Equal(3, item.SelectClause!.Count);
        Assert.NotNull(item.GroupClause);
        Assert.Equal(2, item.GroupClause!.Count());
        Assert.NotNull(item.HavingClause);
    }

    [Fact]
    public void UnionSample()
    {
        var text = @"
select
    a.id
from
    table_a as a
union
select
    b.id
from
    table_b as b
union all
select
    c.id
from
    table_c as c";

        var item = QueryParser.Parse(text) as SelectQuery;
        if (item == null) throw new Exception();
        Monitor.Log(item);
    }

    [Fact]
    public void WithSample()
    {
        var text = @"
with
a as (
    select
        a.id
    from
        table_a as a
), 
b as (
    select
        a.id
    from
        a
)
select
    *
from
    b";

        var item = QueryParser.Parse(text) as SelectQuery;
        if (item == null) throw new Exception();
        Monitor.Log(item);
    }

    [Fact]
    public void LimitSample()
    {
        var text = @"
select
    a.id
from
    table_a as a
limit 10";

        var item = QueryParser.Parse(text) as SelectQuery;
        if (item == null) throw new Exception();
        Monitor.Log(item);
    }

    [Fact]
    public void LimitSample_Postgres()
    {
        var text = @"
select
    a.id
from
    table_a as a
limit 10 offset 3";

        var item = QueryParser.Parse(text) as SelectQuery;
        if (item == null) throw new Exception();
        Monitor.Log(item);
    }

    [Fact]
    public void LimitSample_MySQL()
    {
        var text = @"
select
    a.id
from
    table_a as a
limit 3, 10";

        var item = QueryParser.Parse(text) as SelectQuery;
        if (item == null) throw new Exception();
        Monitor.Log(item);
    }

    [Fact]
    public void Sample()
    {
        var text = @"
with
dat(line_id, name, unit_price, amount, tax_rate) as ( 
    values
    (1, 'apple' , 105, 5, 0.07),
    (2, 'orange', 203, 3, 0.07),
    (3, 'banana', 233, 9, 0.07),
    (4, 'tea'   , 309, 7, 0.08),
    (5, 'coffee', 555, 9, 0.08),
    (6, 'cola'  , 456, 2, 0.08)
),
detail as (
    select  
        q.*,
        trunc(q.price * (1 + q.tax_rate)) - q.price as tax,
        q.price * (1 + q.tax_rate) - q.price as raw_tax
    from
        (
            select
                dat.*,
                (dat.unit_price * dat.amount) as price
            from
                dat
        ) q
), 
tax_summary as (
    select
        d.tax_rate,
        trunc(sum(raw_tax)) as total_tax
    from
        detail d
    group by
        d.tax_rate
)
select 
   line_id,
    name,
    unit_price,
    amount,
    tax_rate,
    price,
    price + tax as tax_included_price,
    tax
from
    (
        select
            line_id,
            name,
            unit_price,
            amount,
            tax_rate,
            price,
            tax + adjust_tax as tax
        from
            (
                select
                    q.*,
                    case when q.total_tax - q.cumulative >= q.priority then 1 else 0 end as adjust_tax
                from
                    (
                        select  
                            d.*, 
                            s.total_tax,
                            sum(d.tax) over (partition by d.tax_rate) as cumulative,
                            row_number() over (partition by d.tax_rate order by d.raw_tax % 1 desc, d.line_id) as priority
                        from
                            detail d
                            inner join tax_summary s on d.tax_rate = s.tax_rate
                    ) q
            ) q
    ) q
order by 
    line_id";

        var item = QueryParser.Parse(text) as SelectQuery;
        if (item == null) throw new Exception();
        Monitor.Log(item);
    }
}