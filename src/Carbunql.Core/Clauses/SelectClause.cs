using Carbunql.Core.Extensions;
using System.Collections;

namespace Carbunql.Core.Clauses;

public class SelectClause : IList<SelectableItem>, IQueryCommand
{
    public SelectClause()
    {
    }

    public SelectClause(List<SelectableItem> collection)
    {
        Items.AddRange(collection);
    }

    public bool HasDistinctKeyword { get; set; } = false;

    public ValueBase? Top { get; set; }

    private List<SelectableItem> Items { get; init; } = new();

    public IEnumerable<Token> GetTokens(Token? parent)
    {
        var tp = GetType();
        Token? clause = null;
        if (HasDistinctKeyword && Top != null)
        {
            clause = Token.Reserved(this, parent, "select distinct top " + Top.GetTokens(parent).ToText());
        }
        else if (HasDistinctKeyword)
        {
            clause = Token.Reserved(this, parent, "select distinct");
        }
        else if (Top != null)
        {
            clause = Token.Reserved(this, parent, "select top " + Top.GetTokens(parent).ToText());
        }
        else
        {
            clause = Token.Reserved(this, parent, "select");
        }
        yield return clause;

        var isFirst = true;
        foreach (var item in Items)
        {
            if (isFirst)
            {
                isFirst = false;
            }
            else
            {
                yield return Token.Comma(this, clause);
            }
            foreach (var token in item.GetTokens(clause)) yield return token;
        }
    }

    #region implements IList<SelectableItem>
    public SelectableItem this[int index] { get => ((IList<SelectableItem>)Items)[index]; set => ((IList<SelectableItem>)Items)[index] = value; }

    public int Count => ((ICollection<SelectableItem>)Items).Count;

    public bool IsReadOnly => ((ICollection<SelectableItem>)Items).IsReadOnly;

    public void Add(SelectableItem item)
    {
        ((ICollection<SelectableItem>)Items).Add(item);
    }

    public void Clear()
    {
        ((ICollection<SelectableItem>)Items).Clear();
    }

    public bool Contains(SelectableItem item)
    {
        return ((ICollection<SelectableItem>)Items).Contains(item);
    }

    public void CopyTo(SelectableItem[] array, int arrayIndex)
    {
        ((ICollection<SelectableItem>)Items).CopyTo(array, arrayIndex);
    }

    public IEnumerator<SelectableItem> GetEnumerator()
    {
        return ((IEnumerable<SelectableItem>)Items).GetEnumerator();
    }

    public int IndexOf(SelectableItem item)
    {
        return ((IList<SelectableItem>)Items).IndexOf(item);
    }

    public void Insert(int index, SelectableItem item)
    {
        ((IList<SelectableItem>)Items).Insert(index, item);
    }

    public bool Remove(SelectableItem item)
    {
        return ((ICollection<SelectableItem>)Items).Remove(item);
    }

    public void RemoveAt(int index)
    {
        ((IList<SelectableItem>)Items).RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Items).GetEnumerator();
    }
    #endregion
}