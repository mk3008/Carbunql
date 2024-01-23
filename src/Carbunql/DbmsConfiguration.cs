namespace Carbunql;

public static class DbmsConfiguration
{
	public static string PlaceholderIdentifier { get; set; }
	//public static string CoalesceFunctionName { get; set; }

	static DbmsConfiguration()
	{
		PlaceholderIdentifier = ":";
		//CoalesceFunctionName = "coalesce";
	}
}