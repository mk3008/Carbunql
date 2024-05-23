using Carbunql.Clauses;
using Xunit.Abstractions;

namespace Carbunql.TypeSafe.Test;

public class WhereTest
{
    public WhereTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private ITestOutputHelper Output { get; }

    [Fact]
    public void WhereStaticValue()
    {
        var a = Sql.DefineTable<sale>();

        var query = Sql.From(() => a)
            .Where(() => a.sale_id == 1);

        var actual = query.ToText();
        Output.WriteLine(query.ToText());

        var expect = @"SELECT
    *
FROM
    sale AS a
WHERE
    a.sale_id = CAST(1 AS integer)";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void WhereStaticVariable()
    {
        var a = Sql.DefineTable<sale>();

        var id = 1;

        var query = Sql.From(() => a)
            .Where(() => a.sale_id == id);

        var actual = query.ToText();
        Output.WriteLine(query.ToText());

        var expect = @"/*
  :id = 1
*/
SELECT
    *
FROM
    sale AS a
WHERE
    a.sale_id = CAST(:id AS integer)";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void Multiple()
    {
        var a = Sql.DefineTable<sale>();

        var query = Sql.From(() => a)
            .Where(() => a.quantity == 1)
            .Where(() => a.unit_price == 2);

        var actual = query.ToText();
        Output.WriteLine(query.ToText());

        var expect = @"SELECT
    *
FROM
    sale AS a
WHERE
    a.quantity = 1
    AND a.unit_price = 2";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void AndTest()
    {
        var a = Sql.DefineTable<sale>();

        var query = Sql.From(() => a)
            .Where(() => a.quantity == 1 && a.unit_price == 2);

        var actual = query.ToText();
        Output.WriteLine(query.ToText());

        var expect = @"SELECT
    *
FROM
    sale AS a
WHERE
    a.quantity = 1
    AND a.unit_price = 2";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void OrTest()
    {
        var a = Sql.DefineTable<sale>();

        var query = Sql.From(() => a)
            .Where(() => a.quantity == 1 || a.unit_price == 2);

        var actual = query.ToText();
        Output.WriteLine(query.ToText());

        var expect = @"SELECT
    *
FROM
    sale AS a
WHERE
    ((a.quantity = 1) OR (a.unit_price = 2))";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void BracketTest()
    {
        var a = Sql.DefineTable<sale>();

        var query = Sql.From(() => a)
            .Where(() =>
                (a.quantity == 1 && a.unit_price == 2)
                ||
                (a.quantity == 10 && a.unit_price == 20)
            );

        var actual = query.ToText();
        Output.WriteLine(query.ToText());

        var expect = @"SELECT
    *
FROM
    sale AS a
WHERE
    ((a.quantity = 1 AND a.unit_price = 2) OR (a.quantity = 10 AND a.unit_price = 20))";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void LikeTest()
    {
        var a = Sql.DefineTable<sale>();

        var query = Sql.From(() => a)
            .Where(() => a.product_name.StartsWith("a"))
            .Where(() => a.product_name.Contains("a"))
            .Where(() => a.product_name.EndsWith("a"));

        var actual = query.ToText();
        Output.WriteLine(query.ToText());

        var expect = @"/*
  :p0 = 'a'
*/
SELECT
    *
FROM
    sale AS a
WHERE
    a.product_name LIKE :p0 || '%'
    AND a.product_name LIKE '%' || :p0 || '%'
    AND a.product_name LIKE :p0 || '%'";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void InTest()
    {
        var idArray = new List<int>() { 1, 2, 3, 4 };

        var a = Sql.DefineTable<sale>();

        var query = Sql.From(() => a)
            .Where(() => idArray.Contains(a.sale_id!.Value));

        var actual = query.ToText();
        Output.WriteLine(query.ToText());

        var expect = @"SELECT
    *
FROM
    sale AS a
WHERE
    a.sale_id IN (1, 2, 3, 4)";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void AnyTest()
    {
        var idArray = new List<int>() { 1, 2, 3, 4 };

        var a = Sql.DefineTable<sale>();

        var query = Sql.From(() => a)
            .Where(() => idArray.Any(x => a.sale_id!.Value == x));

        var actual = query.ToText();
        Output.WriteLine(query.ToText());

        var expect = @"/*
  :id_array = System.Collections.Generic.List`1[System.Int32]
*/
SELECT
    *
FROM
    sale AS a
WHERE
    a.sale_id = ANY(:id_array)";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void AnyTest_Right()
    {
        var idArray = new List<int>() { 1, 2, 3, 4 };

        var a = Sql.DefineTable<sale>();

        var query = Sql.From(() => a)
            .Where(() => idArray.Any(x => x == a.sale_id!.Value));

        var actual = query.ToText();
        Output.WriteLine(query.ToText());

        var expect = @"/*
  :id_array = System.Collections.Generic.List`1[System.Int32]
*/
SELECT
    *
FROM
    sale AS a
WHERE
    a.sale_id = ANY(:id_array)";

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
        CreateTableQuery ITableRowDefinition.CreateTableQuery { get; set; } = null!;
    }
}