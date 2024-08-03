using Carbunql;

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
        var sq = new SelectQuery();
        sq.AddCTEQuery(dailySummaryQuery, "daily_summary");
        sq.AddFrom("daily_summary", "d");
        sq.AddSelectAll("d");

        if (includeSummary)
        {
            // Add monthly summary query with UNION ALL
            sq.AddSelectQuery("union all", _ =>
            {
                var xsq = new SelectQuery();
                xsq.AddCTEQuery(monthlySummaryQuery, "monthly_summary");
                xsq.AddFrom("monthly_summary", "m");
                xsq.AddSelectAll("m");
                return xsq;
            });
        }

        // Add date filter condition
        var pname = ":sale_date";
        sq.AddParameter(new QueryParameter(pname, summaryMonth))
            .AddWhere("sale_date", (source, column) => $"{pname} <= {source.Alias}.{column} and {source.Alias}.{column} < {pname}::timestamp + '1 month'");

        // Convert the entire query to a CTE
        sq = sq.ToCTEQuery("final", "f");

        // Add sorting conditions
        sq.RemoveSelect("sort_number")
            .AddOrder("sale_date", (source, column) => $"{source.Alias}.{column}")
            .AddOrder("sort_number", (source, column) => $"{source.Alias}.{column}");

        return sq.ToText();
    }
}
