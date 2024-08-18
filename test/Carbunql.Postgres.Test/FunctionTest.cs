using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Values;
using Xunit.Abstractions;

namespace Carbunql.Postgres.Test;

public class FunctionTest
{
    private readonly QueryCommandMonitor Monitor;

    public FunctionTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
        Output = output;
    }

    private ITestOutputHelper Output { get; set; }

    [Fact]
    public void AnyTest()
    {
        var sq = new SelectQuery();
        var (from, a) = sq.FromAs<table_a>("a");

        var list = new List<int>() { 1, 2, 3 };
        var array = new int[] { 1, 2, 3 };

        sq.SelectAll();

        sq.Where(() => list.Contains(a.a_id));
        sq.Where(() => !list.Contains(a.a_id));

        sq.Where(() => array.Contains(a.a_id));
        sq.Where(() => !array.Contains(a.a_id));

        Monitor.Log(sq);

        var sql = @"SELECT
    *
FROM
    table_a AS a
WHERE
    a.a_id = ANY(:member_list)
    AND NOT (a.a_id = ANY(:member_list))
    AND a.a_id = ANY(:member_array)
    AND NOT (a.a_id = ANY(:member_array))";

        Assert.Equal(48, sq.GetTokens().ToList().Count);
        Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
    }

    [Fact]
    public void ConcatTest()
    {
        var sq = new SelectQuery();
        var (from, a) = sq.FromAs<table_a>("a");

        sq.Select(() => string.Concat(a.a_id, " ", a.text));
        sq.Select(() => a.a_id + " " + a.text);

        Monitor.Log(sq);

        var sql = @"
SELECT
    CONCAT(a.a_id, ' ', a.text),
    a.a_id || ' ' || a.text
FROM
    table_a AS a";

        Assert.Equal(27, sq.GetTokens().ToList().Count);
        Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
    }

    [Fact]
    public void TrimTest()
    {
        var sq = new SelectQuery();
        var (from, a) = sq.FromAs<table_a>("a");

        sq.Select(() => a.text.Trim());
        sq.Select(() => " value ".Trim());

        Monitor.Log(sq);

        var sql = @"SELECT
    TRIM(a.text),
    :method_trim
FROM
    table_a AS a";

        Assert.Equal(13, sq.GetTokens().ToList().Count);
        Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
    }

    [Fact]
    public void TrimStartTest()
    {
        var sq = new SelectQuery();
        var (from, a) = sq.FromAs<table_a>("a");

        sq.Select(() => a.text.TrimStart());
        sq.Select(() => " value ".TrimStart());

        Monitor.Log(sq);

        var sql = @"SELECT
    LTRIM(a.text),
    :method_trim_start
FROM
    table_a AS a";

        Assert.Equal(13, sq.GetTokens().ToList().Count);
        Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
    }

    [Fact]
    public void TrimEndTest()
    {
        var sq = new SelectQuery();
        var (from, a) = sq.FromAs<table_a>("a");

        sq.Select(() => a.text.TrimEnd());
        sq.Select(() => " value ".TrimEnd());

        Monitor.Log(sq);

        var sql = @"SELECT
    RTRIM(a.text),
    :method_trim_end
FROM
    table_a AS a";

        Assert.Equal(13, sq.GetTokens().ToList().Count);
        Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
    }

    [Fact]
    public void LikeTest_Contains()
    {
        var sq = new SelectQuery();
        var (from, a) = sq.FromAs<table_a>("a");

        var text = "word";

        sq.SelectAll();

        sq.Where(() => a.text.Contains("word"));
        sq.Where(() => a.text.Contains(text));

        Monitor.Log(sq);

        var sql = @"SELECT
    *
FROM
    table_a AS a
WHERE
    a.text LIKE '%' || 'word' || '%'
    AND a.text LIKE '%' || :member_text || '%'";

        Assert.Equal(26, sq.GetTokens().ToList().Count);
        Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
    }

    [Fact]
    public void LikeTest_NotContains()
    {
        var sq = new SelectQuery();
        var (from, a) = sq.FromAs<table_a>("a");

        var text = "word";

        sq.SelectAll();

        sq.Where(() => !a.text.Contains("word"));
        sq.Where(() => !a.text.Contains(text));

        Monitor.Log(sq);

        var sql = @"SELECT
    *
FROM
    table_a AS a
WHERE
    a.text NOT LIKE '%' || 'word' || '%'
    AND a.text NOT LIKE '%' || :member_text || '%'";

        Assert.Equal(28, sq.GetTokens().ToList().Count);
        Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
    }

    [Fact]
    public void LikeTest_StartsWith()
    {
        var sq = new SelectQuery();
        var (from, a) = sq.FromAs<table_a>("a");

        var text = "word";

        sq.SelectAll();

        sq.Where(() => a.text.StartsWith("word"));
        sq.Where(() => a.text.StartsWith(text));

        Monitor.Log(sq);

        var sql = @"SELECT
    *
FROM
    table_a AS a
WHERE
    a.text LIKE 'word' || '%'
    AND a.text LIKE :member_text || '%'";

        Assert.Equal(22, sq.GetTokens().ToList().Count);
        Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
    }

    [Fact]
    public void LikeTest_EndsWith()
    {
        var sq = new SelectQuery();
        var (from, a) = sq.FromAs<table_a>("a");

        var text = "word";

        sq.SelectAll();

        sq.Where(() => a.text.EndsWith("word"));
        sq.Where(() => a.text.EndsWith(text));

        Monitor.Log(sq);

        var sql = @"SELECT
    *
FROM
    table_a AS a
WHERE
    a.text LIKE '%' || 'word'
    AND a.text LIKE '%' || :member_text";

        Assert.Equal(22, sq.GetTokens().ToList().Count);
        Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
    }

    [Fact]
    public void GreatestTest_objectArray()
    {
        var sq = new SelectQuery();
        var (from, a) = sq.FromAs<table_a>("a");
        var b = from.InnerJoinAs<table_b>(b => a.a_id == b.a_id);

        sq.Select(() => sq.Greatest(() => new object[] { 1, a.a_id, b.a_id })).As("max_value");

        Monitor.Log(sq);

        var sql = @"
SELECT
    GREATEST(1, a.a_id, b.a_id) AS max_value
FROM
    table_a AS a
    INNER JOIN table_b AS b ON a.a_id = b.a_id";

        Assert.Equal(31, sq.GetTokens().ToList().Count);
        Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
    }

    [Fact]
    public void GreatestTest()
    {
        var sq = new SelectQuery();
        var (from, a) = sq.FromAs<table_a>("a");
        var b = from.InnerJoinAs<table_b>(b => a.a_id == b.a_id);

        var args = new ValueBase[]
        {
            new LiteralValue(1),
            sq.GetColumn(() => a.a_id),
            sq.GetColumn(() => b.a_id)
        };

        sq.Select(() => sq.Greatest(() => args)).As("max_value");

        Monitor.Log(sq);

        var sql = @"
SELECT
    GREATEST(1, a.a_id, b.a_id) AS max_value
FROM
    table_a AS a
    INNER JOIN table_b AS b ON a.a_id = b.a_id";

        Assert.Equal(31, sq.GetTokens().ToList().Count);
        Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
    }

    [Fact]
    public void LeastTest()
    {
        var sq = new SelectQuery();
        var (from, a) = sq.FromAs<table_a>("a");
        var b = from.InnerJoinAs<table_b>(b => a.a_id == b.a_id);

        var args = new ValueBase[]
        {
            new LiteralValue(1),
            sq.GetColumn(() => a.a_id),
            sq.GetColumn(() => b.a_id)
        };

        sq.Select(() => sq.Least(() => args)).As("min_value");

        Monitor.Log(sq);

        var sql = @"
SELECT
    LEAST(1, a.a_id, b.a_id) AS min_value
FROM
    table_a AS a
    INNER JOIN table_b AS b ON a.a_id = b.a_id";

        Assert.Equal(31, sq.GetTokens().ToList().Count);
        Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
    }

    [Fact]
    public void CoalesceTest()
    {
        var sq = new SelectQuery();
        var (from, a) = sq.FromAs<table_a>("a");
        var b = from.InnerJoinAs<table_b>(b => a.a_id == b.a_id);

        sq.Select(() => a.value ?? b.value ?? 1).As("value");

        Monitor.Log(sq);

        var sql = @"
SELECT
    COALESCE(a.value, b.value, 1) AS value
FROM
    table_a AS a
    INNER JOIN table_b AS b ON a.a_id = b.a_id";

        Assert.Equal(31, sq.GetTokens().ToList().Count);
        Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
    }

    [Fact]
    public void CaseWhenTest()
    {
        var sq = new SelectQuery();
        var (from, a) = sq.FromAs<table_a>("a");
        var b = from.InnerJoinAs<table_b>(b => a.a_id == b.a_id);

        sq.Select(() => a.a_id == 1 ? "x" : b.a_id == 1 ? 'y' : 'z').As("text");

        Monitor.Log(sq);

        var sql = @"
SELECT
    CASE
        WHEN a.a_id = 1 THEN 'x'
        WHEN b.a_id = 1 THEN 'y'
        ELSE 'z'
    END AS text
FROM
    table_a AS a
    INNER JOIN table_b AS b ON a.a_id = b.a_id";

        Assert.Equal(39, sq.GetTokens().ToList().Count);
        Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
    }

    [Fact]
    public void CastTest()
    {
        var sq = new SelectQuery();
        var (from, a) = sq.FromAs<table_a>("a");

        sq.Select(() => Convert.ToChar(a.a_id)).As("to_char");
        sq.Select(() => Convert.ToString(a.a_id)).As("to_string");

        sq.Select(() => Convert.ToByte(a.a_id)).As("to_byte");
        sq.Select(() => Convert.ToSByte(a.a_id)).As("to_sbyte");
        sq.Select(() => Convert.ToBoolean(a.a_id)).As("to_boolean");
        sq.Select(() => Convert.ToDateTime(a.a_id)).As("to_datetime");

        sq.Select(() => Convert.ToInt16(a.a_id)).As("to_int16");
        sq.Select(() => Convert.ToUInt16(a.a_id)).As("to_uint16");
        sq.Select(() => Convert.ToInt32(a.a_id)).As("to_int32");
        sq.Select(() => Convert.ToUInt32(a.a_id)).As("to_uint32");
        sq.Select(() => Convert.ToInt64(a.a_id)).As("to_int64");
        sq.Select(() => Convert.ToUInt64(a.a_id)).As("to_uint64");

        sq.Select(() => Convert.ToSingle(a.a_id)).As("to_single");
        sq.Select(() => Convert.ToDouble(a.a_id)).As("to_double");
        sq.Select(() => Convert.ToDecimal(a.a_id)).As("to_decimal");

        sq.Select(() => (byte)a.a_id).As("cast_byte");
        sq.Select(() => (sbyte)a.a_id).As("cast_sbyte");
        sq.Select(() => (short)a.a_id).As("cast_short");
        sq.Select(() => (ushort)a.a_id).As("cast_ushort");
        sq.Select(() => (int)a.a_id).As("cast_int");
        sq.Select(() => (uint)a.a_id).As("cast_uint");
        sq.Select(() => (long)a.a_id).As("cast_long");
        sq.Select(() => (ulong)a.a_id).As("cast_ulong");
        sq.Select(() => (float)a.a_id).As("cast_float");
        sq.Select(() => (double)a.a_id).As("cast_double");
        sq.Select(() => (decimal)a.a_id).As("cast_decimal");

        Monitor.Log(sq);

        var sql = @"
SELECT
    CAST(a.a_id AS character) AS to_char,
    CAST(a.a_id AS text) AS to_string,
    CAST(a.a_id AS smallint) AS to_byte,
    CAST(a.a_id AS smallint) AS to_sbyte,
    CAST(a.a_id AS boolean) AS to_boolean,
    CAST(a.a_id AS timestamp) AS to_datetime,
    CAST(a.a_id AS smallint) AS to_int16,
    CAST(a.a_id AS integer) AS to_uint16,
    CAST(a.a_id AS integer) AS to_int32,
    CAST(a.a_id AS bigint) AS to_uint32,
    CAST(a.a_id AS bigint) AS to_int64,
    CAST(a.a_id AS numeric) AS to_uint64,
    CAST(a.a_id AS real) AS to_single,
    CAST(a.a_id AS double precision) AS to_double,
    CAST(a.a_id AS numeric) AS to_decimal,
    CAST(a.a_id AS smallint) AS cast_byte,
    CAST(a.a_id AS smallint) AS cast_sbyte,
    CAST(a.a_id AS smallint) AS cast_short,
    CAST(a.a_id AS integer) AS cast_ushort,
    CAST(a.a_id AS integer) AS cast_int,
    CAST(a.a_id AS bigint) AS cast_uint,
    CAST(a.a_id AS bigint) AS cast_long,
    CAST(a.a_id AS numeric) AS cast_ulong,
    CAST(a.a_id AS real) AS cast_float,
    CAST(a.a_id AS double precision) AS cast_double,
    CAST(a.a_id AS numeric) AS cast_decimal
FROM
    table_a AS a";

        Assert.Equal(290, sq.GetTokens().ToList().Count);
        Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
    }

    [Fact]
    public void RowNumberTest()
    {
        var sq = new SelectQuery();
        var (from, a) = sq.FromAs<table_a>("a");

        sq.Select(() => sq.RowNumber(() => new object[] { a.rate }, () => new object[] { a.text, a.a_id })).As("text");

        Monitor.Log(sq);

        var sql = @"
SELECT
    ROW_NUMBER() OVER(
        PARTITION BY
            a.rate
        ORDER BY
            a.text,
            a.a_id
    ) AS text
FROM
    table_a AS a";

        Assert.Equal(25, sq.GetTokens().ToList().Count);
        Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
    }

    [Fact]
    public void RowNumberOrderOnly()
    {
        var sq = new SelectQuery();
        var (from, a) = sq.FromAs<table_a>("a");

        sq.Select(() => sq.RowNumber(() => null, () => new object[] { a.text, a.a_id })).As("text");

        Monitor.Log(sq);

        var sql = @"
SELECT
    ROW_NUMBER() OVER(
        ORDER BY
            a.text,
            a.a_id
    ) AS text
FROM
    table_a AS a";

        Assert.Equal(21, sq.GetTokens().ToList().Count);
        Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
    }

    [Fact]
    public void RowNumberOmit()
    {
        var sq = new SelectQuery();
        var (from, a) = sq.FromAs<table_a>("a");

        sq.Select(() => sq.RowNumber(() => null, () => null)).As("text");

        Monitor.Log(sq);

        var sql = @"
SELECT
    ROW_NUMBER() AS text
FROM
    table_a AS a";

        Assert.Equal(10, sq.GetTokens().ToList().Count);
        Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
    }

    public record struct table_a(int a_id, string text, int? value, bool is_enabled, double rate, DateTime timestamp);

    public record struct table_b(int a_id, string text, int? value, bool is_enabled, double rate, DateTime timestamp);
}
