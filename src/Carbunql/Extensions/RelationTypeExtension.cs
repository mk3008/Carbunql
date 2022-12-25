namespace Carbunql.Extensions;

public static class RelationTypeExtension
{
	public static string ToCommandText(this TableJoin source)
	{
		switch (source)
		{
			case TableJoin.Inner:
				return "inner join";
			case TableJoin.Left:
				return "left join";
			case TableJoin.Right:
				return "right join";
			case TableJoin.Cross:
				return "cross join";
		}
		throw new NotSupportedException();
	}
}