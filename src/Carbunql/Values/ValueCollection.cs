using Carbunql.Analysis.Parser;
using Carbunql.Clauses;
using Carbunql.Tables;
using MessagePack;
using System.Collections;

namespace Carbunql.Values;

[MessagePackObject(keyAsPropertyName: true)]
public class ValueCollection : ValueBase, IList<ValueBase>, IQueryCommandable
{
	public ValueCollection()
	{
	}

	public ValueCollection(string text)
	{
		Collection.Add(new LiteralValue(text));
	}

	public ValueCollection(ValueBase item)
	{
		Collection.Add(item);
	}

	public ValueCollection(List<ValueBase> collection)
	{
		Collection.AddRange(collection);
	}

	public ValueCollection(IEnumerable<string> values)
	{
		foreach (var item in values)
		{
			Collection.Add(new LiteralValue(item));
		}
	}

	public ValueCollection(string tableAlias, IEnumerable<string> columns)
	{
		if (!columns.Any()) throw new ArgumentException(null, nameof(columns));
		foreach (var column in columns)
		{
			Collection.Add(new ColumnValue(tableAlias, column));
		}
	}

	private List<ValueBase> Collection { get; init; } = new();

	public IEnumerable<string> GetColumnNames()
	{
		foreach (var item in Collection) yield return item.GetDefaultName();
	}

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

	public void Add(string value)
	{
		Add(ValueParser.Parse(value));
	}

	public void Add(FromClause from, string column)
	{
		Add(from.Root.Alias, column);
	}

	public void Add(SelectableTable table, string column)
	{
		Add(table.Alias, column);
	}

	public void Add(string table, string column)
	{
		var v = new ColumnValue(table, column);
		Add(v);
	}

	public void Add(int value)
	{
		var v = new LiteralValue(value.ToString());
		Add(v);
	}

	public void Add(long value)
	{
		var v = new LiteralValue(value.ToString());
		Add(v);
	}

	public void Add(decimal value)
	{
		var v = new LiteralValue(value.ToString());
		Add(v);
	}

	public void Add(double value)
	{
		var v = new LiteralValue(value.ToString());
		Add(v);
	}

	public void Add(DateTime value, string sufix = "::timestamp")
	{
		Add("'" + value.ToString() + "'" + sufix);
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