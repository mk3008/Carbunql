using Carbunql.Extensions;
using System.Collections;

namespace Carbunql.Clauses;

public class WithClause : IList<CommonTable>, IQueryCommandable
{
	public WithClause()
	{
	}

	public WithClause(IList<CommonTable> commons)
	{
		CommonTables.AddRange(commons);
	}

	public List<CommonTable> CommonTables { get; private set; } = new();

	public bool HasRecursiveKeyword { get; set; } = false;

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		if (!CommonTables.Any()) yield break;

		Token? clause = null;
		if (HasRecursiveKeyword)
		{
			clause = Token.Reserved(this, parent, "with recursive");
		}
		else
		{
			clause = Token.Reserved(this, parent, "with");
		}
		yield return clause;

		var isFisrt = true;
		foreach (var item in CommonTables)
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

	public IDictionary<string, object?> GetParameters()
	{
		var prm = EmptyParameters.Get();
		foreach (var item in CommonTables)
		{
			prm = prm.Merge(item.GetParameters());
		}
		return prm;
	}

	public IEnumerable<SelectableTable> GetSelectableTables(bool cascade = false)
	{
		foreach (var commonTable in CommonTables)
		{
			foreach (var item in commonTable.GetSelectableTables(cascade))
			{
				yield return item;
			}
		}
	}

	public IEnumerable<SelectQuery> GetSelectQueries()
	{
		foreach (var commonTable in CommonTables)
		{
			foreach (var item in commonTable.GetSelectQueries())
			{
				yield return item;
			}
		}
	}

	#region implements IList<CommonTable>
	public CommonTable this[int index] { get => ((IList<CommonTable>)CommonTables)[index]; set => ((IList<CommonTable>)CommonTables)[index] = value; }

	public int Count => ((ICollection<CommonTable>)CommonTables).Count;

	public bool IsReadOnly => ((ICollection<CommonTable>)CommonTables).IsReadOnly;

	public void Add(CommonTable item)
	{
		((ICollection<CommonTable>)CommonTables).Add(item);
	}

	public void Clear()
	{
		((ICollection<CommonTable>)CommonTables).Clear();
	}

	public bool Contains(CommonTable item)
	{
		return ((ICollection<CommonTable>)CommonTables).Contains(item);
	}

	public void CopyTo(CommonTable[] array, int arrayIndex)
	{
		((ICollection<CommonTable>)CommonTables).CopyTo(array, arrayIndex);
	}

	public IEnumerator<CommonTable> GetEnumerator()
	{
		return ((IEnumerable<CommonTable>)CommonTables).GetEnumerator();
	}

	public int IndexOf(CommonTable item)
	{
		return ((IList<CommonTable>)CommonTables).IndexOf(item);
	}

	public void Insert(int index, CommonTable item)
	{
		((IList<CommonTable>)CommonTables).Insert(index, item);
	}

	public bool Remove(CommonTable item)
	{
		return ((ICollection<CommonTable>)CommonTables).Remove(item);
	}

	public void RemoveAt(int index)
	{
		((IList<CommonTable>)CommonTables).RemoveAt(index);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable)CommonTables).GetEnumerator();
	}
	#endregion
}