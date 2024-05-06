using Carbunql.Annotations;
using Xunit.Abstractions;

namespace Carbunql.Annotation.Test;

public class ParentRelationModelTest
{
    public ParentRelationModelTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
        Output = output;
    }

    private QueryCommandMonitor Monitor { get; }

    private ITestOutputHelper Output { get; }

    [Fact]
    public void CreateTableTest()
    {
        var q = CreateTableQueryFactory.Create<Book>();

        Monitor.Log(q);

        var expect = @"CREATE TABLE book (
    book_id bigserial NOT NULL,
    name text NOT NULL,
    author_id bigint NOT NULL,
    CONSTRAINT book_pkey PRIMARY KEY (book_id)
)";

        Assert.Equal(expect, q.ToText(), true, true, true);
    }

    [Fact]
    public void QuerySetTest()
    {
        var q = DefinitionQuerySetFactory.Create<Book>();

        Output.WriteLine(q.ToText());

        var expect = @"CREATE TABLE book (
    book_id bigserial NOT NULL,
    name text NOT NULL,
    author_id bigint NOT NULL
)
;
ALTER TABLE book
    ADD CONSTRAINT book_pkey PRIMARY KEY (book_id)
;
CREATE INDEX idx_author ON book (
    author_id
)
;
";

        Assert.Equal(expect, q.ToText(), true, true, true);
    }

    //[Table([nameof(author_id)], Schema = "public", Table = "author")]
    public record struct Author
    {
        //[Column(IsAutoNumber = true, ColumnType = "serial8", AutoNumberDefinition = "nextval(author_author_id_seq)")]
        public long author_id { get; set; }

        //[Column]
        public string name { get; set; }
    }

    //[Table([nameof(book_id)], Schema = "public", Table = "book")]
    public record struct Book
    {
        //[Column(IsAutoNumber = true, ColumnType = "serial8", AutoNumberDefinition = "nextval(book_book_id_seq)")]
        public long book_id { get; set; }

        //[Column]
        public string name { get; set; }

        [ParentRelationColumn(nameof(Author.author_id))]
        public Author Author { get; set; }
    }
}
