using Carbunql.Tables;
using System.Collections;

namespace Carbunql.Clauses;

public class CommentClause : IList<string>, IQueryCommandable
{
	public string this[int index] { get => ((IList<string>)Collection)[index]; set => ((IList<string>)Collection)[index] = value; }

	public int Count => ((ICollection<string>)Collection).Count;

	public bool IsReadOnly => ((ICollection<string>)Collection).IsReadOnly;

	private List<string> Collection { get; set; } = new();

	public void Add(string item)
	{
		((ICollection<string>)Collection).Add(item);
	}

	public void Clear()
	{
		((ICollection<string>)Collection).Clear();
	}

	public bool Contains(string item)
	{
		return ((ICollection<string>)Collection).Contains(item);
	}

	public void CopyTo(string[] array, int arrayIndex)
	{
		((ICollection<string>)Collection).CopyTo(array, arrayIndex);
	}

	public IEnumerable<CommonTable> GetCommonTables()
	{
		yield break;
	}

	public IEnumerator<string> GetEnumerator()
	{
		return ((IEnumerable<string>)Collection).GetEnumerator();
	}

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		yield break;
	}

	public IDictionary<string, object?> GetParameters()
	{
		return EmptyParameters.Get();
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		yield break;
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		if (!Collection.Any()) yield break;

		foreach (var item in Collection)
		{
			yield return Token.Reserved(this, parent, "/*");
			yield return new Token(this, parent, item.Replace("/*", "").Replace("*/", ""));
			yield return Token.Reserved(this, parent, "*/");
		}
	}

	public int IndexOf(string item)
	{
		return ((IList<string>)Collection).IndexOf(item);
	}

	public void Insert(int index, string item)
	{
		((IList<string>)Collection).Insert(index, item);
	}

	public bool Remove(string item)
	{
		return ((ICollection<string>)Collection).Remove(item);
	}

	public void RemoveAt(int index)
	{
		((IList<string>)Collection).RemoveAt(index);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable)Collection).GetEnumerator();
	}
}
