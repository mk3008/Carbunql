using MessagePack;

namespace Carbunql.Analysis;

/// <summary>
/// Provides serialization and deserialization functionality for objects implementing the IReadQuery interface.
/// </summary>
public static class Serializer
{
    /// <summary>
    /// Serializes the specified IReadQuery object into a byte array.
    /// </summary>
    /// <param name="query">The IReadQuery object to serialize.</param>
    /// <returns>The serialized byte array.</returns>
    public static byte[] Serialize(IReadQuery query)
    {
        return MessagePackSerializer.Serialize(query);
    }

    /// <summary>
    /// Deserializes a byte array into an object implementing the IReadQuery interface.
    /// </summary>
    /// <param name="json">The byte array to deserialize.</param>
    /// <returns>The deserialized object.</returns>
    public static IReadQuery Deserialize(byte[] json)
    {
        return MessagePackSerializer.Deserialize<IReadQuery>(json);
    }

    /// <summary>
    /// Deserializes a byte array into an object of the specified type implementing the IReadQuery interface.
    /// </summary>
    /// <typeparam name="T">The type of object to deserialize into, which must implement the IReadQuery interface.</typeparam>
    /// <param name="json">The byte array to deserialize.</param>
    /// <returns>The deserialized object of the specified type.</returns>
    public static T Deserialize<T>(byte[] json) where T : IReadQuery
    {
        return (T)MessagePackSerializer.Deserialize<IReadQuery>(json);
    }
}
