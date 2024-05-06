namespace Carbunql.Annotations;

/// <summary>
/// Attribute used to ignore mapping of a property to a database column.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class IgnoreMappingAttribute : Attribute
{
}

