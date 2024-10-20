namespace Carbunql.Formatting.Test
{
    public class LeadingCommaTokenFormattingLogicTest
    {
        [Fact]
        public void Test()
        {
            CommandTextBuilder.FORMATTER = new LeadingCommaTokenFormattingLogic();

            var sq = SelectQuery.Parse("""
                with
                dat(line_id, name, unit_price, quantity, tax_rate) as ( 
                    values
                    (1, 'apple' , 105, 5, 0.07),
                    (2, 'orange', 203, 3, 0.07),
                    (3, 'banana', 233, 9, 0.07),
                    (4, 'tea'   , 309, 7, 0.08),
                    (5, 'coffee', 555, 9, 0.08),
                    (6, 'cola'  , 456, 2, 0.08)
                ),
                detail as (
                    select  
                        q.*,
                        trunc(q.price * (1 + q.tax_rate)) - q.price as tax,
                        q.price * (1 + q.tax_rate) - q.price as raw_tax
                    from
                        (
                            select
                                dat.*,
                                (dat.unit_price * dat.quantity) as price
                            from
                                dat
                        ) q
                ), 
                tax_summary as (
                    select
                        d.tax_rate,
                        trunc(sum(raw_tax)) as total_tax
                    from
                        detail d
                    group by
                        d.tax_rate
                )
                select 
                   line_id,
                    name,
                    unit_price,
                    quantity,
                    tax_rate,
                    price,
                    price + tax as tax_included_price,
                    tax
                from
                    (
                        select
                            line_id,
                            name,
                            unit_price,
                            quantity,
                            tax_rate,
                            price,
                            tax + adjust_tax as tax
                        from
                            (
                                select
                                    q.*,
                                    case when q.total_tax - q.cumulative >= q.priority then 1 else 0 end as adjust_tax
                                from
                                    (
                                        select  
                                            d.*, 
                                            s.total_tax,
                                            sum(d.tax) over (partition by d.tax_rate) as cumulative,
                                            row_number() over (partition by d.tax_rate order by d.raw_tax % 1 desc, d.line_id) as priority
                                        from
                                            detail d
                                            inner join tax_summary s on d.tax_rate = s.tax_rate
                                    ) q
                            ) q
                    ) q
                order by 
                    line_id
                """);

            var actual = sq.ToText();
            var expect = """
                WITH
                    dat (
                        line_id, name, unit_price, quantity, tax_rate
                    ) AS (
                        VALUES
                            (1, 'apple', 105, 5, 0.07)
                            , (2, 'orange', 203, 3, 0.07)
                            , (3, 'banana', 233, 9, 0.07)
                            , (4, 'tea', 309, 7, 0.08)
                            , (5, 'coffee', 555, 9, 0.08)
                            , (6, 'cola', 456, 2, 0.08)
                    )
                    , detail AS (
                        SELECT
                            q.*
                            , TRUNC(q.price * (1 + q.tax_rate)) - q.price AS tax
                            , q.price * (1 + q.tax_rate) - q.price AS raw_tax
                        FROM
                            (
                                SELECT
                                    dat.*
                                    , (dat.unit_price * dat.quantity) AS price
                                FROM
                                    dat
                            ) AS q
                    )
                    , tax_summary AS (
                        SELECT
                            d.tax_rate
                            , TRUNC(SUM(raw_tax)) AS total_tax
                        FROM
                            detail AS d
                        GROUP BY
                            d.tax_rate
                    )
                SELECT
                    line_id
                    , name
                    , unit_price
                    , quantity
                    , tax_rate
                    , price
                    , price + tax AS tax_included_price
                    , tax
                FROM
                    (
                        SELECT
                            line_id
                            , name
                            , unit_price
                            , quantity
                            , tax_rate
                            , price
                            , tax + adjust_tax AS tax
                        FROM
                            (
                                SELECT
                                    q.*
                                    , CASE
                                        WHEN q.total_tax - q.cumulative >= q.priority THEN 1
                                        ELSE 0
                                    END AS adjust_tax
                                FROM
                                    (
                                        SELECT
                                            d.*
                                            , s.total_tax
                                            , SUM(d.tax) OVER(
                                                PARTITION BY
                                                    d.tax_rate
                                            ) AS cumulative
                                            , ROW_NUMBER() OVER(
                                                PARTITION BY
                                                    d.tax_rate
                                                ORDER BY
                                                    d.raw_tax % 1 DESC
                                                    , d.line_id
                                            ) AS priority
                                        FROM
                                            detail AS d
                                            INNER JOIN tax_summary AS s ON d.tax_rate = s.tax_rate
                                    ) AS q
                            ) AS q
                    ) AS q
                ORDER BY
                    line_id
                """;

            Assert.Equal(expect, actual);
        }
    }
}