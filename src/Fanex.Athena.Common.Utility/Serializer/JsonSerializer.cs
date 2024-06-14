namespace Fanex.Athena.Common.Utility.Serializer
{
    using System.IO;
    using System.Runtime.Serialization.Json;
    using System.Text;

    public class JsonSerializer : ISerializer
    {
        public T Deserialize<T>(string data)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
            var serializer = new DataContractJsonSerializer(typeof(T));

            return (T)serializer.ReadObject(stream);
        }

        public string Serialize<T>(T data)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            var stream = new MemoryStream();

            serializer.WriteObject(stream, data);
            stream.Position = 0;
            var streamReader = new StreamReader(stream);

            return streamReader.ReadToEnd();
        }
    }
}