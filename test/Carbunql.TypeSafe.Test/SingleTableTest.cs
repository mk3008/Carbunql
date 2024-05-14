using Carbunql.Clauses;
using System.Diagnostics;
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

    // SQL任意関数RAW（引数に列や定数を使いたいこともある、current_timestmap）、
    // 変換(case)、
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

    [Fact]
    public void CSharpFunction_Operator()
    {
        var a = Sql.DefineTable<sale>();

        var query = Sql.From(() => a)
            .Select(() => new
            {
                v_add = a.unit_price + a.sale_id,
                v_subtract = a.unit_price - a.sale_id,
                v_multiply = a.unit_price * a.sale_id,
                v_divide = a.unit_price / a.sale_id,
                v_modulo = a.unit_price % a.sale_id,
                tax = a.unit_price * a.quantity * (decimal)0.1
            });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    CAST(a.unit_price AS numeric) + CAST(a.sale_id AS numeric) AS v_add,
    CAST(a.unit_price AS numeric) - CAST(a.sale_id AS numeric) AS v_subtract,
    CAST(a.unit_price AS numeric) * CAST(a.sale_id AS numeric) AS v_multiply,
    CAST(a.unit_price AS numeric) / CAST(a.sale_id AS numeric) AS v_divide,
    CAST(a.unit_price AS numeric) % CAST(a.sale_id AS numeric) AS v_modulo,
    a.unit_price * CAST(a.quantity AS numeric) * CAST(0.1 AS numeric) AS tax
FROM
    sale AS a";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void CSharpFunction_Math()
    {
        var a = Sql.DefineTable<sale>();

        var query = Sql.From(() => a)
            .Select(() => new
            {
                v_truncate = Math.Truncate(a.unit_price),
                v_floor = Math.Floor(a.unit_price),
                v_ceiling = Math.Ceiling(a.unit_price),
                v_round_arg1 = Math.Round(a.unit_price),
                v_round_arg2 = Math.Round(a.unit_price, 2),
                test = Math.Truncate(a.unit_price * a.quantity),
            });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    TRUNC(a.unit_price) AS v_truncate,
    FLOOR(a.unit_price) AS v_floor,
    CEIL(a.unit_price) AS v_ceiling,
    ROUND(a.unit_price) AS v_round_arg1,
    ROUND(a.unit_price, CAST(2 AS integer)) AS v_round_arg2,
    TRUNC(a.unit_price * CAST(a.quantity AS numeric)) AS test
FROM
    sale AS a";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void RawCommand()
    {
        var a = Sql.DefineTable<sale>();

        var query = Sql.From(() => a)
            .Select(() => new
            {
                rawcommand = Sql.Raw("current_timestamp")
            });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    current_timestamp AS rawcommand
FROM
    sale AS a";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void ReservedCommand()
    {
        var a = Sql.DefineTable<sale>();

        var query = Sql.From(() => a)
            .Select(() => new
            {
                now_command = Sql.Now,
                timestamp_commend = Sql.CurrentTimestamp,
                row_num = Sql.RowNumber(),
                row_num_partiton_order = Sql.RowNumber(new { a.product_name, a.unit_price }, new { a.quantity, a.sale_id }),
                row_num_partition = Sql.RowNumberPartitionBy(new { a.product_name, a.unit_price }),
                row_num_order = Sql.RowNumberOrderbyBy(new { a.product_name, a.unit_price })
            });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    NOW() AS now_command,
    current_timestamp AS timestamp_commend,
    ROW_NUMBER() OVER() AS row_num,
    ROW_NUMBER() OVER(
        PARTITION BY
            a.product_name,
            a.unit_price
        ORDER BY
            a.quantity,
            a.sale_id
    ) AS row_num_partiton_order,
    ROW_NUMBER() OVER(
        PARTITION BY
            a.product_name,
            a.unit_price
    ) AS row_num_partition,
    ROW_NUMBER() OVER(
        ORDER BY
            a.product_name,
            a.unit_price
    ) AS row_num_order
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