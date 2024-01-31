using Carbunql.Extensions;
using Carbunql.Tables;
using MessagePack;
using System.Collections;

namespace Carbunql.Clauses;

[MessagePackObject(keyAsPropertyName: true)]
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

	public IEnumerable<Token> GetTokens(Token? parent, IList<CommonTable> commons)
	{
		if (!commons.Any() || parent != null) yield break;

		Token? clause;
		if (HasRecursiveKeyword)
		{
			clause = Token.Reserved(this, null, "with recursive");
		}
		else
		{
			clause = Token.Reserved(this, null, "with");
		}
		yield return clause;


		var dic = new Dictionary<string, CommonTable>();
		var isFisrt = true;
		foreach (var item in commons)
		{
			if (dic.ContainsKey(item.Alias)) continue;
			dic.Add(item.Alias, item);

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

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		foreach (var item in GetTokens(parent, CommonTables))
		{
			yield return item;
		}
	}

	public IEnumerable<QueryParameter> GetParameters()
	{
		foreach (var ct in CommonTables)
		{
			foreach (var prm in ct.GetParameters())
			{
				yield return prm;
			}
		}
	}

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		foreach (var commonTable in CommonTables)
		{
			foreach (var item in commonTable.GetInternalQueries())
			{
				yield return item;
			}
		}
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		foreach (var commonTable in CommonTables)
		{
			foreach (var item in commonTable.GetPhysicalTables())
			{
				yield return item;
			}
		}
	}

	public IEnumerable<CommonTable> GetCommonTables()
	{
		foreach (var commonTable in CommonTables)
		{
			foreach (var item in commonTable.GetCommonTables())
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