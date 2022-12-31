using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Values;
using System.Data;

namespace Carbunql;

public class CommandFormatter
{
    public virtual bool IsLineBreakOnBeforeWriteToken(Token token)
    {
        if (token.Text.Equals(",") && token.Sender is Relation) return false;

        if (!token.Text.AreEqual("on") && token.Sender is Relation) return true;
        if (token.Text.AreEqual("else") || token.Text.AreEqual("when")) return true;
        if (token.Text.AreEqual("and"))
        {
            if (token.Sender is BetweenExpression) return false;
            if (token.Parent != null && token.Parent.Sender is WhereClause) return true;
            return false;
        }

        return false;
    }

    public virtual bool IsLineBreakOnAfterWriteToken(Token token)
    {
        if (token.Sender is OperatableQuery) return true;

        if (token.Text.Equals(","))
        {
            if (token.Sender is WithClause) return true;
            if (token.Sender is SelectClause) return true;
            if (token.Sender is Relation) return true;
            if (token.Sender is GroupClause) return true;
            if (token.Sender is OrderClause) return true;
            if (token.Sender is ValuesClause) return true;
        }

        return false;
    }

    public virtual bool IsIncrementIndentOnBeforeWriteToken(Token token)
    {
        if (token.Sender is OperatableQuery) return false;

        if (token.Text.Equals("(") && token.IsReserved == false) return false;

        if (token.Parent != null && token.Parent.Sender is ValuesClause) return false;
        if (token.Sender is FunctionValue) return false;
        if (token.Sender is WindowFunction) return false;

        return true;
    }

    public virtual bool IsDecrementIndentOnBeforeWriteToken(Token token)
    {
        if (token.Parent == null) return true;

        if (token.Text.Equals(")") && token.IsReserved == false) return false;

        if (token.Parent != null && token.Parent.Sender is ValuesClause) return false;
        if (token.Sender is FunctionValue) return false;
        if (token.Sender is WindowFunction) return false;

        return true;
    }
}