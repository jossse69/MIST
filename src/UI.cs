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
            var height = 6;
            var logheight = display.Height;
            var logwidth = 79;
            var surface = display.Map;

            // draw the background
            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    surface.SetCellAppearance(i, j, new ColoredGlyph(Color.White, Color.Black, ' '));
                    
                }
            }

            // draw the log's background
            for (var i = logwidth; i < width; i++)
            {
                for (var j = 0; j < logheight; j++)
                {
                    surface.SetCellAppearance(i, j, new ColoredGlyph(Color.White, Color.Black, ' '));
                    
                }
            }

            surface.DrawLine(new Point(logwidth, 0), new Point(logwidth, logheight), 179, Color.DarkGray,  Color.Black);
            surface.DrawLine(new Point(0, height), new Point(logwidth, height), 196, Color.DarkGray,  Color.Black);
            surface.SetCellAppearance(logwidth, height, new ColoredGlyph(Color.DarkGray, Color.Black, 180));
            surface.Print(0, 0, "HP: " + player.Fighter.HP + " / " + player.Fighter.maxHP);

            // draw the messages of the message bus
            var y = 0; // start drawing messages from the second row

            var messageCount = Messages.Count;
            var maxMessages = logheight;
            var startIndex = Math.Max(0, messageCount - maxMessages);
            var endIndex = messageCount;

            for (var index = startIndex; index < endIndex; index++)
            {
                var message = Messages[index];
                surface.Print(logwidth + 1, y, message);
                y++;
            }
        }

        public void SendMessage(string message)
        {
            Messages.Add(message);
        }
    }
}