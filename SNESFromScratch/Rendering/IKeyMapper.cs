using System.Windows.Forms;
using SNESFromScratch.SNESSystem;

namespace SNESFromScratch.Rendering
{
    public interface IKeyMapper
    {
        SNESButton? Map(Keys key);
        Keys? ReverseMap(SNESButton key);
        void ChangeMap(Keys key, SNESButton button);
        void Clear();
        void SaveChanges();
    }
}