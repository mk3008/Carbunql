namespace Carbunql;

public enum TableJoin
{
    Undefined,
    [Command("inner", "inner join")]
    Inner,
    [Command("left", "left join")]
    Left,
    [Command("right", "right join")]
    Right,
    [Command("cross", "cross join")]
    Cross,
    [Command(",")]
    Comma,
    [Command("left", "left outer join")]
    LeftOuter,
    [Command("right", "right outer join")]
    RightOuter,
}