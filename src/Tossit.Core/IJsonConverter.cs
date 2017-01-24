namespace Tossit.Core
{
    /// <summary>
    /// Json converter interface to serialize/deserialize operations.
    /// </summary>
    public interface IJsonConverter
    {
        /// <summary>
        /// Serialize given object to json string.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <param name="obj">Instance of object.</param>
        /// <returns>Returns string as serailized from given object to json string.</returns>
        string Serialize<T>(T obj);
        /// <summary>
        /// Deserialize given json string.
        /// </summary>
        /// <typeparam name="T">Type of object to be deserialize.</typeparam>
        /// <param name="json">Json string to be deserialize.</param>
        /// <returns>Returns object as deserialized from json string to type of given T object.</returns>
        T Deserialize<T>(string json);
    }
}