using Xunit.Abstractions;

namespace Carbunql.TypeSafe.Test;

public class SubQueryTest
{
    public SubQueryTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private ITestOutputHelper Output { get; }

    private FluentSelectQuery SelectOrderById_NoType(int id)
    {
        var o = Sql.DefineTable<order>();
        var query = Sql.From(() => o)
            .Where(() => o.store_id == id);
        return query;
    }

    private FluentSelectQuery<order> SelectOrderById(int id)
    {
        var o = Sql.DefineTable<order>();
        var query = Sql.From(() => o)
            .Where(() => o.store_id == id);
        return query.Compile<order>();
    }

    private FluentSelectQuery<order> SelectOrder()
    {
        var o = Sql.DefineTable<order>();
        var query = Sql.From(() => o);
        return query.Compile<order>();
    }

    [Fact]
    public void SubQuery_NoType()
    {
        var o = Sql.DefineSubQuery<order>(SelectOrderById_NoType(1));

        var query = Sql.From(() => o)
                .Select(() => new { o.store_id });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"/*
  :id = 1
*/
SELECT
    o.store_id
FROM
    (
        SELECT
            *
        FROM
            order AS o
        WHERE
            o.store_id = :id
    ) AS o";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void SubQuery()
    {
        var o = Sql.DefineSubQuery(SelectOrderById(1));

        var query = Sql.From(() => o)
                .Select(() => new { o.store_id });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"/*
  :id = 1
*/
SELECT
    o.store_id
FROM
    (
        SELECT
            *
        FROM
            order AS o
        WHERE
            o.store_id = :id
    ) AS o";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void SubQuery_Injection()
    {
        var o = Sql.DefineSubQuery(() =>
        {
            var x = Sql.DefineSubQuery(SelectOrder());
            return Sql.From(() => x).Where(() => x.store_id == 1).Compile<order>();
        });

        var query = Sql.From(() => o)
                .Select(() => new { o.store_id });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    o.store_id
FROM
    (
        SELECT
            *
        FROM
            (
                SELECT
                    *
                FROM
                    order AS o
            ) AS x
        WHERE
            x.store_id = 1
    ) AS o";

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
