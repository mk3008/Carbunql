using Carbunql.Annotations;

namespace Carbunql.Annotation.Test;

[Table([nameof(author_id)], Schema = "public", Table = "author")]
public record struct Author
{
    [Column(IsAutoNumber = true, ColumnType = "serial8", AutoNumberDefinition = "nextval(author_author_id_seq)")]
    public int author_id { get; set; }

    [Column]
    public string name { get; set; }
}

[Table([nameof(book_id)], Schema = "public", Table = "book")]
public record struct Book
{
    [Column(IsAutoNumber = true, ColumnType = "serial8", AutoNumberDefinition = "nextval(book_book_id_seq)")]
    public int book_id { get; set; }

    [Column]
    public string name { get; set; }

    //[ParentRelationColumn("bigint", nameof(Author.author_id))]
    public Author Author { get; set; }

    //    public static string CommandText =>
    //    """
    //SELECT
    //    t.a_id,
    //    t.b_id,
    //    t.text,
    //    t.value
    //FROM
    //    public.table_data AS t
    //""";

    //    public static SelectQuery<Book> Query => GetQuery();

    //    private static SelectQuery<Book>? QueryCache;

    //    private static SelectQuery<Book> GetQuery()
    //    {
    //        if (QueryCache == null) QueryCache = new SelectQuery<Book>(CommandText);
    //        return QueryCache;
    //    }
}

