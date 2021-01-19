using SNESFromScratch.SNESSystem;

namespace SNESFromScratch.Rendering
{
    public interface ISystemManager
    {
        ISNESSystem Load(string fileName);
        void Write(string fileName, ISNESSystem system);
    }
}