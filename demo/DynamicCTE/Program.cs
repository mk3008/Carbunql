using Carbunql;
using Carbunql.Fluent;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Which month to summarize? (yyyy-mm-dd)");
        DateTime summaryMonth = Convert.ToDateTime(Console.ReadLine());

        Console.WriteLine("Include monthly summary rows? (true/false)");
        bool includeMonthly = Convert.ToBoolean(Console.ReadLine());

        var query = GenerateReportQuery(includeMonthly, summaryMonth);
        Console.WriteLine("Generated SQL Query:");
        Console.WriteLine(query);
    }

    public static string GenerateReportQuery(bool includeSummary, DateTime summaryMonth)
    {
        string dailySummaryQuery = """
            SELECT
                sale_date
                , sum(amount) AS amount_total
                , '' as caption 
                , 1 as sort_number
            FROM
                salse
            GROUP BY
                sale_date
            """;

        string monthlySummaryQuery = """
            SELECT
                date_trunc('month', sale_date) + '1 month -1 day' as sale_date
                , sum(amount) AS amount_total
                , 'monthly total' as caption 
                , 2 as sort_number
            FROM
                salse
            GROUP BY
                date_trunc('month', sale_date) + '1 month -1 day'
            """;

        // Create daily summary query
        var sq = new SelectQuery()
            .With(dailySummaryQuery, "daily_summary")
            .From("daily_summary", "d")
            .SelectAll("d");

        if (includeSummary)
        {
            // Add monthly summary query with UNION ALL
            sq.UnionAll(() =>
            {
                var xsq = new SelectQuery()
                    .With(monthlySummaryQuery, "monthly_summary")
                    .From("monthly_summary", "m")
                    .SelectAll("m");
                return xsq;
            });
        }

        // Add date filter condition
        var saleDate = ":sale_date";
        sq.AddParameter(saleDate, summaryMonth)
            .BetweenInclusiveStart("sale_date", saleDate, $"{saleDate}::timestamp + '1 month'");

        // Convert the entire query to a CTE
        sq = sq.ToCTEQuery("final", "f");

        // Add sorting conditions
        sq.RemoveSelect("sort_number")
            .OrderBy("sale_date")
            .OrderBy("sort_number");

        return sq.ToText();
    }
}
