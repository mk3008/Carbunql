using Carbunql;
using Carbunql.Fluent;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Enter minimum price (or leave blank to omit):");
        string? minPriceInput = Console.ReadLine();
        decimal? minPrice = string.IsNullOrEmpty(minPriceInput) ? null : Convert.ToDecimal(minPriceInput);

        Console.WriteLine("Enter maximum price (or leave blank to omit):");
        string? maxPriceInput = Console.ReadLine();
        decimal? maxPrice = string.IsNullOrEmpty(maxPriceInput) ? null : Convert.ToDecimal(maxPriceInput);

        Console.WriteLine("Enter category (or leave blank to omit):");
        string? category = Console.ReadLine();

        Console.WriteLine("Enter in-stock status (true/false) (or leave blank to omit):");
        string? inStockInput = Console.ReadLine();
        bool? inStock = string.IsNullOrEmpty(inStockInput) ? null : Convert.ToBoolean(inStockInput);

        var query = GenerateProductQuery(minPrice, maxPrice, category, inStock);
        Console.WriteLine("Generated SQL Query:");
        Console.WriteLine(query);
    }

    private static string GenerateProductQuery(decimal? minPrice, decimal? maxPrice, string? category, bool? inStock)
    {
        var sql = """
    SELECT
        p.product_id,
        p.product_name,
        p.price,
        p.category,
        p.in_stock
    FROM
        product as p
    """;

        var pname = ":category";

        // Convert the selection query to an object
        var sq = SelectQuery.Parse(sql)
            .GreaterThanOrEqualIfNotNullOrEmpty("price", minPrice)
            .LessThanOrEqualIfNotNullOrEmpty("price", maxPrice)
            .AddParameter(pname, category)
            .EqualIfNotNullOrEmpty("category", pname)
            .EqualIfNotNullOrEmpty("in_stock", inStock);

        return sq.ToText();
    }
}
