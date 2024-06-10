using Carbunql.Analysis.Parser;
using Carbunql.Annotations;
using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Definitions;
using Carbunql.Extensions;
using Carbunql.Tables;
using Carbunql.TypeSafe.Extensions;
using Carbunql.Values;
using System.Data;
using System.Linq.Expressions;

namespace Carbunql.TypeSafe;

public class FluentSelectQuery : SelectQuery
{
    public FluentSelectQuery Select<T>(Expression<Func<T>> expression) where T : class
    {
#if DEBUG
        var analyzed = ExpressionReader.Analyze(expression);
#endif
        var mergedParameter = QueryParameterMerger.Merge(GetParameters());

        var prmManager = new ParameterManager(mergedParameter, AddParameter);

        if (expression.Body is MemberExpression mem)
        {
            // DataSet.*
            var table = mem.Member.Name;
            var c = mem.CompileAndInvoke();

            if (!GetSelectableTables().Where(x => x.Alias == table).Any())
            {
                throw new InvalidProgramException($"A dataset that is not defined in the FROM clause has been referenced. Name:{table}");
            }

            if (c is IDataRow dr)
            {
                foreach (var item in dr.DataSet.Columns)
                {
                    //Do not add duplicate columns
                    if (SelectClause != null && SelectClause.Where(x => x.Alias.IsEqualNoCase(item)).FirstOrDefault() != null)
                    {
                        continue;
                    }

                    //add
                    this.Select(table, item);
                }
                return this;
            }
            throw new InvalidProgramException();

        }
        else if (expression.Body is NewExpression ne)
        {
            if (ne.Members != null)
            {
                var columns = GetColumnNames();

                var cnt = ne.Members.Count();
                for (var i = 0; i < cnt; i++)
                {
                    var alias = ne.Members[i].Name;

                    //Remove duplicate columns before adding
                    if (SelectClause != null)
                    {
                        //remove
                        var col = SelectClause.Where(x => x.Alias.IsEqualNoCase(alias)).FirstOrDefault();
                        if (col != null)
                        {
                            SelectClause!.Remove(col);
                        }
                    }

                    //add
                    var value = ne.Arguments[i].ToValue(prmManager.AddParameter);
                    this.Select(RemoveRootBracketOrDefault(value)).As(alias);
                }
                return this;
            }
        }

        throw new InvalidProgramException();
    }

    private FluentSelectQuery Aggregate<T>(Expression<Func<T>> expression, string aggregateFunction) where T : class
    {
#if DEBUG
        var analyzed = ExpressionReader.Analyze(expression);
#endif
        var mergedParameter = QueryParameterMerger.Merge(GetParameters());

        var prmManager = new ParameterManager(mergedParameter, AddParameter);

        if (expression.Body is NewExpression ne)
        {
            if (ne.Members != null)
            {
                var cnt = ne.Members.Count();
                for (var i = 0; i < cnt; i++)
                {
                    var alias = ne.Members[i].Name;
                    var value = ne.Arguments[i].ToValue(prmManager.AddParameter);
                    this.Select($"{aggregateFunction}({value})").As(alias);
                }
                return this;
            }
        }
        throw new InvalidProgramException();
    }

    public FluentSelectQuery GroupBy<T>(Expression<Func<T>> expression) where T : class
    {
#if DEBUG
        var analyzed = ExpressionReader.Analyze(expression);
#endif
        var mergedParameter = QueryParameterMerger.Merge(GetParameters());

        var prmManager = new ParameterManager(mergedParameter, AddParameter);

        if (expression.Body is NewExpression ne)
        {
            if (ne.Members != null)
            {
                var cnt = ne.Members.Count();
                for (var i = 0; i < cnt; i++)
                {
                    var value = ne.Arguments[i].ToValue(prmManager.AddParameter);
                    var s = ValueParser.Parse(RemoveRootBracketOrDefault(value));
                    this.Group(s);
                }
                return this;
            }
        }

        throw new InvalidProgramException();
    }

    public FluentSelectQuery InnerJoin<T>(Expression<Func<T>> tableExpression, Expression<Func<bool>> conditionExpression) where T : IDataRow
    {
#if DEBUG
        var analyzed = ExpressionReader.Analyze(conditionExpression);
#endif

        var tableAlias = ((MemberExpression)tableExpression.Body).Member.Name;

        //execute
        var compiledExpression = tableExpression.Compile();
        var table = compiledExpression();

        var prmManager = new ParameterManager(GetParameters(), AddParameter);
        var condition = conditionExpression.Body.ToValue(prmManager.AddParameter);

        table.DataSet.BuildJoinClause(this, "inner join", tableAlias, condition);

        return this;
    }

