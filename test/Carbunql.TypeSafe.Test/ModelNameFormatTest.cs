using Xunit.Abstractions;

namespace Carbunql.TypeSafe.Test;

public class ModelNameFormatTest
{

    public ModelNameFormatTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private ITestOutputHelper Output { get; }

    [Fact]
    public void SnakeCase_OmitSelectClause()
    {
        var sd = Sql.DefineDataSet<StoreData>();

        var query = Sql.From(() => sd);

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    *
FROM
    store_data AS sd";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void SnakeCase_SelectAll()
    {
        var sd = Sql.DefineDataSet<StoreData>();

        var query = Sql.From(() => sd)
            .Select(() => sd);

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    sd.store_data_id,
    sd.store_name
FROM
    store_data AS sd";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void SnakeCase_SelectColumn()
    {
        var sd = Sql.DefineDataSet<StoreData>();

        var query = Sql.From(() => sd)
            .Select(() => new
            {
                sd.StoreDataId,
                sd.StoreName,
            });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    sd.store_data_id,
    sd.store_name
FROM
    store_data AS sd";

        Assert.Equal(expect, actual, true, true, true);
    }

    public record StoreData : IDataRow
    {
        public int StoreDataId { get; set; }
        public string StoreName { get; set; } = string.Empty;
        // interface property
        IDataSet IDataRow.DataSet { get; set; } = null!;
    }

}