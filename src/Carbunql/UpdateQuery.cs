using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql;

public class UpdateQuery : IQueryCommandable, IReturning, ICommentable
{
	public UpdateClause? UpdateClause { get; set; }

	public WithClause? WithClause { get; set; }

	public SetClause? SetClause { get; set; }

	public FromClause? FromClause { get; set; }

	public WhereClause? WhereClause { get; set; }

	public ReturningClause? ReturningClause { get; set; }

	[IgnoreMember]
	public CommentClause? CommentClause { get; set; }

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		if (WithClause != null)
		{
			foreach (var item in WithClause.GetInternalQueries())
			{
				yield return item;
			}
		}
		if (UpdateClause != null)
		{
			foreach (var item in UpdateClause.GetInternalQueries())
			{
				yield return item;
			}
		}
		if (SetClause != null)
		{
			foreach (var value in SetClause.Items)
			{
				foreach (var item in value.GetInternalQueries())
				{
					yield return item;
				}
			}
		}
		if (FromClause != null)
		{
			foreach (var item in FromClause.GetInternalQueries())
			{
				yield return item;
			}
		}
		if (WhereClause != null)
		{
			foreach (var item in WhereClause.GetInternalQueries())
			{
				yield return item;
			}
		}
		if (ReturningClause != null)
		{
			foreach (var item in ReturningClause.GetInternalQueries())
			{
				yield return item;
			}
		}
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		if (WithClause != null)
		{
			foreach (var item in WithClause.GetPhysicalTables())
			{
				yield return item;
			}
		}
		if (UpdateClause != null)
		{
			foreach (var item in UpdateClause.GetPhysicalTables())
			{
				yield return item;
			}
		}
		if (SetClause != null)
		{
			foreach (var value in SetClause.Items)
			{
				foreach (var item in value.GetPhysicalTables())
				{
					yield return item;
				}
			}
		}
		if (FromClause != null)
		{
			foreach (var item in FromClause.GetPhysicalTables())
			{
				yield return item;
			}
		}
		if (WhereClause != null)
		{
			foreach (var item in WhereClause.GetPhysicalTables())
			{
				yield return item;
			}
		}
		if (ReturningClause != null)
		{
			foreach (var item in ReturningClause.GetPhysicalTables())
			{
				yield return item;
			}
		}
	}

	public IEnumerable<CommonTable> GetCommonTables()
	{
		if (WithClause != null)
		{
			foreach (var item in WithClause.GetCommonTables())
			{
				yield return item;
			}
		}
		if (UpdateClause != null)
		{
			foreach (var item in UpdateClause.GetCommonTables())
			{
				yield return item;
			}
		}
		if (SetClause != null)
		{
			foreach (var value in SetClause.Items)
			{
				foreach (var item in value.GetCommonTables())
				{
					yield return item;
				}
			}
		}
		if (FromClause != null)
		{
			foreach (var item in FromClause.GetCommonTables())
			{
				yield return item;
			}
		}
		if (WhereClause != null)
		{
			foreach (var item in WhereClause.GetCommonTables())
			{
				yield return item;
			}
		}
		if (ReturningClause != null)
		{
			foreach (var item in ReturningClause.GetCommonTables())
			{
				yield return item;
			}
		}
	}

	public IEnumerable<QueryParameter>? Parameters { get; set; }

	public virtual IEnumerable<QueryParameter> GetParameters()
	{
		if (WithClause != null)
		{
			foreach (var item in WithClause.GetParameters())
			{
				yield return item;
			}
		}
		if (SetClause != null)
		{
			foreach (var item in SetClause.GetParameters())
			{
				yield return item;
			}
		}
		if (FromClause != null)
		{
			foreach (var item in FromClause.GetParameters())
			{
				yield return item;
			}
		}
		if (WhereClause != null)
		{
			foreach (var item in WhereClause.GetParameters())
			{
				yield return item;
			}
		}
		if (ReturningClause != null)
		{
			foreach (var item in ReturningClause.GetParameters())
			{
				yield return item;
			}
		}
		if (Parameters != null)
		{
			foreach (var item in Parameters)
			{
				yield return item;
			}
		}
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		if (UpdateClause == null) throw new NullReferenceException(nameof(UpdateClause));
		if (SetClause == null) throw new NullReferenceException(nameof(SetClause));

		if (CommentClause != null) foreach (var item in CommentClause.GetTokens(parent)) yield return item;

		if (parent == null && WithClause != null)
		{
			var lst = GetCommonTables().ToList();
			foreach (var item in WithClause.GetTokens(parent, lst)) yield return item;
		}

		foreach (var item in UpdateClause.GetTokens(parent)) yield return item;
		foreach (var item in SetClause.GetTokens(parent)) yield return item;
		if (FromClause != null) foreach (var item in FromClause.GetTokens(parent)) yield return item;
		if (WhereClause != null) foreach (var item in WhereClause.GetTokens(parent)) yield return item;

		if (ReturningClause == null) yield break;
		foreach (var item in ReturningClause.GetTokens(parent)) yield return item;
	}
}