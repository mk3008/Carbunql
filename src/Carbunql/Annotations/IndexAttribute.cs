namespace Carbunql.Annotations;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]

public class IndexAttribute : Attribute
{
    public IndexAttribute(params string[] properties)
    {
        Properties = properties;
    }

    public string[] Properties { get; init; }

    public string ConstraintName { get; init; } = string.Empty;

    public bool IsUnique { get; init; } = false;

    //public DbIndexDefinition ToDefinition()
    //{
    //    var d = new DbIndexDefinition()
    //    {
    //        Identifiers = Identifiers.ToList(),
    //        IsUnique = IsUnique,
    //        ConstraintName = ConstraintName
    //    };
    //    return d;
    //}
}
