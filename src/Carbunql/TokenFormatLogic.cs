using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;
using Carbunql.Values;

namespace Carbunql;

public class TokenFormatLogic
{
	public Action<string>? Logger { get; set; }

	public virtual bool IsLineBreakOnBeforeWriteToken(Token token)
	{
		if (token.Text.IsEqualNoCase("with")) return true;
		if (token.Text.IsEqualNoCase("window")) return true;
		if (token.Text.IsEqualNoCase("select")) return true;

		if (token.Text.Equals("/*")) return true;

		if (token.Text.Equals(",") && token.Sender is Relation) return false;

		if (token.Text.Equals("as") && token.Sender is CreateTableQuery) return true;

		if (!token.Text.IsEqualNoCase("on") && token.Sender is Relation) return true;
		if (token.Text.IsEqualNoCase("else") || token.Text.IsEqualNoCase("when")) return true;
		if (token.Text.IsEqualNoCase("and"))
		{
			if (token.Sender is BetweenClause) return false;
			if (token.Parent != null && token.Parent.Sender is WhereClause) return true;
			if (token.Parent != null && token.Parent.Sender is HavingClause) return true;
			return false;
		}

		return false;
	}

	public virtual bool IsLineBreakOnAfterWriteToken(Token token)
	{
		if (token.Sender is OperatableQuery) return true;

		if (token.Text.Equals("*/")) return true;

		if (token.Text.Equals(","))
		{
			if (token.Sender is WithClause) return true;
			if (token.Sender is WindowClause) return true;
			if (token.Sender is SelectClause) return true;
			if (token.Sender is Relation) return true;
			if (token.Sender is GroupClause) return true;
			if (token.Sender is OrderClause) return true;
			if (token.Sender is ValuesQuery) return true;
			if (token.Sender is SetClause) return true;
			if (token.Sender is PartitionClause) return true;
			if (token.Sender is TableDefinitionClause) return true;
		}

		return false;
	}

	public virtual bool IsIncrementIndentOnBeforeWriteToken(Token token)
	{
		if (token.Sender is OperatableQuery) return false;

		if (token.Text.Equals("(") && token.Sender is DistinctClause) return false;

		if (token.Text.Equals("(") && token.IsReserved == false) return false;
		if (token.Parent != null && token.Parent.Sender is ValuesQuery) return false;
		if (token.Sender is FunctionValue) return false;
		if (token.Sender is FunctionTable) return false;
		if (token.Text.Equals("filter")) return false;
		if (token.Text.Equals("over")) return false;

		return true;
	}

	public virtual bool IsDecrementIndentOnBeforeWriteToken(Token token)
	{
		if (token.Parent == null) return true;

		if (token.Text.Equals(")") && token.IsReserved == false) return false;

		if (token.Parent.Sender is ValuesQuery) return false;
		if (token.Sender is FunctionValue) return false;
		if (token.Sender is FunctionTable) return false;
		if (token.Text.Equals(")") && token.Parent.Text.IsEqualNoCase("filter")) return true;
		if (token.Text.Equals(")") && token.Parent.Text.IsEqualNoCase("over")) return true;

		return true;
	}
}