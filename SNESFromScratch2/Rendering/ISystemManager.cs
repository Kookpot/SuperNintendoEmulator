using SNESFromScratch2.SNESSystem;

namespace SNESFromScratch2.Rendering
{
    public interface ISystemManager
    {
        ISNESSystem Load(string fileName);
        void Write(string fileName, ISNESSystem system);
    }
}