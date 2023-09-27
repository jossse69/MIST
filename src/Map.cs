using SadRogue.Primitives;
using SadConsole;
using SadRogue.Primitives.GridViews;
using GoRogue.FOV;
using SadConsole.Input;
using SadConsole.Effects;
using GoRogue;
using MIST.items;

namespace MIST
{
    public class Map : SadConsole.Console
    {
        public Tile this[Point pos] { get { return _tiles[pos]; } set { _tiles[pos] = value; } }
        public Tile this[int x, int y] { get { return this[(x, y)]; } set { this[(x, y)] = value; } }
        public Tile this[int index] { get { return this[Point.FromIndex(index, Width)]; } set { this[Point.FromIndex(index, Width)] = value; } }

        public Point start { get; private set; }

        private readonly ColoredGlyph WALL = new ColoredGlyph(Color.AnsiBlue, Color.DarkBlue, '#');
        private readonly ColoredGlyph WALL_LIT = new ColoredGlyph(Color.DarkGray, Color.Gray, '#');
        private readonly ColoredGlyph FLOOR = new ColoredGlyph(Color.DarkBlue, Color.Black, '.');
        private readonly ColoredGlyph FLOOR_LIT = new ColoredGlyph(Color.Purple, Color.Black, '.');
        private readonly ColoredGlyph UNDEFINED = new ColoredGlyph(Color.White, Color.Black, '?');
        private readonly ArrayView<Tile> _tiles;
        private readonly IFOV _fov;

        private Point PreviousMouseScreenPosition;
        private List<GameObject> _objects;
        private int _moveTimer = 0;
        public static List<IItem> items = new List<IItem>();
        public static List<Chest> chests = new List<Chest>();

        /// <summary>
        ///  a 2d array of TileTypes for maps
        /// <param name="Width">the width of the map</param>
        /// <param name="Height">the height of the map</param>
        /// </summary>
        public Map(int width, int height, List<GameObject> objects) : base(width, height)
        {
            _tiles = new ArrayView<Tile>(width, height);
            _fov = new RecursiveShadowcastingFOV(new LambdaTranslationGridView<Tile, bool>(_tiles, x => !x.BlocksView));
            _objects = objects;
        }


        public void additem(items.IItem item)
        {
            items.Add(item);
        }

        public IFOV GetFOV()
        {
            return _fov;
        }

        public ArrayView<Tile> GetTiles()
        {
            return _tiles;
        }

        /// <summary>
        /// generates a map with a border and random tiles
        /// <param name="width">the width of the map</param>
        /// <param name="height">the height of the map</param>
        /// </summary>
        public static Map GenerateMap(int width, int height, List<GameObject> objects, UI ui, List<items.IItem> Items)
        {
            var map = new Map(width, height, objects);
            items = Items;

            // set all tiles in the map to be walls
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    map[x, y] = new Tile(TileType.Wall); // floor for debuging
                }
            }

            var rooms = new List<Rectangle>();

            var MAX_ROOM_SIZE = 6;
            var MIN_ROOM_SIZE = 3;
            var MAX_ROOMS = 15;

            Random random = new Random();
            
        
            for (int i = 0; i < MAX_ROOMS; i++)
            {
                int roomWidth = random.Next(MIN_ROOM_SIZE, MAX_ROOM_SIZE + 1);
                int roomHeight = random.Next(MIN_ROOM_SIZE, MAX_ROOM_SIZE + 1);

                int x = random.Next(0, Math.Clamp(width - roomWidth, 0, 78 - roomWidth));
                int y = random.Next(8, height  - roomHeight);

                var newRoom = new Rectangle(x, y, roomWidth, roomHeight);

                // Check if the new room overlaps with any existing rooms
                bool isOverlapping = rooms.Any(room => newRoom.Intersects(room));
                if (!isOverlapping)
                {
                    // apply the new room to the map
                    for (x = newRoom.X; x < newRoom.X + newRoom.Width; x++)
                    {
                        for (y = newRoom.Y; y < newRoom.Y + newRoom.Height; y++)
                        {
                            // if inside bounds of the map, set the tile
                            if (x >= 0 && x < width && y >= 0 && y < height)
                            {
                                map[x, y] = new Tile(TileType.Floor);
                            }
                        }
                    }


                    rooms.Add(newRoom);
                }
            }

