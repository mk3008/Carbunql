using Carbunql.Annotations;
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
    public void OmitSelectClause()
    {
        var sd = Sql.DefineDataSet<StoreData>();

        var query = Sql.From(() => sd);

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    *
FROM
    StoreData AS sd";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void SelectAll()
    {
        var sd = Sql.DefineDataSet<StoreData>();

        var query = Sql.From(() => sd)
            .Select(() => sd);

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    sd.StoreDataId,
    sd.StoreName
FROM
    StoreData AS sd";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void SelectColumn()
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
    sd.StoreDataId,
    sd.StoreName
FROM
    StoreData AS sd";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void TableNameFromAttribute()
    {
        var s = Sql.DefineDataSet<Store>();

        var query = Sql.From(() => s);

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    *
FROM
    public.stores AS s";

        Assert.Equal(expect, actual, true, true, true);
    }

    public record StoreData : IDataRow
    {
        public int StoreDataId { get; set; }
        public string StoreName { get; set; } = string.Empty;
        // interface property
        ITypeSafeDataSet IDataRow.DataSet { get; set; } = null!;
    }

    [Table([nameof(store_id)], Schema = "public", Table = "stores")]
    public record Store : IDataRow
    {
        public int store_id { get; set; }
        public string store_name { get; set; } = string.Empty;
        // interface property
        ITypeSafeDataSet IDataRow.DataSet { get; set; } = null!;
    }
}