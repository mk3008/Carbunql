﻿using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;
using Carbunql.Values;

namespace Carbunql;

/// <summary>
/// Provides logic for formatting tokens in query writing.
/// </summary>
public class LeadingCommaTokenFormattingLogic : ITokenFormattingLogic
{
    /// <summary>
    /// Gets or sets the logger action for logging.
    /// </summary>
    public Action<string>? Logger { get; set; }

    /// <summary>
    /// Determines whether a line break should occur before writing the specified token.
    /// </summary>
    /// <param name="token">The token to evaluate.</param>
    /// <returns>True if a line break is required before writing the token; otherwise, false.</returns>
    public virtual bool IsLineBreakOnBeforeWriteToken(Token token)
    {
        if (token.Text.IsEqualNoCase("with")) return true;
        if (token.Text.IsEqualNoCase("window")) return true;
        if (token.Text.IsEqualNoCase("select")) return true;

        if (token.Text.Equals("/*")) return true;

        if (token.Text.Equals(",") && token.Sender is Relation) return false;

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
            if (token.Sender is IndexOnClause) return true;
            if (token.Sender is AlterTableClause) return true;
        }

        if (token.Text.IsEqualNoCase("as") && token.Sender is CreateTableQuery) return true;

        if (!token.Text.IsEqualNoCase("on") && token.Sender is Relation) return true;
        if (token.Text.IsEqualNoCase("else") || token.Text.IsEqualNoCase("when")) return true;
        if (token.Text.IsEqualNoCase("and"))
        {
            if (token.Sender is BetweenClause) return false;
            if (token.Parent != null && token.Parent.Sender is WhereClause) return true;
            if (token.Parent != null && token.Parent.Sender is HavingClause) return true;
            return false;
        }

        if (token.Text.IsEqualNoCase("where") && token.Parent == null) return true;

        return false;
    }

    /// <summary>
    /// Determines whether a line break should occur after writing the specified token.
    /// </summary>
    /// <param name="token">The token to evaluate.</param>
    /// <returns>True if a line break is required after writing the token; otherwise, false.</returns>
    public virtual bool IsLineBreakOnAfterWriteToken(Token token)
    {
        if (token.Sender is OperatableQuery) return true;

        if (token.Text.Equals("*/")) return true;

        return false;
    }

    /// <summary>
    /// Determines whether the indent level should be incremented before writing the specified token.
    /// </summary>
    /// <param name="token">The token to evaluate.</param>
    /// <returns>True if the indent level should be incremented before writing the token; otherwise, false.</returns>
    public virtual bool IsIncrementIndentOnBeforeWriteToken(Token token)
    {
        if (token.Sender is OperatableQuery) return false;

        if (token.Text.Equals("(") && token.IsReserved == false) return false;
        if (token.Text.Equals("(") && token.Sender is DistinctClause) return false;
        if (token.Text.Equals("(") && token.Sender is InsertClause) return false;

        if (token.Parent != null && token.Parent.Sender is ValuesQuery) return false;
        if (token.Sender is FunctionValue) return false;
        if (token.Sender is FunctionTable) return false;
        if (token.Sender is Interval) return false;
        if (token.Text.IsEqualNoCase("filter")) return false;
        if (token.Text.IsEqualNoCase("over")) return false;

        return true;
    }

    /// <summary>
    /// Determines whether the indent level should be decremented before writing the specified token.
    /// </summary>
    /// <param name="token">The token to evaluate.</param>
    /// <returns>True if the indent level should be decremented before writing the token; otherwise, false.</returns>
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
