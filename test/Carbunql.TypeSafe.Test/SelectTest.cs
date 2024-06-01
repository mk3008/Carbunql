using Xunit.Abstractions;

namespace Carbunql.TypeSafe.Test;

public class SelectTest
{
    public SelectTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private ITestOutputHelper Output { get; }

    [Fact]
    public void SelectAllDataSet()
    {
        var od = Sql.DefineDataSet<order_detail>();

        var query = Sql.From(() => od)
            .Select(() => od);

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    od.order_detail_id,
    od.order_id,
    od.product_id,
    od.quantity,
    od.price
FROM
    order_detail AS od";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void SelectAllDataSetMultiple()
    {
        var od = Sql.DefineDataSet<order_detail>();
        var p = Sql.DefineDataSet<product>();

        var query = Sql.From(() => od)
            .InnerJoin(() => p, () => od.product_id == p.product_id)
            .Select(() => od)
            .Select(() => p);

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    od.order_detail_id,
    od.order_id,
    od.product_id,
    od.quantity,
    od.price,
    p.name
FROM
    order_detail AS od
    INNER JOIN product AS p ON od.product_id = p.product_id";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void SelectAllDataSetMultiple_WithAlias()
    {
        var od = Sql.DefineDataSet<order_detail>();
        var p = Sql.DefineDataSet<product>();

        var query = Sql.From(() => od)
            .InnerJoin(() => p, () => od.product_id == p.product_id)
            .Select(() => od)
            .Select(() => new
            {
                product_price = p.price,
                product_name = p.name,
            });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    od.order_detail_id,
    od.order_id,
    od.product_id,
    od.quantity,
    od.price,
    p.price AS product_price,
    p.name AS product_name
FROM
    order_detail AS od
    INNER JOIN product AS p ON od.product_id = p.product_id";

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

    public record order_detail_product : order_detail
    {
        public string product_name { get; set; } = string.Empty;
    }
}
