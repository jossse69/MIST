using SadRogue.Primitives;
using SadConsole;

namespace MIST.popups
{
    public class popup
    {
        public string title;

        public Rectangle size;

        public popup(string Title, Rectangle Size)
        {
            Title = title;
            size = Size;
        }

        public virtual void draw(ScreenContainer ScreenContainer)
        {
            ScreenContainer.Map.Surface.DrawBox(size,ShapeParameters.CreateStyledBoxFilled(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.DarkGray, Color.Black), new ColoredGlyph(Color.Black, Color.Black)));
            ScreenContainer.Map.Surface.Print(size.X + 3, size.Y, title, Color.Black, Color.DarkGray);
        }
    }
}