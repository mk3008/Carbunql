using Carbunql.Building;
using Xunit.Abstractions;

namespace Carbunql.TypeSafe.Test;

public class ExistsTest
{
    public ExistsTest(ITestOutputHelper output)
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

    [Fact]
    public void Exists()
    {
        var o = Sql.DefineDataSet<order>();

        var query = Sql.From(() => o)
            .Exists<store>(x => x.store_id == o.store_id);

        var actual = query.ToText();
        Output.WriteLine(query.ToText());

        var expect = @"SELECT
    *
FROM
    order AS o
WHERE
    EXISTS (
        SELECT
            *
        FROM
            store AS x
        WHERE
            x.store_id = o.store_id
    )";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void NotExists()
    {
        var o = Sql.DefineDataSet<order>();

        var query = Sql.From(() => o)
            .NotExists<store>(x => x.store_id == o.store_id);

        var actual = query.ToText();
        Output.WriteLine(query.ToText());

        var expect = @"SELECT
    *
FROM
    order AS o
WHERE
    NOT EXISTS (
        SELECT
            *
        FROM
            store AS x
        WHERE
            x.store_id = o.store_id
    )";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void WhereExists()
    {
        var o = Sql.DefineDataSet<order>();

        var query = Sql.From(() => o)
            .Where(() => Sql.Exists(Sql.DefineDataSet<store>, x => x.store_id == o.store_id)
                        || Sql.NotExists(Sql.DefineDataSet<store>, x => x.store_id == o.store_id)
            );

        var actual = query.ToText();
        Output.WriteLine(query.ToText());

        var expect = @"SELECT
    *
FROM
    order AS o
WHERE
    (EXISTS (
        SELECT
            *
        FROM
            store AS x
        WHERE
            x.store_id = o.store_id
    ) OR NOT EXISTS (
        SELECT
            *
        FROM
            store AS x
        WHERE
            x.store_id = o.store_id
    ))";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void ExistsSubQuery()
    {
        var o = Sql.DefineDataSet<order>();

        var query = Sql.From(() => o)
            .Exists(SelectStoreOfJapan, x => x.store_id == o.store_id);

        var actual = query.ToText();
        Output.WriteLine(query.ToText());

        var expect = @"/*
  :localtion = 'japan'
*/
SELECT
    *
FROM
    order AS o
WHERE
    EXISTS (
        SELECT
            *
        FROM
            (
                SELECT
                    s.store_id,
                    s.name,
                    s.location
                FROM
                    store AS s
                WHERE
                    s.location = :localtion
            ) AS x
        WHERE
            x.store_id = o.store_id
    )";

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