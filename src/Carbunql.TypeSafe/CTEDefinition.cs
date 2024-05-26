namespace Carbunql.TypeSafe;

public struct CTEDefinition
{
    public string Name { get; set; }

    public Type RowType { get; set; }

    public string Query { get; set; }
}
