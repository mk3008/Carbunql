using Carbunql.Annotations;
using Carbunql.Building;
using Carbunql.Definitions;
using Carbunql.Tables;
using System.Data;

namespace Carbunql.TypeSafe;

public static class SelectQueryExtension
{

    public static FluentSelectQuery<T> Compile<T>(this SelectQuery source, bool force = false) where T : IDataRow, new()
    {
        var q = new FluentSelectQuery<T>();

        // Copy clauses and parameters to the new query object
        q.WithClause = source.WithClause;
        q.SelectClause = source.SelectClause;
        q.FromClause = source.FromClause;
        q.WhereClause = source.WhereClause;
        q.GroupClause = source.GroupClause;
        q.HavingClause = source.HavingClause;
        q.WindowClause = source.WindowClause;
        q.OperatableQueries = source.OperatableQueries;
        q.OrderClause = source.OrderClause;
        q.LimitClause = source.LimitClause;
        q.Parameters = source.Parameters;
        q.CommentClause = source.CommentClause;
        q.HeaderCommentClause = source.HeaderCommentClause;

        var columns = PropertySelector.SelectLiteralProperties<T>().Select(x => x.Name);

        if (force)
        {
            foreach (var item in columns)
            {
                q.Select(q.FromClause!.Root, item);
            }
        }

        TypeValidate<T>(q, columns);

        if (q.SelectClause == null)
        {
            foreach (var item in columns)
            {
                q.Select(q.FromClause!.Root, item);
            }
        };

        return q;
    }

    private static void TypeValidate<T>(SelectQuery q)
    {
        TypeValidate<T>(q, PropertySelector.SelectLiteralProperties<T>().Select(x => x.Name));
    }

    private static void TypeValidate<T>(SelectQuery q, IEnumerable<string> columns)
    {
        if (q.SelectClause != null && !(q.SelectClause.Count == 1 && q.SelectClause[0].Alias == "*"))
        {
            // Check if all properties of T are specified in the select clause
            var aliases = q.GetSelectableItems().Select(x => x.Alias).ToHashSet();
            var missingColumns = columns.Where(item => !aliases.Contains(item)).ToList();

            if (missingColumns.Any())
            {
                // If there are missing columns, include all of them in the error message
                throw new InvalidProgramException($"The select query is not compatible with '{typeof(T).Name}'. The following columns are missing: {string.Join(", ", missingColumns)}");
            }
            return;
        }
        else if (q.FromClause != null)
        {
            if (q.FromClause.Root.Table is VirtualTable v && v.Query is SelectQuery vq)
            {
                TypeValidate<T>(vq, columns);
                return;
            }
            else if (q.FromClause.Root.Table is CTETable ct)
            {
                // Check if all properties of T are specified in the select clause
                var aliases = ct.GetColumnNames().ToHashSet();
                var missingColumns = columns.Where(item => !aliases.Contains(item)).ToList();

                if (missingColumns.Any())
                {
                    // If there are missing columns, include all of them in the error message
                    throw new InvalidProgramException($"The select query is not compatible with '{typeof(T).Name}'. The following columns are missing: {string.Join(", ", missingColumns)}");
                }
                return;
            }

            var actual = q.FromClause.Root.Table.GetTableFullName();
            var clause = TableDefinitionClauseFactory.Create<T>();
            var expect = clause.GetTableFullName();

            if (!actual.Equals(expect))
            {
                throw new InvalidProgramException($"The select query is not compatible with '{typeof(T).Name}'. Expect: {expect}, Actual: {actual}");
            }
            return;
        }
        else
        {
            throw new InvalidProgramException($"The select query is not compatible with '{typeof(T).Name}'. FromClause is null.");
        }
    }
}
