using Xunit.Abstractions;

namespace Carbunql.TypeSafe.Test;

public class GroupTest
{
    public GroupTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private ITestOutputHelper Output { get; }

    [Fact]
    public void GroupBy()
    {
        var od = Sql.DefineDataSet<order_detail>();

        var query = Sql.From(() => od)
                .Select(() => new
                {
                    od.order_id,
                    total_price = Sql.Sum(() => od.quantity * od.price),
                    total_quantity = Sql.Sum(() => od.quantity),
                    count = Sql.Count(),
                    avg_quantity = Sql.Average(() => od.quantity),
                    min_quantity = Sql.Min(() => od.quantity),
                    max_quantity = Sql.Max(() => od.quantity),
                })
                .GroupBy(() => new
                {
                    od.order_id,
                });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    od.order_id,
    SUM(CAST(od.quantity AS numeric) * od.price) AS total_price,
    SUM(od.quantity) AS total_quantity,
    COUNT(*) AS count,
    AVG(od.quantity) AS avg_quantity,
    MIN(od.quantity) AS min_quantity,
    MAX(od.quantity) AS max_quantity
FROM
    order_detail AS od
GROUP BY
    od.order_id";

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
