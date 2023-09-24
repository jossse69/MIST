using SadRogue.Primitives;
using SadConsole;
using SadRogue.Primitives.GridViews;
using SadConsole.Input;
using GoRogue.Messaging;
using GoRogue;
namespace MIST
{
    internal class UI
    {
        public ScreenContainer display { get; set; }

        public List<string> Messages { get; set; }


        public UI(ScreenContainer Display)
        {
            display = Display;
            Messages = new List<string>();
        }

        public void Draw(GameObject player)
        {
            var width = display.Width;
            var height = 4;
            var surface = display.Map;

            // draw the background
            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    surface.SetCellAppearance(i, j, new ColoredGlyph(Color.White, Color.Black, ' '));
                    
                }
            }

            surface.DrawLine(new Point(39, 0), new Point(39, height), 179, Color.DarkGray,  Color.Black);
            surface.DrawLine(new Point(0, height), new Point(width, height), 196, Color.DarkGray,  Color.Black);
            surface.SetCellAppearance(39, height, new ColoredGlyph(Color.DarkGray, Color.Black, 193));
            surface.Print(0, 0, "HP: " + player.Fighter.HP + " / " + player.Fighter.maxHP);

            // draw the messages of the message bus
            var y = 0; // start drawing messages from the second row

            var messageCount = Messages.Count;
            var maxMessages = height;
            var startIndex = Math.Max(0, messageCount - maxMessages);
            var endIndex = messageCount;

            for (var index = startIndex; index < endIndex; index++)
            {
                var message = Messages[index];
                surface.Print(40, y, message);
                y++;
            }
        }

        public void SendMessage(string message)
        {
            Messages.Add(message);
        }
    }
}