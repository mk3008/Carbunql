using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Carbunql.Analysis;

public static class ReservedText
{
	public static string Comma => ",";
	public static string Semicolon => ";";
	public static string With => "with";
	public static string Values => "values";
	public static string Select => "select";
	public static string From => "from";
	public static string Where => "where";
	public static string Group => "group";
	public static string Having => "having";
	public static string Order => "order";
	public static string Union => "union";
	public static string Minus => "minus";
	public static string Except => "except";
	public static string Intersect => "intersect";
	public static string Limit => "limit";
	public static string Not => "not";
	public static string As => "as";
	public static string Materialized => "materialized";
	public static string StartBracket => "(";
	public static string EndBracket => ")";
	public static string And => "and";
	public static string Or => "or";
	public static string On => "on";
	public static string Inner => "inner";
	public static string Left => "left";
	public static string Right => "right";
	public static string Cross => "cross";
	public static IEnumerable<string> All()
	{
		yield return Comma;
		yield return Semicolon;
		yield return With;
		yield return Values;
		yield return Select;
		yield return From;
		yield return Where;
		yield return Group;
		yield return Having;
		yield return Order;
		yield return Union;
		yield return Minus;
		yield return Except;
		yield return Intersect;
		yield return Limit;
		yield return Not;
		yield return As;
		yield return Materialized;
		yield return StartBracket;
		yield return EndBracket;
		yield return And;
		yield return Or;
		yield return On;
		yield return Inner;
		yield return Left;
		yield return Right;
		yield return Cross;
	}

	public static IEnumerable<string> All(Predicate<string> fn)
	{
		foreach (var item in All())
		{
			if (fn(item)) yield return item;
		}
	}
}