using Carbunql.Extensions;
using Carbunql.Tables;
using MessagePack;
using System.Collections;

namespace Carbunql.Clauses;

[MessagePackObject(keyAsPropertyName: true)]
public class WindowClause : IList<NamedWindowDefinition>, IQueryCommandable
{
	public WindowClause()
	{
	}

	public WindowClause(IList<NamedWindowDefinition> definitions)
	{
		NamedWindowDefinitions.AddRange(definitions);
	}

	public List<NamedWindowDefinition> NamedWindowDefinitions { get; private set; } = new();

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		foreach (var definition in NamedWindowDefinitions)
		{
			foreach (var item in definition.GetInternalQueries())
			{
				yield return item;
			}
		}
	}

	public IDictionary<string, object?> GetParameters()
	{
		var prm = EmptyParameters.Get();
		foreach (var item in NamedWindowDefinitions)
		{
			prm = prm.Merge(item.GetParameters());
		}
		return prm;
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		foreach (var definition in NamedWindowDefinitions)
		{
			foreach (var item in definition.GetPhysicalTables())
			{
				yield return item;
			}
		}
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		if (!NamedWindowDefinitions.Any()) yield break;

		var clause = Token.Reserved(this, parent, "window");
		yield return clause;

		var isFisrt = true;
		foreach (var item in NamedWindowDefinitions)
		{
			if (isFisrt)
			{
				isFisrt = false;
			}
			else
			{
				yield return Token.Comma(this, clause);
			}
			foreach (var token in item.GetTokens(clause)) yield return token;
		}
	}

	#region implements IList<NamedWindowDefinition>

	public NamedWindowDefinition this[int index] { get => ((IList<NamedWindowDefinition>)NamedWindowDefinitions)[index]; set => ((IList<NamedWindowDefinition>)NamedWindowDefinitions)[index] = value; }

	public int Count => ((ICollection<NamedWindowDefinition>)NamedWindowDefinitions).Count;

	public bool IsReadOnly => ((ICollection<NamedWindowDefinition>)NamedWindowDefinitions).IsReadOnly;

	public void Add(NamedWindowDefinition item)
	{
		((ICollection<NamedWindowDefinition>)NamedWindowDefinitions).Add(item);
	}

	public void Clear()
	{
		((ICollection<NamedWindowDefinition>)NamedWindowDefinitions).Clear();
	}

	public bool Contains(NamedWindowDefinition item)
	{
		return ((ICollection<NamedWindowDefinition>)NamedWindowDefinitions).Contains(item);
	}

	public void CopyTo(NamedWindowDefinition[] array, int arrayIndex)
	{
		((ICollection<NamedWindowDefinition>)NamedWindowDefinitions).CopyTo(array, arrayIndex);
	}

	public IEnumerator<NamedWindowDefinition> GetEnumerator()
	{
		return ((IEnumerable<NamedWindowDefinition>)NamedWindowDefinitions).GetEnumerator();
	}

	public int IndexOf(NamedWindowDefinition item)
	{
		return ((IList<NamedWindowDefinition>)NamedWindowDefinitions).IndexOf(item);
	}

	public void Insert(int index, NamedWindowDefinition item)
	{
		((IList<NamedWindowDefinition>)NamedWindowDefinitions).Insert(index, item);
	}

	public bool Remove(NamedWindowDefinition item)
	{
		return ((ICollection<NamedWindowDefinition>)NamedWindowDefinitions).Remove(item);
	}

	public void RemoveAt(int index)
	{
		((IList<NamedWindowDefinition>)NamedWindowDefinitions).RemoveAt(index);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable)NamedWindowDefinitions).GetEnumerator();
	}

	#endregion
}