    public FluentSelectQuery LeftJoin<T>(Expression<Func<T>> tableExpression, Expression<Func<bool>> conditionExpression) where T : IDataRow
    {
#if DEBUG
        var analyzed = ExpressionReader.Analyze(conditionExpression);
#endif

        var tableAlias = ((MemberExpression)tableExpression.Body).Member.Name;

        //execute
        var compiledExpression = tableExpression.Compile();
        var table = compiledExpression();

        var prmManager = new ParameterManager(GetParameters(), AddParameter);
        var condition = conditionExpression.Body.ToValue(prmManager.AddParameter);

        table.DataSet.BuildJoinClause(this, "left join", tableAlias, condition);

        return this;
    }

    public FluentSelectQuery RightJoin<T>(Expression<Func<T>> tableExpression, Expression<Func<bool>> conditionExpression) where T : IDataRow
    {
#if DEBUG
        var analyzed = ExpressionReader.Analyze(conditionExpression);
#endif

        var tableAlias = ((MemberExpression)tableExpression.Body).Member.Name;

        //execute
        var compiledExpression = tableExpression.Compile();
        var table = compiledExpression();

        var prmManager = new ParameterManager(GetParameters(), AddParameter);
        var condition = conditionExpression.Body.ToValue(prmManager.AddParameter);

        table.DataSet.BuildJoinClause(this, "right join", tableAlias, condition);

        return this;
    }

    public FluentSelectQuery CrossJoin<T>(Expression<Func<T>> tableExpression) where T : IDataRow
    {

        var tableAlias = ((MemberExpression)tableExpression.Body).Member.Name;

        //execute
        var compiledExpression = tableExpression.Compile();
        var table = compiledExpression();

        table.DataSet.BuildJoinClause(this, "cross join", tableAlias);

        return this;
    }

    public FluentSelectQuery Where(Expression<Func<bool>> expression)
    {
#if DEBUG
        var analyzed = ExpressionReader.Analyze(expression);
#endif

        var prmManager = new ParameterManager(GetParameters(), AddParameter);

        var value = expression.Body.ToValue(prmManager.AddParameter);

        this.Where(value);

        return this;
    }

    public FluentSelectQuery Exists<T>(Expression<Func<T, bool>> expression) where T : IDataRow, new()
    {
#if DEBUG
        var analyzed = ExpressionReader.Analyze(expression);
#endif
        var prmManager = new ParameterManager(GetParameters(), AddParameter);
        var value = expression.Body.ToValue(prmManager.AddParameter);

        var clause = TableDefinitionClauseFactory.Create<T>();

        var fsql = new SelectQuery();
        fsql.From(clause).As(expression.Parameters[0].Name!);
        fsql.Where(value);

        this.Where(fsql.ToExists());

        return this;
    }

    public FluentSelectQuery NotExists<T>(Expression<Func<T, bool>> expression) where T : IDataRow, new()
    {
#if DEBUG
        var analyzed = ExpressionReader.Analyze(expression);
#endif
        var prmManager = new ParameterManager(GetParameters(), AddParameter);
        var value = expression.Body.ToValue(prmManager.AddParameter);

        var clause = TableDefinitionClauseFactory.Create<T>();

        var fsql = new SelectQuery();
        fsql.From(clause).As(expression.Parameters[0].Name!);
        fsql.Where(value);

        this.Where(fsql.ToNotExists());

        return this;
    }

    public FluentSelectQuery Exists<T>(Func<FluentSelectQuery<T>> getDataSet, Expression<Func<T, bool>> expression) where T : IDataRow, new()
    {
        var dataset = getDataSet();

#if DEBUG
        var analyzed = ExpressionReader.Analyze(expression);
#endif
        var prmManager = new ParameterManager(GetParameters(), AddParameter);
        var value = expression.Body.ToValue(prmManager.AddParameter);

        var fsql = new SelectQuery();
        fsql.From(dataset).As(expression.Parameters[0].Name!);
        fsql.Where(value);

        this.Where(fsql.ToExists());

        return this;
    }

