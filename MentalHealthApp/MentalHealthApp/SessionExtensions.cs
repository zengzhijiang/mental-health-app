using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

public static class SessionExtensions
{

    /*
     * Serializes to a json object
     */
    public static void SetSerializedObject(this ISession session, string key, object value)
    {
        session.SetString(key, JsonConvert.SerializeObject(value));
    }
    
    /*
     * Deserializes a json object
     */
    public static T GetDeserialized<T>(this ISession session, string key)
    {
        var value = session.GetString(key);

        return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
    }
}