            for (int i = 0; i < rooms.Count - 1; i++)
            {
                var roomA = rooms[i];
                var roomB = rooms[i + 1];

                // Calculate the center points of each room
                var centerA = new Point(roomA.X + roomA.Width / 2, roomA.Y + roomA.Height / 2);
                var centerB = new Point(roomB.X + roomB.Width / 2, roomB.Y + roomB.Height / 2);

                // Connect the centers with a series of horizontal and vertical corridors
                if (random.Next(2) == 0)
                {
                    // Horizontal corridor first, then vertical
                    ConnectHorizontalTunnel(map, centerA.X, centerB.X, centerA.Y);
                    ConnectVerticalTunnel(map, centerA.Y, centerB.Y, centerB.X);
                }
                else
                {
                    // Vertical corridor first, then horizontal
                    ConnectVerticalTunnel(map, centerA.Y, centerB.Y, centerA.X);
                    ConnectHorizontalTunnel(map, centerA.X, centerB.X, centerB.Y);
                }
            }

            // apply a border to the map
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x == 0 || x == width + 78 || y <= 5 || y == height)
                    {
                        map[x, y] = new Tile(TileType.Wall);
                    }
                }
            }


            // set start at the center of the first room
            map.start = new Point(rooms[0].Center.X, rooms[0].Center.Y);

            // create some monsters in the rooms
            foreach (var room in rooms)
            {
                // if its not the first room, create a monster
                if (rooms.IndexOf(room) != 0)
                {
                    map.CreateMonster(map, room, ui);

                    // small chance to swant a chest with loot
                    if (random.Next(2) == 0)
                    {
                        var chest = new Chest(new Point(room.X + room.Width / 2, room.Y + room.Height / 2), map, new Info("Chest", "A brown wooden chest, has loot!"), ui);
                        chest.loot = Chest.GenerateLoot(map);
                        chests.Add(chest);
                    }
                }
            }


            return map;
        }


        private void CreateMonster(Map map, Rectangle room, UI ui)
        {
           
            var random = new Random();
            for (int x = 0; x < random.Next(0, 4); x++)
            {
                var trys = 4;
                while (trys != 0)
                {
                    var monsterX = random.Next(room.X, room.X + room.Width);
                    var monsterY = random.Next(room.Y, room.Y + room.Height);
                    if (map[monsterX, monsterY].TileType == TileType.Floor)
                    {
                        var monster = new GameObject(new ColoredGlyph(Color.White, Color.Black, '!'), new Point(monsterX, monsterY), map,new Fighter(1, 1, 1, 1, ui, monsterType.UNDEFINED), new Info("Undefined", "Undefined"), null, ui);
                        // 20% to be an smlie, alse its a spider
                        if (random.Next(20) == 0)
                        {
                            monster = new GameObject(new ColoredGlyph(Color.LimeGreen, Color.Black, 'S'), new Point(monsterX, monsterY), map, new Fighter(15, 15, 3, 1, ui, monsterType.slime), new Info("Slime", "a mass of living gelatic green goo."), new AI.BasicMonsterAI(135), ui);
                        }
                        else
                        {
                            monster = new GameObject(new ColoredGlyph(Color.Red, Color.Black, 's'), new Point(monsterX, monsterY), map, new Fighter(10, 10, 2, 1, ui, monsterType.insect), new Info("Spider", "a oddly big arachnid. spooky!"), new AI.BasicMonsterAI(84), ui);
                        }
                        // add it to the list
                        map._objects.Add(monster);
                        break;
                    }
                    else
                    {
                        trys--;
                    }
                }
           
            }
        }



        private static void ConnectHorizontalTunnel(Map map, int startX, int endX, int y)
        {
            for (int x = Math.Min(startX, endX); x <= Math.Max(startX, endX); x++)
            {
                map._tiles[x, y] = new Tile(TileType.Floor);
            }
        }

        private static void ConnectVerticalTunnel(Map map, int startY, int endY, int x)
        {
            for (int y = Math.Min(startY, endY); y <= Math.Max(startY, endY); y++)
            {
                map._tiles[x, y] = new Tile(TileType.Floor);
            }
        }



            

        /// <summary>
        /// draws the map
        /// </summary>
        /// <param name="surface"> what surface to draw to</param>
        public void Draw()
        {
            for (int x = 0; x < _tiles.Width; x++)
            {
                for (int y = 0; y < _tiles.Height; y++)
                {
                    // not explored, don't draw
                    if (_tiles[x, y].Explored == false) {
                        Surface.SetCellAppearance(x, y, new ColoredGlyph(Color.Black, Color.Black, ' '));
                        continue;
                    }

                    if (TileType.Floor == _tiles[x, y].TileType)
                    {
                        // if lit
                        if (_tiles[x, y].Visible)
                        {
                            Surface.SetCellAppearance(x, y, FLOOR_LIT);
                        }
                        else
                        {
                            Surface.SetCellAppearance(x, y, FLOOR);
                        }
                    }
                    else if (TileType.Wall == _tiles[x, y].TileType)
                    {
                        // if lit
                        if (_tiles[x, y].Visible)
                        {
                            Surface.SetCellAppearance(x, y, WALL_LIT);
                        }
                        else
                        {
                            Surface.SetCellAppearance(x, y, WALL);
                        }
                    }
                    else
                    {
                        Surface.SetCellAppearance(x, y, UNDEFINED);
                    }
                }
            }
        }

        /// <summary>
        /// updates the FOV
        /// </summary>
        /// <param name="x">the x coordinate to were the source is</param>
        /// <param name="y">the y coordinate to were the source is</param>
        public void UpdateFOV(int x, int y, int radius)
        {
            _fov.Calculate((x, y), radius, Distance.Euclidean);

            foreach (var pos in _fov.NewlySeen)
            {
                _tiles[pos].Visible = true;
                _tiles[pos].Explored = true;
            }

            foreach (var pos in _fov.NewlyUnseen)
            {
                _tiles[pos].Visible = false;
            }
        }

        public override bool ProcessKeyboard(Keyboard keyboard)
        {
            var processed = false;
            var player = ScreenContainer.Instance.Player;
            var UI = ScreenContainer.Instance.UI;
            // grab key
            if (keyboard.IsKeyPressed(Keys.G)){
                UI.inaction = "pickup";
                UI.AskDirection();
                UI.Draw(player);

            };

            // interact key
            if (keyboard.IsKeyPressed(Keys.E))
            {
                UI.inaction = "interact";
                UI.AskDirection();
                UI.Draw(player);
            };

            // inventory key
            if (keyboard.IsKeyPressed(Keys.I))
            {
                UI.inaction = "inventory";
                UI.ingame = false;
                UI.Draw(player);
            };

             // on ESC key, cancel
            if (keyboard.IsKeyPressed(Keys.Escape))
            {
                if (UI.state == "askdirection")
                {
                    UI.inaction = "none";
                    UI.state = "none";
                    UI.ingame = true;
                    UI.SendMessage("Nevermind...");
                    UI.Draw(player);
                }
                else if (UI.inaction == "inventory")
                {
                    // theres is a popup in the ui, close it instead
                    if( UI.activeListpopup != null)
                    {
                        UI.activeListpopup = null;
                        UI.Draw(player);
                    }
                    else
                    {
                    UI.inaction = "none";
                    UI.state = "none";
                    UI.ingame = true;
                    }
                    processed = true;
                }
            }

            // move arrow of selecteditemid
            if(keyboard.IsKeyPressed(Keys.Up))
            {
                if (UI.inaction == "inventory")
                {
                    UI.moveitemcursor(-1);
                    UI.Draw(player);
                    
                }
            }

            if( keyboard.IsKeyPressed(Keys.Down))
            {
                if (UI.inaction == "inventory")
                {
                    UI.moveitemcursor(1);
                    UI.Draw(player);
                }
            }

            // enter for selected item
            if (keyboard.IsKeyPressed(Keys.Enter))
            {
                if (UI.inaction == "inventory")
                {
                    // there is a popup in the ui, close it instead
                    if (UI.activeListpopup != null)
                    {

                        // each case of what action to do
                        if (UI.activeListpopup.options[UI.activeListpopup.selecteditemid] == "drop")
                        {
                            // drop the item
                            UI.ingame = true;
                            UI.state = "none";
                            UI.inaction = "none";
                            var item = UI.playerinventory[UI.selecteditemid];
                            UI.SendMessage("You drop '" + item.Object?.Info.name + "' to the ground.");
                            if (UI.handitem == item)
                            {
                                UI.handitem = null;
                            }
                            UI.playerinventory.Remove(item);
                            items.Add(item);
                            item.Object.MoveTo(new Point(player.Position.X, player.Position.Y));
                        }
                        else if (UI.activeListpopup.options[UI.activeListpopup.selecteditemid] == "look")
                        {
                            UI.ingame = true;
                            UI.state = "none";
                            UI.inaction = "none";
                            var item = UI.playerinventory[UI.selecteditemid];
                            UI.SendMessage("You analyze '" + item.Object?.Info.name + "'.");
                            UI.SendMessage(item.Object.Info.description);
                        } else if (UI.activeListpopup.options[UI.activeListpopup.selecteditemid] == "use")
                        {
                            UI.ingame = true;
                            UI.state = "none";
                            UI.inaction = "none";
                            var item = UI.playerinventory[UI.selecteditemid];
                            UI.SendMessage("You use '" + item.Object?.Info.name + "'.");
                            item.Use();
                        } else if (UI.activeListpopup.options[UI.activeListpopup.selecteditemid] == "equip")
                        {
                            UI.ingame = true;
                            UI.state = "none";
                            UI.inaction = "none";
                            var item = UI.playerinventory[UI.selecteditemid];
                            UI.SendMessage("You equip '" + item.Object?.Info.name + "'.");
                            UI.handitem = item;
                        }
                        else if (UI.activeListpopup.options[UI.activeListpopup.selecteditemid] == "unequip")
                        {
                            UI.ingame = true;
                            UI.state = "none";
                            UI.inaction = "none";
                            var item = UI.playerinventory[UI.selecteditemid];
                            UI.SendMessage("You put away '" + item.Object?.Info.name + "'.");
                            UI.handitem = null;
                        }
                        UI.activeListpopup = null;
                        processed = true;
                    }
                    else
                    {
                    UI.openitempopup(UI.selecteditemid);
                    UI.Draw(player);
                    }
                }
            }

            // move player with numbpad
            var dx = 0;
            var dy = 0;

            if (keyboard.IsKeyDown(Keys.NumPad4) || keyboard.IsKeyDown(Keys.A))
            {
                dx = -1;
                processed = true;
            }
            else if (keyboard.IsKeyDown(Keys.NumPad6) || keyboard.IsKeyDown(Keys.D))
            {
                dx = 1;
                processed = true;
            }
            else if (keyboard.IsKeyDown(Keys.NumPad8) || keyboard.IsKeyDown(Keys.W))
            {
                dy = -1;
                processed = true;
            }
            else if (keyboard.IsKeyDown(Keys.NumPad2) || keyboard.IsKeyDown(Keys.S))
            {
                dy = 1;
                processed = true;
            }
            else if (keyboard.IsKeyDown(Keys.NumPad1))
            {
                dx = -1;
                dy = 1;
                processed = true;
            }
            else if (keyboard.IsKeyDown(Keys.NumPad3))
            {
                dx = 1;
                dy = 1;
                processed = true;
            }
            else if (keyboard.IsKeyDown(Keys.NumPad7))
            {
                dx = -1;
                dy = -1;
                processed = true;
            }
            else if (keyboard.IsKeyDown(Keys.NumPad9))
            {
                dx = 1;
                dy = -1;
                
                processed = true;
            }
            else
            {
            }


            if (processed)
            {   if (UI.ingame)
                {
                // move player if the movetimer expired
                if (_moveTimer <= 0)
                {
                    
                    if (player.Fighter.IsDead) return false;

                    // Do AI turn
                    foreach (var obj in _objects)
                    {
                        if (obj.AI == null) continue;

                        obj.AI.Energy += 100;
                        obj.AI.UpdateEnergy();

                        while (obj.AI.canspendenergy)
                        {
                            obj.AI.AITurn(obj, this, _objects, player); 
                            obj.AI.UpdateEnergy();
                        }
                    }

                    
                        player.MoveAndAttack(dx, dy, this, _objects);

                        UpdateFOV(player.Position.X, player.Position.Y, radius: 5);

                        Draw();

                        // set visble of map true (debuging purposes)
                        
                            for (var x = 0; x < _tiles.Width; x++)
                            {
                                for (var y = 0; y < _tiles.Height; y++)
                                {
                                //_tiles[x, y].Visible = true;
                                //_tiles[x, y].Explored = true;
                                }
                            }

                        if (items != null)
                        {
                            // draw item on floor
                            foreach (var item in items)
                            {
                                // dont cotinue if it doesn't exist
                                if (item.Object == null) continue;

                                // if not in FOV, don't draw
                                if (IsInFov((int)item.Object?.Position.X, (int)(item.Object?.Position.Y)) == false) continue;

                                // if there is no object, don't draw
                                if (item.Object == null) continue;
                                item.Object?.Draw();
                            }
                        } 

                        // make list of objects to draw
                        var drawList = _objects.ToList();


                        // sort so that dead objects are drawn last and only when visible on the map
                        drawList = drawList
                            .Where(a => a.IsVisible(this))
                            .OrderBy(obj => obj.Fighter?.IsDead)
                            .ToList();



                        // draw the list
                        for (var index = 0; index < drawList.Count; index++)
                        {
                            // also update the AI
                            var obj = drawList[index];
                            if (obj == null || obj.AI == null) continue;

                            

                            // if not in FOV, don't draw
                            if (_tiles[obj.Position.X, obj.Position.Y].Visible == false) continue;

                            drawList[index].Draw();
                        }
                        // draw player
                        player.Draw();

                        // draw chests
                        foreach (var chest in chests)
                        {
                            // if not in FOV, don't draw
                            if (chest.IsVisible(this) == false) continue;

                            chest.Draw();
                        }

                        // draw UI
                        UI.Draw(player);                    
                        _moveTimer = 9;
                    }
                }
                else
                {
                    // if we are asking for a direction
                    if (UI.state == "askdirection")
                    {
                        UI.ReciveDirection(dx, dy, player);
                    }

                
                }
            }
            

            return processed;
        }

        public override void Update(TimeSpan delta)
        {
            base.Update(delta);

            if (_moveTimer > 0)
            {
                _moveTimer--;
            }
        }

        public bool IsInFov(int x, int y)
        {
            // if in bounds of the map
            if (x > 0 && x < _tiles.Width && y > 0 && y < _tiles.Height)
            {
                return _fov.BooleanResultView[x, y];
            }

            return false;
        }

        public override bool ProcessMouse(MouseScreenObjectState state)
        {
            var processed = false;

            // get the mouse position in cells
            var mouseX = Math.Clamp(state.Mouse.ScreenPosition.X / this.Font.GlyphWidth, 0, this.Surface.Width - 1);
            var mouseY = Math.Clamp(state.Mouse.ScreenPosition.Y / this.Font.GlyphHeight, 0, this.Surface.Height - 1);

            if (state.Mouse.ScreenPosition != PreviousMouseScreenPosition)
            {              

                PreviousMouseScreenPosition = state.Mouse.ScreenPosition;
                processed = true;
            }

            if (state.Mouse.LeftClicked)
            {
                
                var ui = ScreenContainer.Instance.UI;
                var player = ScreenContainer.Instance.Player;
                // check if the mouse is in the FOV
                if (IsInFov(mouseX, mouseY))
                {
                    // check if we clicked on a object
                    foreach (var obj in _objects)
                    {
                        if (mouseX == obj.Position.X && mouseY == obj.Position.Y)
                        {
                            // display info on UI
                            ui.SendMessage("You analyse "+ obj.Info.name + ".");
                            ui.SendMessage(obj.Info.description);
                            ui.Draw(player);
                            processed = true;
                            break;
                        }
                    }

                    // check chests also
                    foreach (var chest in chests)
                    {
                        if (mouseX == chest.Position.X && mouseY == chest.Position.Y)
                        {
                            ui.SendMessage("You analyse "+ chest.Info.name + ".");
                            ui.SendMessage(chest.Info.description);
                            processed = true;
                            break;
                        }
                    }


                }

            }

            return processed;

        }
    }
}