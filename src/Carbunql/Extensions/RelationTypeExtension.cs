namespace Carbunql.Extensions;

public static class RelationTypeExtension
{
    public static string ToCommandText(this TableJoin source)
    {
        return TableJoinEnumReader.GetCommandAttribute(source).FullText;
    }
}