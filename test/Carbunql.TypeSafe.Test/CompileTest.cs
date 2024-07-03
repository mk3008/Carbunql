using Xunit.Abstractions;

namespace Carbunql.TypeSafe.Test;

public class CompileTest
{
    public CompileTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private ITestOutputHelper Output { get; }

    [Fact]
    public void Compile_SelectAll()
    {
        var o = Sql.DefineDataSet<order>();

        var query = Sql.From(() => o).Compile<order>();

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    o.order_id,
    o.order_date,
    o.customer_name,
    o.store_id
FROM
    order AS o";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void Compile_SelectAll_Exception()
    {
        var o = Sql.DefineDataSet<order>();

        var query = Sql.From(() => o);

        var ex = Assert.Throws<InvalidProgramException>(() => query.Compile<order_detail>());
        Output.WriteLine(ex.Message);

        Assert.Equal("The select query is not compatible with 'order_detail'. Expect: order_detail, Actual: order", ex.Message);
    }

    [Fact]
    public void Compile_Excess()
    {
        var o = Sql.DefineDataSet<order>();

        var query = Sql.From(() => o).Select(() => new { o.order_id, o.store_id, o.order_date, o.customer_name, memo = "test" }).Compile<order>();

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"/*
  :const_0 = 'test'
*/
SELECT
    o.order_id,
    o.store_id,
    o.order_date,
    o.customer_name,
    :const_0 AS memo
FROM
    order AS o";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void Compile_Undersized()
    {
        var o = Sql.DefineDataSet<order>();

        var query = Sql.From(() => o).Select(() => new { o.order_id });

        var ex = Assert.Throws<InvalidProgramException>(() => query.Compile<order>());
        Output.WriteLine(ex.Message);

        Assert.Equal("The select query is not compatible with 'order'. The following columns are missing: order_date, customer_name, store_id", ex.Message);
    }

    [Fact]
    public void Compile_ForceCorrect()
    {
        var o = Sql.DefineDataSet<order>();

        var query = Sql.From(() => o).Select(() => new { o.order_id }).Compile<order>(true);

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    o.order_id,
    CAST(null AS timestamp) AS order_date,
    CAST(null AS text) AS customer_name,
    CAST(null AS integer) AS store_id
FROM
    order AS o";

        Assert.Equal(expect, actual, true, true, true);
    }

    public record product(int product_id, string name, decimal price) : IDataRow
    {
        // no arguments constructor.
        // Since it is used as a definition, it has no particular meaning as a value.
        public product() : this(0, "", 0) { }

        // interface property
        ITypeSafeDataSet IDataRow.DataSet { get; set; } = null!;
    }

    public record store(int store_id, string name, string location) : IDataRow
    {
        // no arguments constructor.
        // Since it is used as a definition, it has no particular meaning as a value.
        public store() : this(0, "", "") { }

        // interface property
        ITypeSafeDataSet IDataRow.DataSet { get; set; } = null!;
    }

    public record order(int order_id, DateTime order_date, string customer_name, int store_id, List<order_detail> order_details) : IDataRow
    {
        // no arguments constructor.
        // Since it is used as a definition, it has no particular meaning as a value.
        public order() : this(0, DateTime.Now, "", 0, new List<order_detail>()) { }

        // interface property
        ITypeSafeDataSet IDataRow.DataSet { get; set; } = null!;
    }

    public record order_detail(int order_detail_id, int order_id, int product_id, int quantity, decimal price) : IDataRow
    {
        // no arguments constructor.
        // Since it is used as a definition, it has no particular meaning as a value.
        public order_detail() : this(0, 0, 0, 0, 0) { }

        // interface property
        ITypeSafeDataSet IDataRow.DataSet { get; set; } = null!;
    }
}
