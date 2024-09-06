using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;
using Cysharp.Text;

namespace Carbunql;

/// <summary>
/// Interface for objects that can generate tokens, parameters, and internal queries for a query command.
/// </summary>
public interface IQueryCommandable : IColumnContainer
{
    /// <summary>
    /// Retrieves tokens for the query command.
    /// </summary>
    IEnumerable<Token> GetTokens(Token? parent);

    /// <summary>
    /// Retrieves SQL query parameters associated with the query command.
    /// </summary>
    /// <returns>An enumerable collection of SQL query parameters.</returns>
    IEnumerable<QueryParameter> GetParameters();

    /// <summary>
    /// Retrieves internal queries within the main query.
    /// Internal queries include the main query itself, as well as any inline or subqueries used within the SQL.
    /// </summary>
    /// <returns>An enumerable collection of internal queries.</returns>
    IEnumerable<SelectQuery> GetInternalQueries();

    /// <summary>
    /// Retrieves physical tables from the database.
    /// </summary>
    /// <returns>An enumerable collection of physical tables.</returns>
    IEnumerable<PhysicalTable> GetPhysicalTables();

    /// <summary>
    /// Retrieves common tables (CTEs) defined in the query's WITH clause.
    /// </summary>
    /// <returns>An enumerable collection of common tables (CTEs).</returns>
    IEnumerable<CommonTable> GetCommonTables();
}

/// <summary>
/// Extension methods for <see cref="IQueryCommandable"/> to facilitate command generation and conversion to text.
/// </summary>
public static class IQueryCommandableExtension
{
    /// <summary>
    /// Retrieves tokens for the query command.
    /// </summary>
    /// <param name="source">The source <see cref="IQueryCommandable"/>.</param>
    /// <returns>Tokens for the query command.</returns>
    public static IEnumerable<Token> GetTokens(this IQueryCommandable source)
    {
        return source.GetTokens(null);
    }

    /// <summary>
    /// Converts the query commandable to a <see cref="QueryCommand"/>.
    /// </summary>
    /// <param name="source">The source <see cref="IQueryCommandable"/>.</param>
    /// <returns>A <see cref="QueryCommand"/>.</returns>
    public static QueryCommand ToCommand(this IQueryCommandable source)
    {
        var builder = new CommandTextBuilder();
        return new QueryCommand(builder.Execute(source), source.GetParameters());
    }

    /// <summary>
    /// Converts the query commandable to a <see cref="QueryCommand"/> using a custom <see cref="CommandTextBuilder"/>.
    /// </summary>
    /// <param name="source">The source <see cref="IQueryCommandable"/>.</param>
    /// <param name="builder">The <see cref="CommandTextBuilder"/> instance to use.</param>
    /// <returns>A <see cref="QueryCommand"/>.</returns>
    public static QueryCommand ToCommand(this IQueryCommandable source, CommandTextBuilder builder)
    {
        return new QueryCommand(builder.Execute(source), source.GetParameters());
    }

    /// <summary>
    /// Converts the query commandable to a one-line <see cref="QueryCommand"/>.
    /// </summary>
    /// <param name="source">The source <see cref="IQueryCommandable"/>.</param>
    /// <returns>A one-line <see cref="QueryCommand"/>.</returns>
    public static QueryCommand ToOneLineCommand(this IQueryCommandable source)
    {
        return new QueryCommand(source.GetTokens().ToText(), source.GetParameters());
    }

    /// <summary>
    /// Converts the query commandable to text.
    /// </summary>
    /// <param name="source">The source <see cref="IQueryCommandable"/>.</param>
    /// <returns>Text representation of the query commandable.</returns>
    public static string ToText(this IQueryCommandable source)
    {
        var cmd = source.ToCommand();
        return cmd.CommandText;
    }

    /// <summary>
    /// Converts the query commandable to text.
    /// </summary>
    /// <param name="source">The source <see cref="IQueryCommandable"/>.</param>
    /// <param name="exportParameterInfo">This parameter is invalid</param>
    /// <returns>Text representation of the query commandable.</returns>
    [Obsolete("use ToText method.")]
    public static string ToText(this IQueryCommandable source, bool exportParameterInfo = true)
    {
        var cmd = source.ToCommand();
        return cmd.CommandText;
    }

    /// <summary>
    /// Converts the query commandable to a one-line text.
    /// </summary>
    /// <param name="source">The source <see cref="IQueryCommandable"/>.</param>
    /// <returns>One-line text representation of the query commandable.</returns>
    public static string ToOneLineText(this IQueryCommandable source)
    {
        var cmd = source.ToOneLineCommand();
        return cmd.CommandText;
    }

    public static string GetParameterText(this QueryCommand source)
    {
        var prms = source.Parameters;
        if (!prms.Any()) return string.Empty;

        var names = new List<string>();

        var sb = ZString.CreateStringBuilder();
        sb.AppendLine("/*");
        foreach (var item in prms)
        {
            if (names.Contains(item.Key)) continue;

            names.Add(item.Key);
            if (item.Value == null)
            {
                sb.AppendLine($"  {item.Key} is NULL");
            }
            else if (item.Value.GetType() == typeof(string) || item.Value.GetType() == typeof(DateTime))
            {
                sb.AppendLine($"  {item.Key} = '{item.Value}'");
            }
            else
            {
                sb.AppendLine($"  {item.Key} = {item.Value}");
            }
        }
        sb.Append("*/");

        return sb.ToString();
    }
}
