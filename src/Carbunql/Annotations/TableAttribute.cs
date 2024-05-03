using Carbunql.Definitions;

namespace Carbunql.Annotations;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
public class TableAttribute : Attribute, ITable
{
    public TableAttribute(string[] identifiers)
    {
        PrimaryKeyProperties = identifiers;
        if (identifiers.Length > 1) HasAutoNumber = false;
    }

    /// <summary>
    /// Name of the "property" that will be the primary key.
    /// </summary>
    public IEnumerable<string> PrimaryKeyProperties { get; init; }

    public string Schema { get; init; } = string.Empty;

    public string Table { get; init; } = string.Empty;

    public string ConstraintName { get; init; } = string.Empty;

    public string Comment { get; init; } = string.Empty;

    public bool HasAutoNumber { get; init; } = true;

    public string AutoNumberDefinition { get; init; } = string.Empty;

    public string NextValueCommand { get; init; } = string.Empty;


    //public ClassTableDefinitionClause<T> ToDefinition<T>()
    //{
    //    var table = !string.IsNullOrEmpty(Table) ? Table : typeof(T).Name.ToSnakeCase();

    //    var d = new ClassTableDefinitionClause<T>(Schema, table)
    //    {
    //        Comment = Comment,
    //    };
    //    return d;
    //}

    //public static TableAttribute CreateDefault<T>()
    //{
    //    var table = typeof(T).Name.ToSnakeCase();
    //    var pkeys = new[] { table + "id" };
    //    var attr = new TableAttribute(pkeys) { Table = table };
    //    return attr;
    //}
}