using Xunit.Abstractions;

namespace Carbunql.TypeSafe.Test;

public class WindowFunctionTest
{
    public WindowFunctionTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private ITestOutputHelper Output { get; }

    [Fact]
    public void RowNumber()
    {
        var od = Sql.DefineDataSet<order_detail>();

        var query = Sql.From(() => od)
                .Select(() => new
                {
                    od.order_detail_id,
                    no_argument = Sql.RowNumber(),
                    partition_only = Sql.RowNumber(
                        () => new { od.order_id },
                        () => null
                    ),
                    order_only = Sql.RowNumber(
                        () => null,
                        () => new { od.order_detail_id }
                    ),
                    partition_order = Sql.RowNumber(
                        () => new { od.order_id },
                        () => new { od.order_detail_id }
                    ),
                });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    od.order_detail_id,
    ROW_NUMBER() AS no_argument,
    ROW_NUMBER() OVER(
        PARTITION BY
            od.order_id
    ) AS partition_only,
    ROW_NUMBER() OVER(
        ORDER BY
            od.order_detail_id
    ) AS order_only,
    ROW_NUMBER() OVER(
        PARTITION BY
            od.order_id
        ORDER BY
            od.order_detail_id
    ) AS partition_order
FROM
    order_detail AS od";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void Count()
    {
        var od = Sql.DefineDataSet<order_detail>();

        var query = Sql.From(() => od)
                .Select(() => new
                {
                    od.order_detail_id,
                    no_argument = Sql.Count(),
                    partition_only = Sql.Count(
                        () => new { od.order_id },
                        () => null
                    ),
                    order_only = Sql.Count(
                        () => null,
                        () => new { od.order_detail_id }
                    ),
                    partition_order = Sql.Count(
                        () => new { od.order_id },
                        () => new { od.order_detail_id }
                    ),
                });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    od.order_detail_id,
    COUNT(*) AS no_argument,
    COUNT(*) OVER(
        PARTITION BY
            od.order_id
    ) AS partition_only,
    COUNT(*) OVER(
        ORDER BY
            od.order_detail_id
    ) AS order_only,
    COUNT(*) OVER(
        PARTITION BY
            od.order_id
        ORDER BY
            od.order_detail_id
    ) AS partition_order
FROM
    order_detail AS od";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void Sum()
    {
        var od = Sql.DefineDataSet<order_detail>();

        var query = Sql.From(() => od)
                .Select(() => new
                {
                    od.order_detail_id,
                    no_argument = Sql.Sum(() => od.quantity),
                    partition_only = Sql.Sum(
                        () => od.quantity,
                        () => new { od.order_id },
                        () => null
                    ),
                    order_only = Sql.Sum(
                        () => od.quantity,
                        () => null,
                        () => new { od.order_detail_id }
                    ),
                    partition_order = Sql.Sum(
                        () => od.quantity,
                        () => new { od.order_id },
                        () => new { od.order_detail_id }
                    ),
                });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    od.order_detail_id,
    SUM(od.quantity) AS no_argument,
    SUM(od.quantity) OVER(
        PARTITION BY
            od.order_id
    ) AS partition_only,
    SUM(od.quantity) OVER(
        ORDER BY
            od.order_detail_id
    ) AS order_only,
    SUM(od.quantity) OVER(
        PARTITION BY
            od.order_id
        ORDER BY
            od.order_detail_id
    ) AS partition_order
FROM
    order_detail AS od";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void Min()
    {
        var od = Sql.DefineDataSet<order_detail>();

        var query = Sql.From(() => od)
                .Select(() => new
                {
                    od.order_detail_id,
                    no_argument = Sql.Min(() => od.quantity),
                    partition_only = Sql.Min(
                        () => od.quantity,
                        () => new { od.order_id },
                        () => null
                    ),
                    order_only = Sql.Min(
                        () => od.quantity,
                        () => null,
                        () => new { od.order_detail_id }
                    ),
                    partition_order = Sql.Min(
                        () => od.quantity,
                        () => new { od.order_id },
                        () => new { od.order_detail_id }
                    ),
                });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    od.order_detail_id,
    MIN(od.quantity) AS no_argument,
    MIN(od.quantity) OVER(
        PARTITION BY
            od.order_id
    ) AS partition_only,
    MIN(od.quantity) OVER(
        ORDER BY
            od.order_detail_id
    ) AS order_only,
    MIN(od.quantity) OVER(
        PARTITION BY
            od.order_id
        ORDER BY
            od.order_detail_id
    ) AS partition_order
FROM
    order_detail AS od";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void Max()
    {
        var od = Sql.DefineDataSet<order_detail>();

        var query = Sql.From(() => od)
                .Select(() => new
                {
                    od.order_detail_id,
                    no_argument = Sql.Max(() => od.quantity),
                    partition_only = Sql.Max(
                        () => od.quantity,
                        () => new { od.order_id },
                        () => null
                    ),
                    order_only = Sql.Max(
                        () => od.quantity,
                        () => null,
                        () => new { od.order_detail_id }
                    ),
                    partition_order = Sql.Max(
                        () => od.quantity,
                        () => new { od.order_id },
                        () => new { od.order_detail_id }
                    ),
                });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    od.order_detail_id,
    MAX(od.quantity) AS no_argument,
    MAX(od.quantity) OVER(
        PARTITION BY
            od.order_id
    ) AS partition_only,
    MAX(od.quantity) OVER(
        ORDER BY
            od.order_detail_id
    ) AS order_only,
    MAX(od.quantity) OVER(
        PARTITION BY
            od.order_id
        ORDER BY
            od.order_detail_id
    ) AS partition_order
FROM
    order_detail AS od";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void Average()
    {
        var od = Sql.DefineDataSet<order_detail>();

        var query = Sql.From(() => od)
                .Select(() => new
                {
                    od.order_detail_id,
                    no_argument = Sql.Average(() => od.quantity),
                    partition_only = Sql.Average(
                        () => od.quantity,
                        () => new { od.order_id },
                        () => null
                    ),
                    order_only = Sql.Average(
                        () => od.quantity,
                        () => null,
                        () => new { od.order_detail_id }
                    ),
                    partition_order = Sql.Average(
                        () => od.quantity,
                        () => new { od.order_id },
                        () => new { od.order_detail_id }
                    ),
                });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    od.order_detail_id,
    AVG(od.quantity) AS no_argument,
    AVG(od.quantity) OVER(
        PARTITION BY
            od.order_id
    ) AS partition_only,
    AVG(od.quantity) OVER(
        ORDER BY
            od.order_detail_id
    ) AS order_only,
    AVG(od.quantity) OVER(
        PARTITION BY
            od.order_id
        ORDER BY
            od.order_detail_id
    ) AS partition_order
FROM
    order_detail AS od";

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
