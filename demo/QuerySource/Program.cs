using Carbunql;
using Carbunql.Building;

var sql = """
    select
        s.sale_id
        , s.store_id
        , s.sale_date as journal_date
        , s.price as journal_price
        , s.request_timestamp
    from
        sales as s
    """;

var lower_limit = new DateTime(2024, 7, 20);

var query = SelectQuery.Parse(sql)
    .OverrideSelect("journal_date", (source, item) => $"greatest({item}, {source.Query.AddParameter(":lower_limit", lower_limit)})")
    .AddNotExists(["sale_id"], "sale_journals")
    .AddWhere("request_timestamp", (source) => $"{source.Alias}.request_timestamp >= :lower_limit")
    .ToCTEQuery("final", "f")
    .ToInsertQuery("sale_journals");

Console.WriteLine(query.ToText());

var fixedsql = """
    select
        s.sale_id
        , s.store_id
        , s.sale_date as journal_date
        , s.price + s.tax as journal_price
        , s.request_timestamp
    from
        sales as s
    """;

var modDate = new DateTime(2024, 7, 1);
var modQuery = new SelectQuery()
    // Define expected values (current values) as a CTE named 'expect'
    .AddCTEQuery("""
        select distinct on(sale_id) 
            sale_id
            , store_id
            , journal_date
            , journal_price
        from
            sale_journals
        where
            journal_price > 0
        order by
            sale_id
            , sale_journal_id desc
    """, "expect")
    // Define the correct values as a CTE named 'actual'
    .AddCTEQuery(fixedsql, "actual")
    // Compare the expected and correct values, and format the differences with red/black formatting
    .AddFrom("expect", "exp")
    .AddJoin("inner join", "actual", "act", "exp.sale_id = act.sale_id")
    .AddWhere("exp.journal_price <> act.journal_price")
    .OverrideSelect("journal_date", (source, item) => $"greatest({item}, {source.Query.AddParameter(":mod_date", modDate)})")
    .AddSelectAll("exp")
    .RemoveSelect("journal_price")
    .AddSelect("exp.journal_price * -1", "reverse_price")
    .AddSelect("act.journal_price * +1", "collect_price")
    // Define the query result with red/black formatting as a CTE named 'diff' and select it
    .ToCTEQuery("diff", "r")
    // Since we want to process red entries, the collect_price value is unnecessary; use reverse_price as the journal value
    .RemoveSelect("collect_price")
    .RenameSelect("reverse_price", "journal_price")
    // To process black entries, generate a UNION ALL query
    .AddSelectQuery("union all", owner =>
    {
        return new SelectQuery()
            // Since we want to use the CTE 'diff', import the CTE information
            .ImportCTEQueries(owner)
            // Define the black entry selection query, similar to the red entry
            .AddFrom("diff", "c")
            .AddSelectAll("c")
            .RemoveSelect("reverse_price")
            .RenameSelect("collect_price", "journal_price");
    })
    // Convert to an insert query
    .ToInsertQuery("sale_journals");




Console.WriteLine(modQuery.ToText());


Console.WriteLine();
Console.WriteLine();
Console.WriteLine();
Console.WriteLine();
Console.WriteLine();
Console.WriteLine();
Console.WriteLine();