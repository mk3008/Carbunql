using Xunit.Abstractions;

namespace Carbunql.TypeSafe.Test;

public class CTETest
{
    public CTETest(ITestOutputHelper output)
    {
        Output = output;
    }

    private ITestOutputHelper Output { get; }

    private FluentSelectQuery<order> SelectTodayOrder()
    {
        var o = Sql.DefineTable<order>();
        var query = Sql.From(() => o).Where(() => o.order_date == Sql.Now);
        return query.Compile<order>();
    }

    [Fact]
    public void CTE()
    {
        // Assign to a variable
        var today_order = SelectTodayOrder();

        // Pass the variable using an Expression
        var o = Sql.DefineCTE(() => today_order);

        var query = Sql.From(() => o)
                .Select(() => new { o.store_id });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"WITH
    today_order AS (
        SELECT
            *
        FROM
            order AS o
        WHERE
            o.order_date = CAST(NOW() AS timestamp)
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
        var o = Sql.DefineMaterializedCTE(() => filtered_order);

        var query = Sql.From(() => o)
                .Select(() => new { o.store_id });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"WITH
    filtered_order AS MATERIALIZED (
        SELECT
            *
        FROM
            order AS o
        WHERE
            o.order_date = CAST(NOW() AS timestamp)
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
        var o = Sql.DefineNotMaterializedCTE(() => filtered_order);

        var query = Sql.From(() => o)
                .Select(() => new { o.store_id });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"WITH
    filtered_order AS NOT MATERIALIZED (
        SELECT
            *
        FROM
            order AS o
        WHERE
            o.order_date = CAST(NOW() AS timestamp)
    )
SELECT
    o.store_id
FROM
    filtered_order AS o";

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
