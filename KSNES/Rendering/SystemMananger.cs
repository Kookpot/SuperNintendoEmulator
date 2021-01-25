using System.IO;
using Newtonsoft.Json;
using KSNES.SNESSystem;

namespace KSNES.Rendering
{
    public class SystemMananger : ISystemManager
    {
        public ISNESSystem Load(string fileName)
        {
            using (StreamReader file = File.OpenText(fileName))
            {
                var reader = new JsonTextReader(file);
                JsonSerializer serializer = GetSerializer();
                serializer.Converters.Add(new TestConverter());
                return serializer.Deserialize<ISNESSystem>(reader);
            }
        }

        public void Write(string fileName, ISNESSystem system)
        {
            StreamWriter file = File.CreateText(fileName);
            var writer = new JsonTextWriter(file);
            JsonSerializer serializer = GetSerializer();
            serializer.Serialize(writer, system);
            writer.Close();
            file.Close();
            file.Dispose();
        }

        private static JsonSerializer GetSerializer()
        {
            return new JsonSerializer
            {
                ContractResolver = new CustomContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };
        }
    }
}