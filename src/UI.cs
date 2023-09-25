using SadRogue.Primitives;
using SadConsole;
using SadRogue.Primitives.GridViews;
using SadConsole.Input;
using GoRogue.Messaging;
using GoRogue;
using SadConsole.Ansi;
using MIST.items;
namespace MIST
{
    internal class UI
    {
        public ScreenContainer display { get; set; }

        public List<string> Messages { get; set; }
        public static Point LastDirection { get; private set; }

        public bool ingame = true;

        public string state = "none";

        public string inaction = "none";

        public List<Item> playerinventory = new List<Item>();
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
            var maxiventoryroom = 10;

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

            // if we are also in the inventory, draw it
            if (inaction == "inventory")
            {
                surface.DrawBox(new Rectangle(10, 10, 60, 20),ShapeParameters.CreateStyledBoxFilled(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.DarkGray, Color.Black), new ColoredGlyph(Color.Black, Color.Black)));
                surface.Print(13, 10, "Inventory", Color.Black, Color.DarkGray);
                //draw each item
                var i = 0;

                for (var j = 0; j < maxiventoryroom; j++)
                {
                    surface.Print(13, j + 13, "-", Color.DarkGray);
                }

                // if inventory is empty
                if (playerinventory.Count > 0)
                {
                    foreach (var item in playerinventory)
                    {
                        if (item != null)
                        {
                            surface.Print(13, i + 13, item.Object?.Info.name, Color.White);
                            i++;
                        }
                    }
                }

            }
        }

        public void SendMessage(string message)
        {
            Messages.Add(message);
        }

        public void AskDirection()
        {
            
            ingame = false;
            state = "askdirection";
            SendMessage("Please enter a direction:");
            SendMessage("Press a movement key to input.");
        }

        public void ReciveDirection(int x, int y, GameObject player)
        {
            LastDirection = new Point(x, y);

            if (inaction == "pickup")
            {
                // loop through all items and see if we can pickup the item at the position of the direction
                foreach (var item in display.Map.items)
                {
                    if (item.Object?.Position == new Point(player.Position.X + x, player.Position.Y + y))
                    {
                        SendMessage("You pick up '" + item.Object.Info.name + "'.");
                        Draw(player);
                        item.Object.MoveTo(new Point(-1,-1));
                        inaction = "none";
                        state = "none";
                        ingame = true;

                        // add the item to the inventory
                        playerinventory.Add(item);
                    }
                }
            }
        }
    }
}