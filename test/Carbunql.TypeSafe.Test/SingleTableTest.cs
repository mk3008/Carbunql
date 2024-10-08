using Carbunql.TypeSafe.Extensions;
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
    public void SelectAllTest()
    {
        var a = Sql.DefineDataSet<sale>();

        var query = Sql.From(() => a);

        var actual = query.ToText();
        Output.WriteLine(query.ToText());

        var expect = @"SELECT
    *
FROM
    sale AS a";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void SelectTest()
    {
        var a = Sql.DefineDataSet<sale>();

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
        var a = Sql.DefineDataSet<sale>();

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
        var a = Sql.DefineDataSet<sale>();

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

        var expect = @"SELECT
    1 AS id,
    10 AS value,
    0.1 AS rate,
    True AS tf_value,
    :const_0 AS remarks,
    :const_1 AS created_at
FROM
    sale AS a";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void BracketTest()
    {
        var a = Sql.DefineDataSet<sale>();

        var query = Sql.From(() => a)
            .Select(() => new
            {
                value1 = (a.unit_price + a.unit_price) * 3,
                value2 = a.unit_price + (a.unit_price * 3)
            });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    (a.unit_price + a.unit_price) * 3 AS value1,
    a.unit_price + a.unit_price * 3 AS value2
FROM
    sale AS a";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void VariableTest()
    {
        var a = Sql.DefineDataSet<sale>();

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

        var expect = @"SELECT
    :id AS id,
    :value AS value,
    :rate AS rate,
    :tf_value AS tf_value,
    :remarks AS remarks,
    :created_at AS created_at
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
        var a = Sql.DefineDataSet<sale>();

        var query = Sql.From(() => a)
            .Select(() => new
            {
                value = a.sale_id ?? 0,
                text = a.product_name ?? ""
            });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    COALESCE(a.sale_id, 0) AS value,
    COALESCE(a.product_name, '') AS text
FROM
    sale AS a";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void CSharpFunction_Operator()
    {
        var a = Sql.DefineDataSet<sale>();

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
    a.unit_price * CAST(a.quantity AS numeric) * 0.1 AS tax
FROM
    sale AS a";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void DatetimeTest_SqlCommand()
    {
        var a = Sql.DefineDataSet<sale>();

        var d = new DateTime(2000, 10, 20);

        var query = Sql.From(() => a)
            .Select(() => new
            {
                v_trunc_year = Sql.DateTruncateToYear(d),
                v_trunc_quarter = Sql.DateTruncateToQuarter(d),
                v_trunc_month = Sql.DateTruncToMonth(d),
                v_trunc_day = Sql.DateTruncateToDay(d),
                v_trunc_hour = Sql.DateTruncateToHour(d),
                v_trunc_minute = Sql.DateTruncateToMinute(d),
                v_trunc_second = Sql.DateTruncateToSecond(d),
                last_date_of_month = Sql.DateTruncToMonth(Sql.Now).AddMonths(1).AddDays(-1),
            });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    DATE_TRUNC('year', :d) AS v_trunc_year,
    DATE_TRUNC('quarter', :d) AS v_trunc_quarter,
    DATE_TRUNC('month', :d) AS v_trunc_month,
    DATE_TRUNC('day', :d) AS v_trunc_day,
    DATE_TRUNC('hour', :d) AS v_trunc_hour,
    DATE_TRUNC('minute', :d) AS v_trunc_minute,
    DATE_TRUNC('second', :d) AS v_trunc_second,
    DATE_TRUNC('month', NOW()) + 1 * INTERVAL '1 month' + -1 * INTERVAL '1 day' AS last_date_of_month
FROM
    sale AS a";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void DatetimeTest_SqlExtension()
    {
        var a = Sql.DefineDataSet<sale>();

        var d = new DateTime(2000, 10, 20);

        var query = Sql.From(() => a)
            .Select(() => new
            {
                v_trunc_year = d.TruncateToYear(),
                v_trunc_quarter = d.TruncateToQuarter(),
                v_trunc_month = d.TruncateToMonth(),
                v_trunc_day = d.TruncateToDay(),
                v_trunc_hour = d.TruncateToHour(),
                v_trunc_minute = d.TruncateToMinute(),
                v_trunc_second = d.TruncateToSecond(),
                last_date_of_month = Sql.Now.TruncateToMonth().AddMonths(1).AddDays(-1),
                last_date_of_month2 = Sql.Now.ToMonthEndDate(),
            });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    DATE_TRUNC('year', :d) AS v_trunc_year,
    DATE_TRUNC('quarter', :d) AS v_trunc_quarter,
    DATE_TRUNC('month', :d) AS v_trunc_month,
    DATE_TRUNC('day', :d) AS v_trunc_day,
    DATE_TRUNC('hour', :d) AS v_trunc_hour,
    DATE_TRUNC('minute', :d) AS v_trunc_minute,
    DATE_TRUNC('second', :d) AS v_trunc_second,
    DATE_TRUNC('month', NOW()) + 1 * INTERVAL '1 month' + -1 * INTERVAL '1 day' AS last_date_of_month,
    DATE_TRUNC('month', NOW()) + INTERVAL '1 month - 1 day' AS last_date_of_month2
FROM
    sale AS a";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void CSharpFunction_Math()
    {
        var a = Sql.DefineDataSet<sale>();

        var query = Sql.From(() => a)
            .Select(() => new
            {
                v_truncate = Math.Truncate(a.unit_price),
                v_floor = Math.Floor(a.unit_price),
                v_ceiling = Math.Ceiling(a.unit_price),
                v_round_arg1 = Math.Round(a.unit_price),
                v_round_arg2 = Math.Round(a.unit_price, 2),
                test1 = Math.Truncate(a.unit_price * a.quantity),
                test2 = Math.Truncate(a.unit_price + a.quantity),
            });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    TRUNC(a.unit_price) AS v_truncate,
    FLOOR(a.unit_price) AS v_floor,
    CEIL(a.unit_price) AS v_ceiling,
    ROUND(a.unit_price) AS v_round_arg1,
    ROUND(a.unit_price, 2) AS v_round_arg2,
    TRUNC(a.unit_price * CAST(a.quantity AS numeric)) AS test1,
    TRUNC(a.unit_price + CAST(a.quantity AS numeric)) AS test2
FROM
    sale AS a";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void RawCommand()
    {
        var a = Sql.DefineDataSet<sale>();

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
    public void Now_Timestamp()
    {
        var a = Sql.DefineDataSet<sale>();

        var query = Sql.From(() => a)
            .Select(() => new
            {
                now_command = Sql.Now,
                timestamp_commend = Sql.CurrentTimestamp,
            });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    NOW() AS now_command,
    current_timestamp AS timestamp_commend
FROM
    sale AS a";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void Trinomial()
    {
        var a = Sql.DefineDataSet<sale>();

        var query = Sql.From(() => a)
            .Select(() => new
            {
                v_equal = a.sale_id == 1 ? 0 : 1,
                v_not_equal = a.sale_id != 1 ? 0 : 1,
                v_gt = a.sale_id < 1 ? 0 : 1,
                v_ge = a.sale_id <= 1 ? 0 : 1,
                v_lt = a.sale_id > 1 ? 0 : 1,
                v_le = a.sale_id >= 1 ? 0 : 1,
            });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    CASE
        WHEN a.sale_id = CAST(1 AS integer) THEN 0
        ELSE 1
    END AS v_equal,
    CASE
        WHEN a.sale_id <> CAST(1 AS integer) THEN 0
        ELSE 1
    END AS v_not_equal,
    CASE
        WHEN a.sale_id < CAST(1 AS integer) THEN 0
        ELSE 1
    END AS v_gt,
    CASE
        WHEN a.sale_id <= CAST(1 AS integer) THEN 0
        ELSE 1
    END AS v_ge,
    CASE
        WHEN a.sale_id > CAST(1 AS integer) THEN 0
        ELSE 1
    END AS v_lt,
    CASE
        WHEN a.sale_id >= CAST(1 AS integer) THEN 0
        ELSE 1
    END AS v_le
FROM
    sale AS a";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void Trinomial_When()
    {
        var a = Sql.DefineDataSet<sale>();

        var query = Sql.From(() => a)
            .Select(() => new
            {
                v_nest = a.sale_id == 1 ? 10 :
                         a.sale_id == 2 ? 20 :
                         a.sale_id == 3 ? 30 :
                         99,
            });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    CASE
        WHEN a.sale_id = CAST(1 AS integer) THEN 10
        WHEN a.sale_id = CAST(2 AS integer) THEN 20
        WHEN a.sale_id = CAST(3 AS integer) THEN 30
        ELSE 99
    END AS v_nest
FROM
    sale AS a";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void Trinomial_Nest()
    {
        var a = Sql.DefineDataSet<sale>();

        var query = Sql.From(() => a)
            .Select(() => new
            {
                v_nest = a.sale_id == 1 ? a.unit_price == 10 ? 11 :
                                          a.unit_price == 20 ? 21 :
                                          91 :
                         a.sale_id == 2 ? a.unit_price == 10 ? 12 :
                                          a.unit_price == 20 ? 22 :
                                          92 :
                         a.sale_id == 3 ? a.unit_price == 10 ? 31 :
                                          a.unit_price == 10 ? 32 :
                                          93 :
                         99,
            });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    CASE
        WHEN a.sale_id = CAST(1 AS integer) THEN CASE
            WHEN a.unit_price = 10 THEN 11
            WHEN a.unit_price = 20 THEN 21
            ELSE 91
        END
        WHEN a.sale_id = CAST(2 AS integer) THEN CASE
            WHEN a.unit_price = 10 THEN 12
            WHEN a.unit_price = 20 THEN 22
            ELSE 92
        END
        WHEN a.sale_id = CAST(3 AS integer) THEN CASE
            WHEN a.unit_price = 10 THEN 31
            WHEN a.unit_price = 10 THEN 32
            ELSE 93
        END
        ELSE 99
    END AS v_nest
FROM
    sale AS a";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void TrimTest()
    {
        var a = Sql.DefineDataSet<sale>();

        var query = Sql.From(() => a)
            .Select(() => new
            {
                v_start_trim = a.product_name.TrimStart(),
                v_trim = a.product_name.Trim(),
                v_end_trim = a.product_name.TrimEnd(),
            });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    LTRIM(a.product_name) AS v_start_trim,
    TRIM(a.product_name) AS v_trim,
    RTRIM(a.product_name) AS v_end_trim
FROM
    sale AS a";

        Assert.Equal(expect, actual, true, true, true);
    }

    [Fact]
    public void ToStringTest()
    {
        var a = Sql.DefineDataSet<sale>();

        var query = Sql.From(() => a)
            .Select(() => new
            {
                product_name = a.product_name.ToString(),
                sale_id = a.sale_id.ToString(),
                created_at = a.created_at.ToString("yyyy/MM/dd HH:mm:ss.ffffff"),
                created_at2 = a.created_at.ToString("yyyy/MM/dd HH:mm:ss.fff"),
            });

        var actual = query.ToText();
        Output.WriteLine(actual);

        var expect = @"SELECT
    CAST(a.product_name AS text) AS product_name,
    CAST(a.sale_id AS text) AS sale_id,
    TO_CHAR(a.created_at, :const_0) AS created_at,
    TO_CHAR(a.created_at, :const_1) AS created_at2
FROM
    sale AS a";

        Assert.Equal(expect, actual, true, true, true);
    }

    //    //procrastinate
    //    //[Fact]
    //    public void Switch()
    //    {
    //        var a = Sql.DefineTable<sale>();

    //        var query = Sql.From(() => a)
    //            .Select(() => new
    //            {
    //                value = switch (a.sale_id)
    //        {
    //            default:
    //                1;
    //        }
    //    });

    //        var actual = query.ToText();
    //    Output.WriteLine(actual);

    //        var expect = @"SELECT
    //    *
    //FROM
    //    sale AS a";

    //    Assert.Equal(expect, actual, true, true, true);
    //    }

    /*   //procrastinate
       //[Fact]
       public void SelectAll()
       {
           var a = Sql.DefineTable<sale>();

           var query = Sql.From(() => a)
               .Select(() => new { });

           var actual = query.ToText();
           Output.WriteLine(actual);

           var expect = @"SELECT
       *
   FROM
       sale AS a";

           Assert.Equal(expect, actual, true, true, true);
       }

       //procrastinate
       //[Fact]
       public void SelectTableAll()
       {
           var a = Sql.DefineTable<sale>();

           var query = Sql.From(() => a)
               .Select(() => new { a });

           var actual = query.ToText();
           Output.WriteLine(actual);

           var expect = @"SELECT
       a.sale_id,
       a.product_name,
       a.quantity,
       a.unit_price
   FROM
       sale AS a";

           Assert.Equal(expect, actual, true, true, true);
       }*/

    public record sale(
        int? sale_id,
        string product_name,
        int quantity,
        decimal unit_price,
        DateTime created_at
    ) : IDataRow
    {
        // no arguments constructor.
        // Since it is used as a definition, it has no particular meaning as a value.
        public sale() : this(0, "", 0, 0, DateTime.Now) { }

        // interface property
        IDataSet IDataRow.DataSet { get; set; } = null!;
    }
}