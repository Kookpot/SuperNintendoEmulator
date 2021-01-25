using System.Windows.Forms;
using KSNES.SNESSystem;

namespace KSNES.GUI
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