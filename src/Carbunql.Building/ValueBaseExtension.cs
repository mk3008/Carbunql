using Carbunql.Clauses;
using Carbunql.Values;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carbunql.Building;

public static class ValueBaseExtension
{

    public static ValueBase GetLast(this ValueBase source)
    {
        if (source.OperatableValue == null) return source;
        return source.OperatableValue.Value.GetLast();
    }

    public static ValueBase Equal(this ValueBase source, ValueBase operand)
    {
        source.GetLast().AddOperatableValue("=", operand);
        return source;
    }

    public static ValueBase NotEqual(this ValueBase source, ValueBase operand)
    {
        source.GetLast().AddOperatableValue("<>", operand);
        return source;
    }

    public static ValueBase IsNull(this ValueBase source)
    {
        source.GetLast().AddOperatableValue("is", new LiteralValue("null"));
        return source;
    }

    public static ValueBase IsNotNull(this ValueBase source)
    {
        source.GetLast().AddOperatableValue("is not", new LiteralValue("null"));
        return source;
    }

    public static ValueBase True(this ValueBase source)
    {
        source.GetLast().AddOperatableValue("=", new LiteralValue("true"));
        return source;
    }

    public static ValueBase False(this ValueBase source)
    {
        source.GetLast().AddOperatableValue("=", new LiteralValue("false"));
        return source;
    }

    public static ValueBase Expression(this ValueBase source, string @operator, ValueBase operand)
    {
        source.GetLast().AddOperatableValue(@operator, operand);
        return source;
    }

    public static ValueBase Expression(this ValueBase source, string @operator, Func<ValueBase> builder)
    {
        source.GetLast().AddOperatableValue(@operator, builder());
        return source;
    }

    public static ValueBase And(this ValueBase source, ValueBase operand)
    {
        source.GetLast().AddOperatableValue("and", operand);
        return source;
    }

    public static ValueBase Or(this ValueBase source, ValueBase operand)
    {
        source.GetLast().AddOperatableValue("or", operand);
        return source;
    }

    public static ValueBase ToGroup(this ValueBase source)
    {
        return new BracketValue(source);
    }

    public static SortableItem ToSortable(this ValueBase source, bool isAscending = true)
    {
        return new SortableItem(source) { IsAscending = isAscending };
    }
}