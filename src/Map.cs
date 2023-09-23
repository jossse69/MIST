using SadRogue.Primitives;
using SadConsole;
using SadRogue.Primitives.GridViews;
using GoRogue.FOV;
using SadConsole.Input;

namespace MIST
{
    internal class Map : SadConsole.Console
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

        private List<GameObject> _objects;


        private int _moveTimer = 0;

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

        /// <summary>
        /// generates a map with a border and random tiles
        /// <param name="width">the width of the map</param>
        /// <param name="height">the height of the map</param>
        /// </summary>
        public static Map GenerateMap(int width, int height, List<GameObject> objects)
        {
            var map = new Map(width, height, objects);

            // set all tiles in the map to be walls
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    map[x, y] = new Tile(TileType.Wall);
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

                int x = random.Next(0, width - roomWidth);
                int y = random.Next(0, height - roomHeight);

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
                    if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
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
                    map.CreateMonster(map, room);
                }
            }


            return map;
        }


        private void CreateMonster(Map map, Rectangle room)
        {
           
            var random = new Random();
            for (int x =0; x < random.Next(0, 4); x++)
            {
                var success = false;
                while (!success)
                {
                    var monsterX = random.Next(room.X, room.X + room.Width);
                    var monsterY = random.Next(room.Y, room.Y + room.Height);
                    if (map[monsterX, monsterY].TileType == TileType.Floor)
                    {
                        var monster = new GameObject(new ColoredGlyph(Color.White, Color.Black, '!'), new Point(monsterX, monsterY), map,new Fighter(1, 1, 1, 1), new Info("Undefined", "Undefined", monsterType.UNDEFINED));
                        // 20% to be an smile, alse its a spider
                        if (random.Next(20) == 0)
                        {
                            monster = new GameObject(new ColoredGlyph(Color.LimeGreen, Color.Black, 'S'), new Point(monsterX, monsterY), map, new Fighter(15, 15, 2, 1), new Info("Smile", "a mass of gelatic green goo... that is alive, it digest any food it comes across.", monsterType.smlie));
                        }
                        else
                        {
                            monster = new GameObject(new ColoredGlyph(Color.Red, Color.Black, 's'), new Point(monsterX, monsterY), map, new Fighter(5, 5, 1, 1), new Info("Spider", "a oddly big arachnid. its black and has a big skull-shaped symbol on it's abdomen, that probably means something.", monsterType.insect));
                        }
                        success = true;
                        // add it to the list
                        map._objects.Add(monster);
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
                    if (_tiles[x, y].Explored == false) continue;

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

            // move player with numbpad
            var dx = 0;
            var dy = 0;

            if (keyboard.IsKeyDown(Keys.NumPad4))
            {
                dx = -1;
                processed = true;
            }
            else if (keyboard.IsKeyDown(Keys.NumPad6))
            {
                dx = 1;
                processed = true;
            }
            else if (keyboard.IsKeyDown(Keys.NumPad8))
            {
                dy = -1;
                processed = true;
            }
            else if (keyboard.IsKeyDown(Keys.NumPad2))
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

            if (processed)
            {
                // move player if the movetimer expired
                if (_moveTimer <= 0)
                {
                    var player = ScreenContainer.Instance.Player;
                    player.MoveAndAttack(dx, dy, this, _objects);
                    UpdateFOV(player.Position.X, player.Position.Y, radius: 5);

                    Draw();
                    
                    // make list of objects to draw
                    var drawList = new List<GameObject>();
                    drawList.AddRange(_objects);
                    
                    // sort so that dead objects are drawn last
                    drawList = drawList.OrderBy(obj => obj.fighter?.IsDead).ToList();

                    // draw the list
                    for (var obj = 0; obj < drawList.Count; obj++)
                    {
                        // if not in FOV, don't draw
                        if (_fov.BooleanResultView[drawList[obj].Position.X, drawList[obj].Position.Y] == false) continue;

                        drawList[obj].Draw();
                    }
                    player.Draw();
                    _moveTimer = 5;
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

    }
}