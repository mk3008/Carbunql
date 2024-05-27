using Xunit.Abstractions;

namespace Carbunql.TypeSafe.Test;

public class SubQueryTest
{
    public SubQueryTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private ITestOutputHelper Output { get; }

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

        var expect = @"/*
  :store_id = 1
*/
SELECT
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

    public record product(int product_id, string name, decimal price) : IDataRow
    {
        // no arguments constructor.
        // Since it is used as a definition, it has no particular meaning as a value.
        public product() : this(0, "", 0) { }

        // interface property
        IDataSet IDataRow.DataSet { get; set; } = null!;
    }

    public record store(int store_id, string name, string location) : IDataRow
    {
        // no arguments constructor.
        // Since it is used as a definition, it has no particular meaning as a value.
        public store() : this(0, "", "") { }

        // interface property
        IDataSet IDataRow.DataSet { get; set; } = null!;
    }

    public record order(int order_id, DateTime order_date, string customer_name, int store_id, List<order_detail> order_details) : IDataRow
    {
        // no arguments constructor.
        // Since it is used as a definition, it has no particular meaning as a value.
        public order() : this(0, DateTime.Now, "", 0, new List<order_detail>()) { }

        // interface property
        IDataSet IDataRow.DataSet { get; set; } = null!;
    }

    public record order_detail(int order_detail_id, int order_id, int product_id, int quantity, decimal price) : IDataRow
    {
        // no arguments constructor.
        // Since it is used as a definition, it has no particular meaning as a value.
        public order_detail() : this(0, 0, 0, 0, 0) { }

        // interface property
        IDataSet IDataRow.DataSet { get; set; } = null!;
    }
}
