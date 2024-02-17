using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Definitions;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql;

public class CreateTableQuery : IQueryCommandable, ICommentable
{
	public CreateTableQuery(string schema, string table)
	{
		Schema = schema;
		Table = table;
	}

	public CreateTableQuery(string table)
	{
		Table = table;
	}

	public bool IsTemporary { get; set; } = false;

	public string? Schema { get; init; } = null;

	public string Table { get; init; }

	public string TableFullName => (string.IsNullOrEmpty(Schema)) ? Table : Schema + "." + Table;

	public List<ColumnDefinition> Columns { get; set; } = new();

	public List<IConstraint> Constraints { get; set; } = new();

	public IReadQuery? Query { get; set; }

	public IEnumerable<QueryParameter>? Parameters { get; set; }

	[IgnoreMember]
	public CommentClause? CommentClause { get; set; }

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		if (Query != null)
		{
			foreach (var item in Query.GetInternalQueries())
			{
				yield return item;
			}
		}
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		if (Query != null)
		{
			foreach (var item in Query.GetPhysicalTables())
			{
				yield return item;
			}
		}
	}

	public virtual IEnumerable<QueryParameter> GetParameters()
	{
		if (Parameters != null)
		{
			foreach (var item in Parameters)
			{
				yield return item;
			}
		}

		if (Query != null)
		{
			foreach (var item in Query.GetParameters())
			{
				yield return item;
			}
			yield break;
		}
	}

	private Token GetCreateTableToken(Token? parent)
	{
		if (IsTemporary)
		{
			return Token.Reserved(this, parent, "create temporary table");
		}
		return Token.Reserved(this, parent, "create table");
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		//if (Query == null) throw new NullReferenceException(nameof(Query));

		if (CommentClause != null) foreach (var item in CommentClause.GetTokens(parent)) yield return item;

		var ct = GetCreateTableToken(parent);
		yield return ct;
		yield return new Token(this, parent, TableFullName);

		if (Query != null)
		{
			var t = new Token(this, parent, "as", isReserved: true);
			yield return t;

			foreach (var item in Query.GetTokens())
			{
				yield return item;
			}
			yield break;
		}


		var isFirst = true;
		var bracket = Token.ReservedBracketStart(this, parent);
		yield return bracket;
		foreach (var column in Columns)
		{
			if (isFirst)
			{
				isFirst = false;
			}
			else
			{
				yield return Token.Comma(this, bracket);
			}
			foreach (var item in column.GetTokens(bracket))
			{
				yield return item;
			}
		}
		foreach (var constraint in Constraints)
		{
			yield return Token.Comma(this, bracket);
			foreach (var item in constraint.GetTokens(bracket))
			{
				yield return item;
			}
		}
		yield return Token.ReservedBracketEnd(this, parent);
	}

	public IEnumerable<CommonTable> GetCommonTables()
	{
		if (Query != null)
		{
			foreach (var item in Query.GetCommonTables())
			{
				yield return item;
			}
			yield break;
		}
	}

	public SelectQuery ToSelectQuery()
	{
		if (string.IsNullOrEmpty(TableFullName)) throw new NullReferenceException(nameof(TableFullName));

		if (Query != null)
		{
			var sq = new SelectQuery();
			var (_, t) = sq.From(TableFullName).As("t");

			foreach (var item in Query.GetColumnNames())
			{
				sq.Select(t, item);
			}

			return sq;
		}

		if (Columns.Any())
		{
			var sq = new SelectQuery();
			var (_, t) = sq.From(TableFullName).As("t");

			foreach (var item in Columns)
			{
				sq.Select(t, item.ColumnName);
			}

			return sq;
		}

		throw new InvalidOperationException();
	}

	public SelectQuery ToCountQuery(string alias = "row_count")
	{
		if (string.IsNullOrEmpty(TableFullName)) throw new NullReferenceException(nameof(TableFullName));

		var sq = new SelectQuery();
		sq.From(TableFullName).As("q");
		sq.Select("count(*)").As(alias);
		return sq;
	}
}