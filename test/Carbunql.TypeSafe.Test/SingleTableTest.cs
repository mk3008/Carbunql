using Carbunql.Clauses;
using Xunit.Abstractions;

namespace Carbunql.TypeSafe.Test;

public class SingleTableTest
{
    public SingleTableTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private ITestOutputHelper Output { get; }

    [Fact]
    public void SelectTest()
    {
        var a = Sql.DefineTable<sale>();

        var query = Sql.From(() => a)
            .Select(() => new
            {
                a.sale_id,
            });

        var actual = query.ToText();
        Output.WriteLine(query.ToText());

        var expect = @"SELECT
    a.sale_id
FROM
    sale AS a";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void AliasTest()
    {
        var a = Sql.DefineTable<sale>();

        var query = Sql.From(() => a)
            .Select(() => new
            {
                id = a.sale_id,
                a.unit_price,
                a.product_name
            });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    a.sale_id AS id,
    a.unit_price,
    a.product_name
FROM
    sale AS a";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void LiteralTest()
    {
        var a = Sql.DefineTable<sale>();

        var query = Sql.From(() => a)
            .Select(() => new
            {
                id = 1,
                value = (long)10,
                rate = (decimal)0.1,
                tf_value = true,
                remarks = "test",
                created_at = new DateTime(2000, 1, 1, 10, 10, 0)
            });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"/*
  :p0 = 'test'
  :p1 = 2000/01/01 10:10:00
*/
SELECT
    CAST(1 AS integer) AS id,
    CAST(10 AS bigint) AS value,
    CAST(0.1 AS numeric) AS rate,
    CAST(True AS boolean) AS tf_value,
    :p0 AS remarks,
    :p1 AS created_at
FROM
    sale AS a";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void VariableTest()
    {
        var a = Sql.DefineTable<sale>();

        var id = 1;
        var value = (long)10;
        var rate = (decimal)0.1;
        var remarks = "test";
        var tf_value = true;
        var created_at = new DateTime(2000, 1, 1, 10, 10, 0);

        var query = Sql.From(() => a)
            .Select(() => new
            {
                id,
                value,
                rate,
                tf_value,
                remarks,
                created_at
            });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"/*
  :p0 = 'test'
  :p1 = 2000/01/01 10:10:00
*/
SELECT
    CAST(1 AS integer) AS id,
    CAST(10 AS bigint) AS value,
    CAST(0.1 AS numeric) AS rate,
    CAST(True AS boolean) AS tf_value,
    :p0 AS remarks,
    :p1 AS created_at
FROM
    sale AS a";

        Assert.Equal(expect, actual, true, true, true);
    }

    // 演算、SQL任意関数RAW（引数に列や定数を使いたいこともある、current_timestmap）、
    // 変換(coalesce, case)、
    // 条件(like, exists)、

    [Fact]
    public void CSharpFunction_Coalesce()
    {
        var a = Sql.DefineTable<sale>();

        var query = Sql.From(() => a)
            .Select(() => new
            {
                value = a.sale_id ?? 0,
                text = a.product_name ?? ""
            });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    COALESCE(a.sale_id, CAST(0 AS integer)) AS value,
    COALESCE(a.product_name, '') AS text
FROM
    sale AS a";

        Assert.Equal(expect, actual, true, true, true);
    }

    public record sale(
        int? sale_id,
        string product_name,
        int quantity,
        decimal unit_price
    ) : ITableRowDefinition
    {
        // no arguments constructor.
        // Since it is used as a definition, it has no particular meaning as a value.
        public sale() : this(0, "", 0, 0) { }

        // interface property
        TableDefinitionClause ITableRowDefinition.TableDefinition { get; set; } = null!;
    }
}