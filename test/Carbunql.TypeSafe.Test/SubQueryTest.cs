using Xunit.Abstractions;

namespace Carbunql.TypeSafe.Test;

public class SubQueryTest
{
    public SubQueryTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private ITestOutputHelper Output { get; }

    private FluentSelectQuery<store> SelectStoreOfJapan()
    {
        var localtion = "japan";
        var s = Sql.DefineDataSet<store>();
        var query = Sql.From(() => s).Where(() => s.location == localtion);
        return query.Compile<store>();
    }

    private FluentSelectQuery<order_detail_report> SelectOrderDetailReport()
    {
        var od = Sql.DefineDataSet<order_detail>();
        var o = Sql.DefineDataSet<order>();
        var query = Sql.From(() => od)
            .InnerJoin(() => o, () => od.order_id == o.order_id)
            .Select(() => od)
            .Select(() => o);
        return query.Compile<order_detail_report>();
    }

    private FluentSelectQuery<order> SelectTodayOrder()
    {
        var o = Sql.DefineDataSet<order>();
        var query = Sql.From(() => o).Where(() => o.order_date == Sql.Now);
        return query.Compile<order>();
    }

    private FluentSelectQuery<order> SelectOrderByStoreId(int store_id)
    {
        var o = Sql.DefineDataSet<order>();
        var query = Sql.From(() => o)
            .Where(() => o.store_id == store_id);
        return query.Compile<order>();
    }

    private FluentSelectQuery<order> SelectOrder()
    {
        var o = Sql.DefineDataSet<order>();
        var query = Sql.From(() => o);
        return query.Compile<order>();
    }

    [Fact]
    public void SubQuery()
    {
        var o = Sql.DefineDataSet(() => SelectOrder());

        var query = Sql.From(() => o)
                .Select(() => new { o.store_id });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    o.store_id
FROM
    (
        SELECT
            o.order_id,
            o.order_date,
            o.customer_name,
            o.store_id
        FROM
            order AS o
    ) AS o";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void SubQuery_Parameter()
    {
        var o = Sql.DefineDataSet(() => SelectOrderByStoreId(1));

        var query = Sql.From(() => o)
                .Select(() => new { o.store_id });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    o.store_id
FROM
    (
        SELECT
            o.order_id,
            o.order_date,
            o.customer_name,
            o.store_id
        FROM
            order AS o
        WHERE
            o.store_id = :store_id
    ) AS o";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void SubQuery_Edit()
    {
        var o = Sql.DefineDataSet(() => SelectOrder(), all_order =>
        {
            var x = Sql.DefineDataSet(() => all_order);
            return Sql.From(() => x).Where(() => x.store_id == 1).Compile<order>();
        });

        var query = Sql.From(() => o)
                .Select(() => new { o.store_id });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"WITH
    all_order AS (
        SELECT
            o.order_id,
            o.order_date,
            o.customer_name,
            o.store_id
        FROM
            order AS o
    )
SELECT
    o.store_id
FROM
    (
        SELECT
            x.order_id,
            x.order_date,
            x.customer_name,
            x.store_id
        FROM
            all_order AS x
        WHERE
            x.store_id = 1
    ) AS o";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void SubQueryJoin()
    {
        // Definition DataSet
        var r = Sql.DefineDataSet(() => SelectOrderDetailReport());
        var s = Sql.DefineDataSet(() => SelectStoreOfJapan());

        var query = Sql.From(() => r)
            .InnerJoin(() => s, () => r.store_id == s.store_id)
            .Select(() => r)
            .Select(() => new
            {
                store_name = s.name,
            });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    r.order_date,
    r.customer_name,
    r.store_id,
    r.order_detail_id,
    r.order_id,
    r.product_id,
    r.quantity,
    r.price,
    s.name AS store_name
FROM
    (
        SELECT
            od.order_detail_id,
            od.order_id,
            od.product_id,
            od.quantity,
            od.price,
            o.order_date,
            o.customer_name,
            o.store_id
        FROM
            order_detail AS od
            INNER JOIN order AS o ON od.order_id = o.order_id
    ) AS r
    INNER JOIN (
        SELECT
            s.store_id,
            s.name,
            s.location
        FROM
            store AS s
        WHERE
            s.location = :localtion
    ) AS s ON r.store_id = s.store_id";

        Assert.Equal(expect, actual, true, true, true);
    }

    public record product : IDataRow
    {
        public int product_id { get; set; }
        public string name { get; set; } = string.Empty;
        public decimal price { get; set; }

        // interface property
        IDataSet IDataRow.DataSet { get; set; } = null!;
    }

    public record store : IDataRow
    {
        public int store_id { get; set; }
        public string name { get; set; } = string.Empty;
        public string location { get; set; } = string.Empty;

        // interface property
        IDataSet IDataRow.DataSet { get; set; } = null!;
    }

    public record order : IDataRow
    {
        public int order_id { get; set; }
        public DateTime order_date { get; set; }
        public string customer_name { get; set; } = string.Empty;
        public int store_id { get; set; }
        public IList<order_detail> order_details { get; init; } = new List<order_detail>();

        // interface property
        IDataSet IDataRow.DataSet { get; set; } = null!;
    }

    public record order_detail : IDataRow
    {
        public int order_detail_id { get; set; }
        public int order_id { get; set; }
        public int product_id { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }

        // interface property
        IDataSet IDataRow.DataSet { get; set; } = null!;
    }

    public record order_detail_report : order_detail
    {
        public DateTime order_date { get; set; }
        public string customer_name { get; set; } = string.Empty;
        public int store_id { get; set; }
    }
}
