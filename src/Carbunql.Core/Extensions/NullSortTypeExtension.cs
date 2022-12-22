namespace Carbunql.Core.Extensions;

public static class NullSortTypeExtension
{
    public static string ToCommandText(this NullSort source)
    {
        switch (source)
        {
            case NullSort.Undefined:
                return string.Empty;
            case NullSort.First:
                return "nulls first";
            case NullSort.Last:
                return "nulls last";
        }
        throw new NotSupportedException();
    }
}