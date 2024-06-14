using Carbunql;
using Carbunql.TypeSafe;

internal class Program
{
    private static void Main(string[] args)
    {
        // The easiest way to use
        Console.WriteLine(SelectSale().ToText());
        Console.WriteLine(";");

        Console.WriteLine(SelectFilterdSale().ToText());
        Console.WriteLine(";");

        Console.WriteLine(SelectFilterdSaleWithTestData().ToText());
        Console.WriteLine(";");

        // Select Sale Data
        Console.WriteLine(SelectSaleTestDataQuery().ToText());
        Console.WriteLine(";");

        // Debug: Add a column to Sale data with tax calculations
        Console.WriteLine(SelectSaleWithTaxQuery().ToText());
        Console.WriteLine(";");

        // Debug: Aggregate by tax rate
        Console.WriteLine(SelectTaxSummary().ToText());
        Console.WriteLine(";");

        // If there is a discrepancy between the tax total in the details and the total by tax rate, allocate it proportionally
        Console.WriteLine(SelectSaleReportQuery().ToText());
        Console.WriteLine(";");

        // Debug
        var query = SelectSaleReportQuery();
        var sale = SelectSaleTestDataQuery();
        query.With(() => sale);

        Console.WriteLine(query.ToText());
        Console.WriteLine(";");
    }


    private static FluentSelectQuery<Sale> SelectSale()
    {
        // select * from sale
        var s = Sql.DefineDataSet<Sale>();
        return Sql.From(() => s);
    }

    private static FluentSelectQuery<Sale> SelectFilterdSale()
    {
        // Give the query a name
        var all_sale = SelectSale;
        // Define the query as a dataset and give it an alias name
        var sl = Sql.DefineDataSet(() => all_sale());

        return Sql.From(() => sl)
            .Where(() => sl.sale_id == 1);
    }


    private static FluentSelectQuery<Sale> SelectSaleTestDataQuery()
    {
        return new FluentSelectQuery<Sale>([
            new Sale{sale_id = 1, product_name = "apple", unit_price = 105, quantity = 5, tax_rate = 0.07},
            new Sale{sale_id = 2, product_name = "orange", unit_price = 203, quantity = 3, tax_rate = 0.07},
            new Sale{sale_id = 3, product_name = "banana", unit_price = 233, quantity = 9, tax_rate = 0.07},
            new Sale{sale_id = 4, product_name = "tea", unit_price = 309, quantity = 7, tax_rate = 0.08},
            new Sale{sale_id = 5, product_name = "coffee", unit_price = 555, quantity = 9, tax_rate = 0.08},
            new Sale{sale_id = 6, product_name = "cola", unit_price = 456, quantity = 2, tax_rate = 0.08},
        ]).Comment("test data");
    }

    private static FluentSelectQuery<Sale> SelectFilterdSaleWithTestData()
    {

        var query = SelectFilterdSale();

        // Important: Give your test data the same name as your table name
        var sale = SelectSaleTestDataQuery();

        // Add test data to the WITH clause
        query.With(() => sale);

        return query;
    }


    private static FluentSelectQuery<SaleWithTax> SelectSaleWithTaxQuery()
    {
        var s = Sql.DefineDataSet<Sale>();

        var getSubQuery = () => Sql.From(() => s)
            .Select(() => s)
            .Select(() => new SaleWithTax
            {
                line_tax_truncated = (int)Math.Truncate(s.unit_price * s.quantity * s.tax_rate),
                line_tax_raw = s.unit_price * s.quantity * s.tax_rate
            }).Compile<SaleWithTax>(force: true);

        var q = Sql.DefineDataSet(() => getSubQuery());

        var query = Sql.From(() => q)
            .Comment("Calculate tax on a line-by-line basis")
            .Select(() => q)
            .Select(() => new SaleWithTax
            {
                adjust_priority = Sql.RowNumber(
                        () => new
                        {
                            q.tax_rate,
                        },
                        () => new
                        {
                            desc = q.line_tax_raw - q.line_tax_truncated,
                            q.sale_id
                        }
                )
            });

        return query.Compile<SaleWithTax>();
    }

