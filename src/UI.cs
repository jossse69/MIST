using SadRogue.Primitives;
using SadConsole;
using MIST.items;
using MIST.popups;
namespace MIST
{
    public class UI
    {
        public ScreenContainer display { get; set; }

        public List<string> Messages { get; set; }
        public static Point LastDirection { get; private set; }

        public bool ingame = true;

        public string state = "none";

        public string inaction = "none";

        public List<IItem> playerinventory = new List<IItem>();

        public List<IItem> equipment = new List<IItem>();
        public int selecteditemid = 0;

        public IItem? handitem = null;

        public int maxiventoryroom = 10;

        public int totalactioncost = 100;

        public popups.SelectListPopup? activeListpopup = null;
        public UI(ScreenContainer Display)
        {
            display = Display;
            Messages = new List<string>();
            playerinventory = new List<IItem>();
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
            if (handitem != null)
            {
            surface.Print(16, 0, "Holding: " + handitem.Object?.Info.name);
            }
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

                // add the arrow that is pointing to the selected item
                surface.Print(11, selecteditemid + 13, ">", Color.White);

                // if inventory is empty
                if (playerinventory.Count > 0)
                {
                    foreach (var item in playerinventory)
                    {
                        if (item != null)
                        {
                            // if item is the equipped item
                            if (handitem == item)
                            {
                                surface.Print(13, i + 13, item.Object?.Info.name, Color.Gold);
                            }
                            else
                            {
                                surface.Print(13, i + 13, item.Object?.Info.name, Color.White);
                            }
                            i++;
                        }
                    }
                }

            }

            // draw the active popup
            if (activeListpopup != null)
            {
                activeListpopup.draw(ScreenContainer.Instance);
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
                foreach (var item in Map.items)
                {
                    if (item.Object?.Position == new Point(player.Position.X + x, player.Position.Y + y))
                    {
                        // if the inventory is full, return
                        if (playerinventory.Count >= maxiventoryroom)
                        {
                            SendMessage("You can't carry more stuff!");
                            Draw(player);
                            return;
                        }


                        SendMessage("You pick up '" + item.Object.Info.name + "'.");
                        Draw(player);
                        item.Object.MoveTo(new Point(-1, -1));
                        inaction = "none";
                        state = "none";
                        ingame = true;

                        // add the item to the inventory
                        playerinventory.Add(item);
                    }
                }
            }
            else if (inaction == "interact")
            {
                // loop through all chests and see if we can open it
                foreach (var chest in Map.chests)
                {
                    

                    if (chest.Position == new Point(player.Position.X + x, player.Position.Y + y))
                    {
                        // check if the chest is open
                        if (chest.open == false)
                        {
                        SendMessage("You open the chest. it contains:");
                        // if has loot
                        if (chest.loot.Count > 0)
                        {
                            foreach (var item in chest.loot)
                            {
                                SendMessage("'" + item.Object?.Info.name +"'");
                                playerinventory.Add(item);
                                break;
                            }
                        }
                        else
                        {
                            SendMessage("oops... nothing here...");
                        }
                        chest.open = true;
                        chest.Appearance = new ColoredGlyph(Color.SaddleBrown, Color.Black, 251);
                        inaction = "none";
                        state = "none";
                        ingame = true;
                        Draw(player);
                        }
                    }
                }
            }
        }

        public void openitempopup(int id)
        {
            // inventory is empty, return
            if (playerinventory.Count == 0 || id >= playerinventory.Count)
            {
                return;
            }

            // if the item is not in the inventory, return
            if (playerinventory[id] == null)
            {
                return;
            }

            var choices = new List<string>();
            choices.Add("drop");
            choices.Add("look");

            // if the item is a  Consumable, add it to the inventory
            if (playerinventory[id].Type == ItemType.Consumable)
            {
                choices.Add("use");
            }
            else
            {
                // if this item is equipped, add an option to unequip
                if (handitem == playerinventory[id])
                {
                    choices.Add("unequip");
                }
                else{
                    choices.Add("equip");
                }
            }

            activeListpopup = new popups.SelectListPopup("Inventory", new Rectangle(10, 10, 20, 20), choices);
        }

        public void moveitemcursor(int idx)
        {
            // if theres is a List popup, move its cursor
            if (activeListpopup != null)
            {
                activeListpopup.selecteditemid = Math.Clamp(idx + activeListpopup.selecteditemid, 0, activeListpopup.options.Count - 1);
            }
            else if (inaction == "inventory")
            {
                selecteditemid = Math.Clamp(idx + selecteditemid, 0, maxiventoryroom - 1);
            }
        }
    }

    
}