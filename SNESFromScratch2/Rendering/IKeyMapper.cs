using System.Windows.Forms;
using SNESFromScratch2.SNESSystem;

namespace SNESFromScratch2.Rendering
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