    private static FluentSelectQuery<TaxSummary> SelectTaxSummary()
    {
        /*
        Guidelines:
        Decide whether to use a CTE or a subquery based on whether the processing is indivisible.

        Specific Example:
        Store queries obtained from outside the function in a CTE.
        Use subqueries for those obtained within the function.
        */

        // Use the dataset with added tax calculation columns
        // Store in a variable to use as a CTE
        var sale_with_tax = SelectSaleWithTaxQuery();
        var swt = Sql.DefineDataSet(() => sale_with_tax);

        // Define a subquery function for two-stage aggregation
        // Use a subquery instead of a variable to avoid redundancy, even though a variable would create a CTE
        // Force Compile because there are columns that cannot be computed in the first stage
        // Recompute dummy values in the second stage, so forcing is acceptable
        var getSubQuery = () => Sql.From(() => swt)
            .Select(() => new TaxSummary
            {
                tax_rate = swt.tax_rate,
                total_tax = (int)Math.Truncate(Sql.Sum(() => swt.line_tax_raw)),
                line_tax_trancated_summary = Sql.Sum(() => swt.line_tax_truncated),
            })
            .GroupBy(() => new
            {
                swt.tax_rate
            }).Compile<TaxSummary>(force: true);

        // Perform the second stage of processing
        // Use the first stage's results to express the tax difference
        var q = Sql.DefineDataSet(() => getSubQuery());

        var query = Sql.From(() => q)
            .Comment("Calculate tax by tax rate")
            .Select(() => q)
            .Select(() => new TaxSummary
            {
                tax_difference = q.total_tax - q.line_tax_trancated_summary
            });

        return query.Compile<TaxSummary>();
    }


    private static FluentSelectQuery<SaleReport> SelectSaleReportQuery()
    {
        var sale_with_tax = SelectSaleWithTaxQuery();
        var swt = Sql.DefineDataSet(() => sale_with_tax);

        var tax_summary = SelectTaxSummary();
        var ts = Sql.DefineDataSet(() => tax_summary);

        var getSubQuery = () => Sql.From(() => swt)
            .InnerJoin(() => ts, () => swt.tax_rate == ts.tax_rate)
            .Select(() => swt)
            .Select(() => new SaleReport
            {
                tax_adjustment = (swt.adjust_priority <= ts.tax_difference) ? 1 : 0
            }).Compile<SaleReport>(force: true);

        var q = Sql.DefineDataSet(() => getSubQuery());

        var query = Sql.From(() => q)
            .Comment("If the tax amount per line does not match the tax amount per tax rate, it will be adjusted.\r\nThe priority for adjustments will be in descending order of the rounded down fraction.")
            .Select(() => q)
            .Select(() => new SaleReport
            {
                tax = q.line_tax_truncated + q.tax_adjustment
            });

        return query.Compile<SaleReport>(); ;
    }

    private record SaleWithTax : Sale
    {
        public int line_tax_truncated { get; set; }
        public double line_tax_raw { get; set; }
        public int adjust_priority { get; set; }
    }

    private record TaxSummary : IDataRow
    {
        public double tax_rate { get; set; }
        public int total_tax { get; set; }
        public int line_tax_trancated_summary { get; set; }
        public int tax_difference { get; set; }
        public IDataSet DataSet { get; set; } = null!;
    }

    private record SaleReport : SaleWithTax
    {
        public int tax { get; set; }

        public int tax_adjustment { get; set; }
    }
}

public record Sale : IDataRow
{
    public long sale_id { get; set; }
    public string product_name { get; set; } = null!;
    public int unit_price { get; set; }
    public int quantity { get; set; }
    public double tax_rate { get; set; }

    public IDataSet DataSet { get; set; } = null!;
}
