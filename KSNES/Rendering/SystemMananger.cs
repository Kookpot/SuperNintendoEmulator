namespace KSNES.Rendering;

public class SystemMananger : ISystemManager
{
    public ISNESSystem Load(string fileName)
    {
        using FileStream file = File.OpenRead(fileName);
        return JsonSerializer.Deserialize<ISNESSystem>(file)!;
    }

    public void Write(string fileName, ISNESSystem system)
    {
        using FileStream file = File.OpenWrite(fileName);
        JsonSerializer.Serialize(file, system);
    }

    //private static JsonSerializer GetSerializer()
    //{
    //    return new JsonSerializer
    //    {
    //        ContractResolver = new CustomContractResolver(),
    //        ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
    //        PreserveReferencesHandling = PreserveReferencesHandling.Objects
    //    };
    //}
}