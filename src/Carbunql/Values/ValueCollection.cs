using Carbunql.Analysis.Parser;
using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;
using System.Collections;
using static System.Net.Mime.MediaTypeNames;

namespace Carbunql.Values;

[MessagePack.MessagePackObject]
public class ValueCollection : ValueBase, IList<ValueBase>, IQueryCommand
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
		if (!columns.Any()) throw new ArgumentException(nameof(columns));
		foreach (var column in columns)
		{
			Collection.Add(new ColumnValue(tableAlias, column));
		}
	}

	[MessagePack.Key(1)]
	private List<ValueBase> Collection { get; init; } = new();

	public IEnumerable<string> GetColumnNames()
	{
		foreach (var item in Collection) yield return item.GetDefaultName();
	}

	internal override IEnumerable<SelectQuery> GetInternalQueriesCore()
	{
		foreach (var value in Collection)
		{
			foreach (var item in value.GetInternalQueries())
			{
				yield return item;
			}
		}
	}

	internal override IEnumerable<PhysicalTable> GetPhysicalTablesCore()
	{
		foreach (var value in Collection)
		{
			foreach (var item in value.GetPhysicalTables())
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

	public override IDictionary<string, object?> GetParameters()
	{
		var prm = EmptyParameters.Get();
		Collection.ForEach(x => prm = prm.Merge(x.GetParameters()));
		return prm;
	}

	public void Add(string value)
	{
		Add(ValueParser.Parse(value));
	}

	#region implements IList<ValueBase>
	[MessagePack.Key(2)]
	public ValueBase this[int index]
	{ get => ((IList<ValueBase>)Collection)[index]; set => ((IList<ValueBase>)Collection)[index] = value; }

	[MessagePack.Key(3)]
	public int Count => ((ICollection<ValueBase>)Collection).Count;

	[MessagePack.Key(4)]
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