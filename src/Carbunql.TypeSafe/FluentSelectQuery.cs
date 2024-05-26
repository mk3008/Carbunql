using Carbunql.Analysis.Parser;
using Carbunql.Annotations;
using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Definitions;
using Carbunql.Tables;
using Carbunql.TypeSafe.Extensions;
using Carbunql.Values;
using System.Linq.Expressions;

namespace Carbunql.TypeSafe;

public class FluentSelectQuery : SelectQuery
{
    public FluentSelectQuery Select<T>(Expression<Func<T>> expression) where T : class
    {
#if DEBUG
        var analyzed = ExpressionReader.Analyze(expression);
#endif

        var body = (NewExpression)expression.Body;

        var prmManager = new ParameterManager(GetParameters(), AddParameter);

        if (body.Members != null)
        {
            var cnt = body.Members.Count();
            for (var i = 0; i < cnt; i++)
            {
                var alias = body.Members[i].Name;
                var value = ToValue(body.Arguments[i], prmManager.AddParaemter);
                this.Select(RemoveRootBracketOrDefault(value)).As(alias);
            }
        }

        return this;
    }

    public FluentSelectQuery InnerJoin<T>(Expression<Func<T>> tableExpression, Expression<Func<bool>> conditionExpression) where T : ITableRowDefinition
    {
#if DEBUG
        var analyzed = ExpressionReader.Analyze(conditionExpression);
#endif

        var tableAlias = ((MemberExpression)tableExpression.Body).Member.Name;

        //execute
        var compiledExpression = tableExpression.Compile();
        var table = compiledExpression();

        var prmManager = new ParameterManager(GetParameters(), AddParameter);
        var condition = ToValue(conditionExpression.Body, prmManager.AddParaemter);

        table.Datasource.BuildJoinClause(this, "inner join", tableAlias, condition);

        return this;
    }

    public FluentSelectQuery LeftJoin<T>(Expression<Func<T>> tableExpression, Expression<Func<bool>> conditionExpression) where T : ITableRowDefinition
    {
#if DEBUG
        var analyzed = ExpressionReader.Analyze(conditionExpression);
#endif

        var tableAlias = ((MemberExpression)tableExpression.Body).Member.Name;

        //execute
        var compiledExpression = tableExpression.Compile();
        var table = compiledExpression();

        var prmManager = new ParameterManager(GetParameters(), AddParameter);
        var condition = ToValue(conditionExpression.Body, prmManager.AddParaemter);

        table.Datasource.BuildJoinClause(this, "left join", tableAlias, condition);

        return this;
    }

    public FluentSelectQuery RightJoin<T>(Expression<Func<T>> tableExpression, Expression<Func<bool>> conditionExpression) where T : ITableRowDefinition
    {
#if DEBUG
        var analyzed = ExpressionReader.Analyze(conditionExpression);
#endif

        var tableAlias = ((MemberExpression)tableExpression.Body).Member.Name;

        //execute
        var compiledExpression = tableExpression.Compile();
        var table = compiledExpression();

        var prmManager = new ParameterManager(GetParameters(), AddParameter);
        var condition = ToValue(conditionExpression.Body, prmManager.AddParaemter);

        table.Datasource.BuildJoinClause(this, "right join", tableAlias, condition);

        return this;
    }

    public FluentSelectQuery CrossJoin<T>(Expression<Func<T>> tableExpression) where T : ITableRowDefinition
    {

        var tableAlias = ((MemberExpression)tableExpression.Body).Member.Name;

        //execute
        var compiledExpression = tableExpression.Compile();
        var table = compiledExpression();

        table.Datasource.BuildJoinClause(this, "cross join", tableAlias);

        return this;
    }

    public FluentSelectQuery Where(Expression<Func<bool>> expression)
    {
#if DEBUG
        var analyzed = ExpressionReader.Analyze(expression);
#endif

        var prmManager = new ParameterManager(GetParameters(), AddParameter);

        var value = ToValue(expression.Body, prmManager.AddParaemter);

        if (expression.Body is BinaryExpression be && be.NodeType == ExpressionType.OrElse)
        {
            this.Where($"({value})");
        }
        else
        {
            this.Where(value);
        }
        return this;
    }

    private string ToValue(Expression exp, Func<string, object?, string> addParameter)
    {
        if (exp is MemberExpression mem)
        {
            return mem.ToValue(ToValue, addParameter);
        }
        else if (exp is ConstantExpression ce)
        {
            return ce.ToValue(ToValue, addParameter);
        }
        else if (exp is NewExpression ne)
        {
            return ne.ToValue(ToValue, addParameter);
        }
        else if (exp is BinaryExpression be)
        {
            return be.ToValue(ToValue, addParameter);
        }
        else if (exp is UnaryExpression ue)
        {
            return ue.ToValue(ToValue, addParameter);
        }
        else if (exp is MethodCallExpression mce)
        {
            return mce.ToValue(ToValue, addParameter);
        }
        else if (exp is ConditionalExpression cnd)
        {
            return cnd.ToValue(ToValue, addParameter);
        }
        else if (exp is ParameterExpression prm)
        {
            return prm.ToValue(ToValue, addParameter);
        }

        throw new InvalidProgramException(exp.ToString());
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
    public FluentSelectQuery<T> Compile<T>(bool force = false) where T : ITableRowDefinition, new()
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

public class FluentSelectQuery<T> : FluentSelectQuery where T : ITableRowDefinition, new()
{
    public T ToTable()
    {
        throw new InvalidOperationException();
    }
}
