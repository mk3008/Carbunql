using Xunit.Abstractions;

namespace Carbunql.TypeSafe.Test;

public class CTETest
{
    public CTETest(ITestOutputHelper output)
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

    [Fact]
    public void CTE()
    {
        // Assign to a variable
        var today_order = SelectTodayOrder();

        // Pass the variable using an Expression
        var o = Sql.DefineDataSet(() => today_order);

        var query = Sql.From(() => o)
                .Select(() => new { o.store_id });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"WITH
    today_order AS (
        SELECT
            o.order_id,
            o.order_date,
            o.customer_name,
            o.store_id
        FROM
            order AS o
        WHERE
            o.order_date = NOW()
    )
SELECT
    o.store_id
FROM
    today_order AS o";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void CTE_Materialized()
    {
        // Assign to a variable
        var filtered_order = SelectTodayOrder();

        // Pass the variable using an Expression
        var o = Sql.DefineMaterializedDataSet(() => filtered_order);

        var query = Sql.From(() => o)
                .Select(() => new { o.store_id });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"WITH
    filtered_order AS MATERIALIZED (
        SELECT
            o.order_id,
            o.order_date,
            o.customer_name,
            o.store_id
        FROM
            order AS o
        WHERE
            o.order_date = NOW()
    )
SELECT
    o.store_id
FROM
    filtered_order AS o";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void CTE_NotMaterialized()
    {
        // Assign to a variable
        var filtered_order = SelectTodayOrder();

        // Pass the variable using an Expression
        var o = Sql.DefineNotMaterializedDataSet(() => filtered_order);

        var query = Sql.From(() => o)
                .Select(() => new { o.store_id });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"WITH
    filtered_order AS NOT MATERIALIZED (
        SELECT
            o.order_id,
            o.order_date,
            o.customer_name,
            o.store_id
        FROM
            order AS o
        WHERE
            o.order_date = NOW()
    )
SELECT
    o.store_id
FROM
    filtered_order AS o";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void CTEJoin()
    {
        // Assign to a variable
        var order_detail_report = SelectOrderDetailReport();
        var store_of_jp = SelectStoreOfJapan();

        // Definition DataSet
        var r = Sql.DefineDataSet(() => order_detail_report);
        var s = Sql.DefineDataSet(() => store_of_jp);

        var query = Sql.From(() => r)
            .InnerJoin(() => s, () => r.store_id == s.store_id)
            .Select(() => r)
            .Select(() => new
            {
                store_name = s.name,
            });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"/*
  :localtion = 'japan'
*/
WITH
    order_detail_report AS (
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
    ),
    store_of_jp AS (
        SELECT
            s.store_id,
            s.name,
            s.location
        FROM
            store AS s
        WHERE
            s.location = :localtion
    )
SELECT
    r.order_detail_id,
    r.order_id,
    r.product_id,
    r.quantity,
    r.price,
    r.order_date,
    r.customer_name,
    r.store_id,
    s.name AS store_name
FROM
    order_detail_report AS r
    INNER JOIN store_of_jp AS s ON r.store_id = s.store_id";

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
