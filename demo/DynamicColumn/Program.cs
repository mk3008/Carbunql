using Carbunql;
using Carbunql.Fluent;

public class Program
{
    public static void Main()
    {
        // Define available columns
        var availableColumns = new Dictionary<string, string>
        {
            { "1", "customer_name" },
            { "2", "email" },
            { "3", "purchase_history" }
        };

        Console.WriteLine("Available columns to select:");
        foreach (var column in availableColumns)
        {
            Console.WriteLine($"{column.Key}: {column.Value}");
        }

        Console.WriteLine("Enter the numbers of the columns you want to include, separated by commas (e.g., 1,2):");
        string? input = Console.ReadLine();
        var selectedColumnNumbers = input?.Split(',', StringSplitOptions.RemoveEmptyEntries);

        var selectedColumns = new List<string>();
        if (selectedColumnNumbers != null)
        {
            foreach (var number in selectedColumnNumbers)
            {
                if (availableColumns.TryGetValue(number.Trim(), out var column))
                {
                    selectedColumns.Add(column);
                }
            }
        }

        var query = GenerateCustomReportQuery(selectedColumns);
        Console.WriteLine("Generated SQL Query:");
        Console.WriteLine(query);

        var sq = new SelectQuery()
            .From("customer", "c")
            .Select("c", "customer_id")
            .SelectValue("c.first_name || c.last_name", "customer_name");
        Console.WriteLine(sq.ToText());
    }

    public static string GenerateCustomReportQuery(List<string> columns)
    {
        var sql = """
            SELECT
                customer_name,
                email,
                purchase_history
            FROM
                customer
            """;

        // Convert the query to an object
        var sq = SelectQuery.Parse(sql);

        // Restrict the selected columns
        sq.FilterInColumns(columns);

        // Convert the query to text format
        return sq.ToText();
    }
}