    internal static string CreateCastStatement(string value, Type type)
    {
        var dbtype = DbmsConfiguration.ToDbType(type);
        return $"cast({value} as {dbtype})";
    }

    private string RemoveRootBracketOrDefault(string value)
    {
        if (value.StartsWith("("))
        {
            //remove excess parentheses
            if (BracketValueParser.TryParse(value, out var v) && v is BracketValue bv)
            {
                return bv.Inner.ToText();
            }
        }
        return value;
    }

    /// <summary>
    /// Compiles a FluentSelectQuery for a specified table row definition type.
    /// </summary>
    /// <typeparam name="T">The type of the table row definition.</typeparam>
    /// <returns>A compiled FluentSelectQuery of type T.</returns>
    /// <exception cref="InvalidProgramException">
    /// Thrown when the select clause does not include all required columns of the table row definition type.
    /// </exception>
    public FluentSelectQuery<T> Compile<T>(bool force = false) where T : IDataRow, new()
    {
        var q = new FluentSelectQuery<T>();

        // Copy clauses and parameters to the new query object
        q.WithClause = WithClause;
        q.SelectClause = SelectClause;
        q.FromClause = FromClause;
        q.WhereClause = WhereClause;
        q.GroupClause = GroupClause;
        q.HavingClause = HavingClause;
        q.WindowClause = WindowClause;
        q.OperatableQueries = OperatableQueries;
        q.OrderClause = OrderClause;
        q.LimitClause = LimitClause;
        q.Parameters = Parameters;

        var clause = TableDefinitionClauseFactory.Create<T>();

        if (force)
        {
            CorrectSelectClause(q, clause);
        }

        TypeValidate<T>(q, clause);

        if (SelectClause == null)
        {
            foreach (var item in clause.OfType<ColumnDefinition>())
            {
                q.Select(q.FromClause!.Root, item.ColumnName);
            }
        };

        return q;
    }

    private static void CorrectSelectClause(SelectQuery q, TableDefinitionClause clause)
    {
        if (q.SelectClause == null)
        {
            // End without making corrections
            return;
        }

        // Check if all properties of T are specified in the select clause
        var aliases = q.GetSelectableItems().Select(x => x.Alias).ToHashSet();
        var missingColumns = clause.OfType<ColumnDefinition>().Where(x => !aliases.Contains(x.ColumnName));

        // Automatically add missing columns
        foreach (var item in missingColumns)
        {
            q.Select($"cast(null as {item.ColumnType.ToText()})").As(item.ColumnName);
        }
        return;
    }

    private static void TypeValidate<T>(SelectQuery q, TableDefinitionClause clause)
    {
        if (q.SelectClause != null)
        {
            // Check if all properties of T are specified in the select clause
            var aliases = q.GetSelectableItems().Select(x => x.Alias).ToHashSet();
            var missingColumns = clause.ColumnNames.Where(item => !aliases.Contains(item)).ToList();

            if (missingColumns.Any())
            {
                // If there are missing columns, include all of them in the error message
                throw new InvalidProgramException($"The select query is not compatible with '{typeof(T).Name}'. The following columns are missing: {string.Join(", ", missingColumns)}");
            }
            return;
        }
        else if (q.FromClause != null)
        {
            var actual = q.FromClause.Root.Table.GetTableFullName();
            var expect = clause.GetTableFullName();

            if (q.FromClause.Root.Table is VirtualTable v && v.Query is SelectQuery vq)
            {
                TypeValidate<T>(vq, clause);
            }
            else if (q.FromClause.Root.Table is CTETable ct)
            {
                // Check if all properties of T are specified in the select clause
                var aliases = ct.GetColumnNames().ToHashSet();
                var missingColumns = clause.ColumnNames.Where(item => !aliases.Contains(item)).ToList();

                if (missingColumns.Any())
                {
                    // If there are missing columns, include all of them in the error message
                    throw new InvalidProgramException($"The select query is not compatible with '{typeof(T).Name}'. The following columns are missing: {string.Join(", ", missingColumns)}");
                }
                return;
            }
            else if (!actual.Equals(expect))
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

public class FluentSelectQuery<T> : FluentSelectQuery where T : IDataRow, new()
{
    public T ToTable()
    {
        throw new InvalidOperationException();
    }
}
