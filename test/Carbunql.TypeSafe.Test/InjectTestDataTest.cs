using Xunit.Abstractions;

namespace Carbunql.TypeSafe.Test;

public class InjectTestDataTest
{
    public InjectTestDataTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private ITestOutputHelper Output { get; }

    [Fact]
    public void UnitTestByCTE()
    {
        // Define a dataset for the store entity
        var s = Sql.DefineDataSet<store>();
        var query = Sql.From(() => s);

        // Inject test data into the WITH clause
        var store = new FluentSelectQuery<store>([
            new store() { store_id = 1, name = "Tokyo Store", location = "Japan" },
            new store() { store_id = 2, name = "Osaka Store", location = "Japan" },
            new store() { store_id = 3, name = "Nagoya Store", location = "Japan" }
        ]);
        query.With(() => store);

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"WITH
    store AS (
        SELECT
            v.store_id,
            v.name,
            v.location
        FROM
            (
                VALUES
                    (1, CAST('Tokyo Store' AS text), CAST('Japan' AS text)),
                    (2, CAST('Osaka Store' AS text), CAST('Japan' AS text)),
                    (3, CAST('Nagoya Store' AS text), CAST('Japan' AS text))
            ) AS v (
                store_id, name, location
            )
    )
SELECT
    *
FROM
    store AS s";

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
