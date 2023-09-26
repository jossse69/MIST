using SadRogue.Primitives;
using SadConsole;

namespace MIST
{
    public class popup
    {
        public string title;

        public popupType type;

        public popup(string Title, popupType Type)
        {
            Title = title;
            type = Type;
        }

        public void draw(IScreenSurface surface)
        {
            
        }
    }
}