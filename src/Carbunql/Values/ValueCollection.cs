using Carbunql.Analysis.Parser;
using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Tables;
using MessagePack;
using System.Collections;

namespace Carbunql.Values;

/// <summary>
/// Represents a collection of values.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class ValueCollection : ValueBase, IList<ValueBase>, IQueryCommandable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValueCollection"/> class.
    /// </summary>
    public ValueCollection()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueCollection"/> class with the specified text.
    /// </summary>
    /// <param name="text">The text value to be added to the collection.</param>
    public ValueCollection(string text)
    {
        Collection.Add(new LiteralValue(text));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueCollection"/> class with the specified item.
    /// </summary>
    /// <param name="item">The value to be added to the collection.</param>
    public ValueCollection(ValueBase item)
    {
        Collection.Add(item);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueCollection"/> class with the specified collection.
    /// </summary>
    /// <param name="collection">The collection of values to be added.</param>
    public ValueCollection(List<ValueBase> collection)
    {
        Collection.AddRange(collection);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueCollection"/> class with the specified values.
    /// </summary>
    /// <param name="values">The values to be added to the collection.</param>
    public ValueCollection(IEnumerable<string> values)
    {
        foreach (var item in values)
        {
            Collection.Add(new LiteralValue(item));
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueCollection"/> class with the specified table alias and columns.
    /// </summary>
    /// <param name="tableAlias">The alias of the table.</param>
    /// <param name="columns">The columns to be added to the collection.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="columns"/> is empty.</exception>
    public ValueCollection(string tableAlias, IEnumerable<string> columns)
    {
        if (!columns.Any()) throw new ArgumentException(null, nameof(columns));
        foreach (var column in columns)
        {
            Collection.Add(new ColumnValue(tableAlias, column));
        }
    }

    private List<ValueBase> Collection { get; init; } = new();

    /// <summary>
    /// Gets the column names from the collection.
    /// </summary>
    /// <returns>The column names.</returns>
    public IEnumerable<string> GetColumnNames()
    {
        foreach (var item in Collection) yield return item.GetDefaultName();
    }

    /// <inheritdoc/>
    protected override IEnumerable<SelectQuery> GetInternalQueriesCore()
    {
        foreach (var value in Collection)
        {
            foreach (var item in value.GetInternalQueries())
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    protected override IEnumerable<PhysicalTable> GetPhysicalTablesCore()
    {
        foreach (var value in Collection)
        {
            foreach (var item in value.GetPhysicalTables())
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    protected override IEnumerable<CommonTable> GetCommonTablesCore()
    {
        foreach (var value in Collection)
        {
            foreach (var item in value.GetCommonTables())
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        var isFirst = true;
        foreach (var item in Collection)
        {
            if (isFirst)
            {
                isFirst = false;
            }
            else
            {
                yield return Token.Comma(this, parent);
            }
            foreach (var token in item.GetTokens(parent)) yield return token;
        }
    }
    /// <inheritdoc/>
    protected override IEnumerable<QueryParameter> GetParametersCore()
    {
        foreach (var item in Collection)
        {
            foreach (var p in item.GetParameters())
            {
                yield return p;
            }
        }
    }

    /// <inheritdoc/>
    public override IEnumerable<ValueBase> GetValues()
    {
        foreach (var item in this)
        {
            foreach (var value in item.GetValues())
            {
                yield return value;
            }
        }
    }

    /// <summary>
    /// Adds a string value to the collection.
    /// </summary>
    /// <param name="value">The string value to add.</param>
    public void Add(string value)
    {
        Add(ValueParser.Parse(value));
    }

    /// <summary>
    /// Adds a column value to the collection.
    /// </summary>
    /// <param name="from">The FROM clause.</param>
    /// <param name="column">The column name.</param>
    public void Add(FromClause from, string column)
    {
        Add(from.Root.Alias, column);
    }

    /// <summary>
    /// Adds a column value to the collection.
    /// </summary>
    /// <param name="table">The table alias.</param>
    /// <param name="column">The column name.</param>
    public void Add(SelectableTable table, string column)
    {
        Add(table.Alias, column);
    }

    /// <summary>
    /// Adds a column value to the collection.
    /// </summary>
    /// <param name="table">The table name.</param>
    /// <param name="column">The column name.</param>
    public void Add(string table, string column)
    {
        var v = new ColumnValue(table, column);
        Add(v);
    }

    /// <summary>
    /// Adds an integer value to the collection.
    /// </summary>
    /// <param name="value">The integer value to add.</param>
    public void Add(int value)
    {
        var v = new LiteralValue(value.ToString());
        Add(v);
    }

    /// <summary>
    /// Adds a long integer value to the collection.
    /// </summary>
    /// <param name="value">The long integer value to add.</param>
    public void Add(long value)
    {
        var v = new LiteralValue(value.ToString());
        Add(v);
    }

    /// <summary>
    /// Adds a decimal value to the collection.
    /// </summary>
    /// <param name="value">The decimal value to add.</param>
    public void Add(decimal value)
    {
        var v = new LiteralValue(value.ToString());
        Add(v);
    }

    /// <summary>
    /// Adds a double value to the collection.
    /// </summary>
    /// <param name="value">The double value to add.</param>
    public void Add(double value)
    {
        var v = new LiteralValue(value.ToString());
        Add(v);
    }

    /// <summary>
    /// Adds a DateTime value to the collection.
    /// </summary>
    /// <param name="value">The DateTime value to add.</param>
    /// <param name="sufix">The optional suffix.</param>
    public void Add(DateTime value, string sufix = "::timestamp")
    {
        Add("'" + value.ToString() + "'" + sufix);
    }

    /// <summary>
    /// Converts the collection to a plain SelectQuery.
    /// </summary>
    /// <param name="columnAlias">The list of column aliases.</param>
    /// <returns>A plain SelectQuery.</returns>
    public SelectQuery ToPlainSelectQuery(IList<string> columnAlias)
    {
        var sq = new SelectQuery();
        sq.SelectClause ??= new();

        var index = 0;
        foreach (var column in this)
        {
            sq.SelectClause!.Add(column.ToSelectable(columnAlias[index]));
            index++;
        }
        return sq;
    }

    #region implements IList<ValueBase>
    public ValueBase this[int index]
    { get => ((IList<ValueBase>)Collection)[index]; set => ((IList<ValueBase>)Collection)[index] = value; }

    public int Count => ((ICollection<ValueBase>)Collection).Count;

    public bool IsReadOnly => ((ICollection<ValueBase>)Collection).IsReadOnly;

    public void Add(ValueBase item)
    {
        ((ICollection<ValueBase>)Collection).Add(item);
    }

    public void Clear()
    {
        ((ICollection<ValueBase>)Collection).Clear();
    }

    public bool Contains(ValueBase item)
    {
        return ((ICollection<ValueBase>)Collection).Contains(item);
    }

    public void CopyTo(ValueBase[] array, int arrayIndex)
    {
        ((ICollection<ValueBase>)Collection).CopyTo(array, arrayIndex);
    }

    public IEnumerator<ValueBase> GetEnumerator()
    {
        return ((IEnumerable<ValueBase>)Collection).GetEnumerator();
    }

    public int IndexOf(ValueBase item)
    {
        return ((IList<ValueBase>)Collection).IndexOf(item);
    }

    public void Insert(int index, ValueBase item)
    {
        ((IList<ValueBase>)Collection).Insert(index, item);
    }

    public bool Remove(ValueBase item)
    {
        return ((ICollection<ValueBase>)Collection).Remove(item);
    }

    public void RemoveAt(int index)
    {
        ((IList<ValueBase>)Collection).RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Collection).GetEnumerator();
    }
    #endregion
}