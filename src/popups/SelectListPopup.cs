using SadRogue.Primitives;
using SadConsole;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace MIST.popups
{
    public class SelectListPopup : popup
    {
        public List<string> options;
        public int selecteditemid = 0;
        public SelectListPopup(string Title, Rectangle Size, List<string> Options) : base(Title, Size)
        {
            options = Options;
        }

        public override void draw(ScreenContainer ScreenContainer)
        {
            ScreenContainer.Map.Surface.DrawBox(size,ShapeParameters.CreateStyledBoxFilled(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.DarkGray, Color.Black), new ColoredGlyph(Color.Black, Color.Black)));
            ScreenContainer.Map.Surface.Print(size.X + 3, size.Y, title, Color.Black, Color.DarkGray);

            // draw each item
            var y = size.Y;

            // draw the arrow
            ScreenContainer.Map.Surface.SetCellAppearance(size.X + 1, size.Y + 1 + selecteditemid, new ColoredGlyph(Color.White, Color.Black, '>'));


            foreach (var option in options)
            {
                ScreenContainer.Map.Surface.Print(size.X + 3, y + 1 , option, Color.White, Color.Black);
                y++;

            }
        }

    }
}