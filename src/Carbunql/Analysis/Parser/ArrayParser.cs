namespace Carbunql.Analysis.Parser;

public class ArrayParser
{
	public static List<string> Parse(string text)
	{
		var r = new SqlTokenReader(text);
		var q = Parse(r);
		return q;
	}

	public static List<string> Parse(ITokenReader r)
	{
		var lst = new List<string>();

		r.Read("(");
		do
		{
			r.ReadOrDefault(",");
			lst.Add(r.Read());
		} while (r.Peek() != ")");

		r.Read(")");

		return lst;
	}
}