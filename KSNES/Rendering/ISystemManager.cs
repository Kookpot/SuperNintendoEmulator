using KSNES.SNESSystem;

namespace KSNES.Rendering
{
    public interface ISystemManager
    {
        ISNESSystem Load(string fileName);
        void Write(string fileName, ISNESSystem system);
    }
}