using Carbunql;

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

        // Convert the selection query to an object
        var sq = new SelectQuery(sql);

        // Dynamically add search conditions
        if (minPrice != null)
        {
            sq.AddWhere("price", (source, column) => $"{source.Alias}.{column} >= {minPrice.Value}");
        }
        if (maxPrice != null)
        {
            sq.AddWhere("price", (source, column) => $"{source.Alias}.{column} <= {maxPrice.Value}");
        }
        if (!string.IsNullOrEmpty(category))
        {
            // Parameterize string values before adding them to search conditions
            var pname = ":category";
            sq.AddParameter(new QueryParameter(pname, category))
                .AddWhere("category", (source, column) => $"{source.Alias}.{column} = {pname}");
        }
        if (inStock != null)
        {
            sq.AddWhere("in_stock", (source, column) => $"{source.Alias}.{column} = {inStock.Value}");
        }
        return sq.ToText();
    }
}
