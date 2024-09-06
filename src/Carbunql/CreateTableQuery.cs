using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Definitions;
using Carbunql.Tables;
using Carbunql.Values;
using System.Data;

namespace Carbunql;

/// <summary>
/// Represents a query for creating a table.
/// </summary>
public class CreateTableQuery : IQueryCommandable, ICommentable, ITable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateTableQuery"/> class with the specified schema and table name.
    /// </summary>
    /// <param name="schema">The schema name.</param>
    /// <param name="table">The table name.</param>
    public CreateTableQuery(string schema, string table)
    {
        Schema = schema;
        Table = table;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateTableQuery"/> class with the specified table name.
    /// </summary>
    /// <param name="table">The table name.</param>
    public CreateTableQuery(string table)
    {
        Table = table;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateTableQuery"/> class with the specified table interface.
    /// </summary>
    /// <param name="t">The table interface.</param>
    public CreateTableQuery(ITable t)
    {
        Schema = t.Schema;
        Table = t.Table;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateTableQuery"/> class with the specified table definition clause.
    /// </summary>
    /// <param name="clause">The table definition clause.</param>
    public CreateTableQuery(TableDefinitionClause clause)
    {
        Schema = clause.Schema;
        Table = clause.Table;
        DefinitionClause = clause;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the table is temporary.
    /// </summary>
    public bool IsTemporary { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether the table has the "if not exists" clause.
    /// </summary>
    public bool HasIfNotExists { get; set; } = false;

    /// <summary>
    /// Gets or sets the schema of the table.
    /// </summary>
    public string Schema { get; init; } = string.Empty;

    /// <summary>
    /// Gets the name of the table.
    /// </summary>
    public string Table { get; init; }

    /// <summary>
    /// Gets or sets the table definition clause.
    /// </summary>
    public TableDefinitionClause? DefinitionClause { get; set; } = null;

    /// <summary>
    /// Gets or sets the query.
    /// </summary>
    public IReadQuery? Query { get; set; }

    /// <summary>
    /// Gets or sets the parameters.
    /// </summary>
    public IEnumerable<QueryParameter>? Parameters { get; set; }

    /// <summary>
    /// Gets or sets the comment clause for the table.
    /// </summary>
    public CommentClause? CommentClause { get; set; }

    /// <summary>
    /// Gets internal queries.
    /// </summary>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        if (Query != null)
        {
            foreach (var item in Query.GetInternalQueries())
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Gets physical tables.
    /// </summary>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        if (Query != null)
        {
            foreach (var item in Query.GetPhysicalTables())
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Gets query parameters.
    /// </summary>
    public virtual IEnumerable<QueryParameter> GetParameters()
    {
        if (Parameters != null)
        {
            foreach (var item in Parameters)
            {
                yield return item;
            }
        }

        if (Query != null)
        {
            foreach (var item in Query.GetParameters())
            {
                yield return item;
            }
        }
    }
    /// <summary>
    /// Gets the token representing the create table operation.
    /// </summary>
    /// <param name="parent">The parent token.</param>
    /// <returns>The token representing the create table operation.</returns>
    private Token GetCreateTableToken(Token? parent)
    {
        if (IsTemporary)
        {
            return Token.Reserved(this, parent, "create temporary table");
        }
        return Token.Reserved(this, parent, "create table");
    }

    /// <summary>
    /// Gets the tokens representing the create table query.
    /// </summary>
    /// <param name="parent">The parent token.</param>
    /// <returns>The tokens representing the create table query.</returns>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        if (CommentClause != null) foreach (var item in CommentClause.GetTokens(parent)) yield return item;

        var ct = GetCreateTableToken(parent);
        yield return ct;

        if (HasIfNotExists)
        {
            yield return Token.Reserved(this, parent, "if not exists");
        }

        yield return new Token(this, parent, this.GetTableFullName());

        if (Query != null)
        {
            var t = new Token(this, parent, "as", isReserved: true);
            yield return t;

            foreach (var item in Query.GetTokens())
            {
                yield return item;
            }
            yield break;
        }

        if (DefinitionClause != null)
        {
            foreach (var item in DefinitionClause.GetTokens(parent))
            {
                yield return item;
            }
            yield break;
        }

        throw new InvalidOperationException();
    }

    /// <summary>
    /// Gets the common tables.
    /// </summary>
    /// <returns>The common tables.</returns>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        if (Query != null)
        {
            foreach (var item in Query.GetCommonTables())
            {
                yield return item;
            }
            yield break;
        }
    }

    /// <summary>
    /// Converts the CreateTableQuery instance to a SelectQuery instance.
    /// </summary>
    /// <returns>The SelectQuery instance.</returns>
    public SelectQuery ToSelectQuery()
    {
        if (string.IsNullOrEmpty(this.GetTableFullName())) throw new NullReferenceException(nameof(Table));

        if (Query != null)
        {
            var sq = new SelectQuery();
            var (_, t) = sq.From(this.GetTableFullName()).As("t");

            foreach (var item in Query.GetColumnNames())
            {
                sq.Select(t, item);
            }

            return sq;
        }

        if (DefinitionClause != null)
        {
            var sq = new SelectQuery();
            var (_, t) = sq.From(this.GetTableFullName()).As("t");

            foreach (var column in DefinitionClause.OfType<ColumnDefinition>())
            {
                sq.Select(t, column.ColumnName);
            }

            return sq;
        }

        throw new InvalidOperationException();
    }

    /// <summary>
    /// Converts the CreateTableQuery instance to a SelectQuery instance representing a count query.
    /// </summary>
    /// <param name="alias">The alias for the count.</param>
    /// <returns>The SelectQuery instance representing a count query.</returns>
    public SelectQuery ToCountQuery(string alias = "row_count")
    {
        if (string.IsNullOrEmpty(this.GetTableFullName())) throw new NullReferenceException(nameof(Table));

        var sq = new SelectQuery();
        sq.From(this.GetTableFullName()).As("q");
        sq.Select("count(*)").As(alias);
        return sq;
    }

    /// <summary>
    /// Converts the CreateTableQuery instance to a DefinitionQuerySet instance to normalize the query.
    /// </summary>
    /// <returns>The DefinitionQuerySet instance.</returns>
    public DefinitionQuerySet ToNormalize()
    {
        if (IsTemporary) throw new InvalidOperationException();
        if (Query != null) throw new InvalidOperationException();
        if (DefinitionClause == null) throw new InvalidOperationException();

        //create table
        var ct = new CreateTableQuery(this);
        ct.DefinitionClause = DefinitionClause.ToNormalize();

        var queryset = new DefinitionQuerySet(ct);

        // alter table query normalize
        foreach (var item in DefinitionClause.Disassemble())
        {
            if (!item.TrySet(ct.DefinitionClause))
            {
                queryset.AddAlterTableQuery(item);
            }
        }

        return queryset;
    }

    public IEnumerable<ColumnValue> GetColumns()
    {
        yield break;
    }
}
