using MessagePack;

namespace Carbunql.Analysis;

public static class Serializer
{
	public static byte[] Serialize(IReadQuery query)
	{
		return MessagePackSerializer.Serialize(query);
	}

	public static IReadQuery Deserialize(byte[] json)
	{
		return MessagePackSerializer.Deserialize<IReadQuery>(json);
	}

	public static T Deserialize<T>(byte[] json) where T : IReadQuery
	{
		return (T)MessagePackSerializer.Deserialize<IReadQuery>(json);
	}
}
