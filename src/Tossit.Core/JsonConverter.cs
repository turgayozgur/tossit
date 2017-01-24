using Newtonsoft.Json;

namespace Tossit.Core
{
    /// <summary>
    /// Json converter.
    /// </summary>
    public class JsonConverter : IJsonConverter
    {
        /// <summary>
        /// Serialize given object to json string.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <param name="obj">Instance of object.</param>
        /// <returns>Returns string as serailized from given object to json string.</returns>
        public string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// Deserialize given json string.
        /// </summary>
        /// <typeparam name="T">Type of object to be deserialize.</typeparam>
        /// <param name="json">Json string to be deserialize.</param>
        /// <returns>Returns object as deserialized from json string to type of given T object.</returns>
        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
