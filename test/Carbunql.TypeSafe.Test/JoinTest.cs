using Xunit.Abstractions;

namespace Carbunql.TypeSafe.Test;

public class JoinTest
{
    public JoinTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private ITestOutputHelper Output { get; }

    [Fact]
    public void InnerJoin()
    {
        var od = Sql.DefineTable<order_detail>();
        var o = Sql.DefineTable<order>();
        var p = Sql.DefineTable<product>();
        var s = Sql.DefineTable<store>();

        var query = Sql.From(() => od)
                        .InnerJoin(() => o, () => o.order_id == od.order_id)
                        .InnerJoin(() => p, () => od.product_id == p.product_id)
                        .InnerJoin(() => s, () => o.store_id == s.store_id);

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    *
FROM
    order_detail AS od
    INNER JOIN order AS o ON o.order_id = od.order_id
    INNER JOIN product AS p ON od.product_id = p.product_id
    INNER JOIN store AS s ON o.store_id = s.store_id";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void LeftJoin()
    {
        var o = Sql.DefineTable<order>();
        var od = Sql.DefineTable<order_detail>();

        var query = Sql.From(() => o)
                        .LeftJoin(() => od, () => o.order_id == od.order_id);

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    *
FROM
    order AS o
    LEFT JOIN order_detail AS od ON o.order_id = od.order_id";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void CrossJoin()
    {
        var p = Sql.DefineTable<product>();
        var s = Sql.DefineTable<store>();

        var query = Sql.From(() => p)
                        .CrossJoin(() => s);

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    *
FROM
    product AS p
    CROSS JOIN store AS s";

        Assert.Equal(expect, actual, true, true, true);
    }

    public record product(int product_id, string name, decimal price) : ITableRowDefinition
    {
        // no arguments constructor.
        // Since it is used as a definition, it has no particular meaning as a value.
        public product() : this(0, "", 0) { }

        // interface property
        IDatasource ITableRowDefinition.Datasource { get; set; } = null!;
    }

    public record store(int store_id, string name, string location) : ITableRowDefinition
    {
        // no arguments constructor.
        // Since it is used as a definition, it has no particular meaning as a value.
        public store() : this(0, "", "") { }

        // interface property
        IDatasource ITableRowDefinition.Datasource { get; set; } = null!;
    }

    public record order(int order_id, DateTime order_date, string customer_name, int store_id, List<order_detail> order_details) : ITableRowDefinition
    {
        // no arguments constructor.
        // Since it is used as a definition, it has no particular meaning as a value.
        public order() : this(0, DateTime.Now, "", 0, new List<order_detail>()) { }

        // interface property
        IDatasource ITableRowDefinition.Datasource { get; set; } = null!;
    }

    public record order_detail(int order_detail_id, int order_id, int product_id, int quantity, decimal price) : ITableRowDefinition
    {
        // no arguments constructor.
        // Since it is used as a definition, it has no particular meaning as a value.
        public order_detail() : this(0, 0, 0, 0, 0) { }

        // interface property
        IDatasource ITableRowDefinition.Datasource { get; set; } = null!;
    }
}
