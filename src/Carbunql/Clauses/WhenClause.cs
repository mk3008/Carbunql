using Carbunql.Extensions;
using Carbunql.Tables;
using System.Collections;

namespace Carbunql.Clauses;

public class WhenClause : IList<MergeCondition>, IQueryCommandable
{
	public List<MergeCondition> Conditions { get; set; } = new();

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		foreach (var condition in Conditions)
		{
			foreach (var item in condition.GetInternalQueries())
			{
				yield return item;
			}
		}
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		foreach (var condition in Conditions)
		{
			foreach (var item in condition.GetPhysicalTables())
			{
				yield return item;
			}
		}
	}

	public IDictionary<string, object?> GetParameters()
	{
		var prm = EmptyParameters.Get();
		foreach (var item in Conditions)
		{
			prm = prm.Merge(item.GetParameters());
		}
		return prm;
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		foreach (var condition in Conditions)
		{
			foreach (var item in condition.GetTokens(parent)) yield return item;
		}
	}

	#region implements IList<MergeCondition>
	public MergeCondition this[int index] { get => ((IList<MergeCondition>)Conditions)[index]; set => ((IList<MergeCondition>)Conditions)[index] = value; }

	public int Count => ((ICollection<MergeCondition>)Conditions).Count;

	public bool IsReadOnly => ((ICollection<MergeCondition>)Conditions).IsReadOnly;

	public void Add(MergeCondition item)
	{
		((ICollection<MergeCondition>)Conditions).Add(item);
	}

	public void Clear()
	{
		((ICollection<MergeCondition>)Conditions).Clear();
	}

	public bool Contains(MergeCondition item)
	{
		return ((ICollection<MergeCondition>)Conditions).Contains(item);
	}

	public void CopyTo(MergeCondition[] array, int arrayIndex)
	{
		((ICollection<MergeCondition>)Conditions).CopyTo(array, arrayIndex);
	}

	public IEnumerator<MergeCondition> GetEnumerator()
	{
		return ((IEnumerable<MergeCondition>)Conditions).GetEnumerator();
	}

	public int IndexOf(MergeCondition item)
	{
		return ((IList<MergeCondition>)Conditions).IndexOf(item);
	}

	public void Insert(int index, MergeCondition item)
	{
		((IList<MergeCondition>)Conditions).Insert(index, item);
	}

	public bool Remove(MergeCondition item)
	{
		return ((ICollection<MergeCondition>)Conditions).Remove(item);
	}

	public void RemoveAt(int index)
	{
		((IList<MergeCondition>)Conditions).RemoveAt(index);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable)Conditions).GetEnumerator();
	}
	#endregion
}