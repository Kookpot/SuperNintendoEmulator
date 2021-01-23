using System.IO;
using Newtonsoft.Json;
using SNESFromScratch2.SNESSystem;

namespace SNESFromScratch2.Rendering
{
    public class SystemMananger : ISystemManager
    {
        public ISNESSystem Load(string fileName)
        {
            using (var file = File.OpenText(fileName))
            {
                var reader = new JsonTextReader(file);
                JsonSerializer serializer = new JsonSerializer
                {
                    ContractResolver = new CustomContractResolver(),
                    ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects
                };
                serializer.Converters.Add(new TestConverter());
                return serializer.Deserialize<ISNESSystem>(reader);
            }
        }

        public void Write(string fileName, ISNESSystem system)
        {
            StreamWriter file = File.CreateText(fileName);
            var writer = new JsonTextWriter(file);
            var serializer = new JsonSerializer
            {
                ContractResolver = new CustomContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };
            serializer.Serialize(writer, system);
            writer.Close();
            file.Close();
            file.Dispose();
        }
    }
}