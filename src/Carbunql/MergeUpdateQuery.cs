using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;

namespace Carbunql;

public class MergeUpdateQuery : IQueryCommandable
{
	public MergeSetClause? SetClause { get; set; }

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		if (SetClause != null)
		{
			foreach (var item in SetClause.GetInternalQueries())
			{
				yield return item;
			}
		}
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		if (SetClause != null)
		{
			foreach (var item in SetClause.GetPhysicalTables())
			{
				yield return item;
			}
		}
	}

	public IEnumerable<CommonTable> GetCommonTables()
	{
		if (SetClause != null)
		{
			foreach (var item in SetClause.GetCommonTables())
			{
				yield return item;
			}
		}
	}

	public virtual IEnumerable<QueryParameter> GetParameters()
	{
		if (SetClause != null)
		{
			foreach (var item in SetClause.GetParameters())
			{
				yield return item;
			}
		}
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		if (SetClause == null) throw new NullReferenceException();

		var t = Token.Reserved(this, parent, "update set");
		yield return t;
		foreach (var item in SetClause.GetTokens(t)) yield return item;
	}
}