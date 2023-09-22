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

        private readonly ColoredGlyph WALL = new ColoredGlyph(Color.AnsiBlue, Color.DarkBlue, '#');
        private readonly ColoredGlyph WALL_LIT = new ColoredGlyph(Color.DarkGray, Color.Brown, '#');
        private readonly ColoredGlyph FLOOR = new ColoredGlyph(Color.DarkBlue, Color.Black, '.');
        private readonly ColoredGlyph FLOOR_LIT = new ColoredGlyph(Color.Purple, Color.Black, '.');
        private readonly ColoredGlyph UNDEFINED = new ColoredGlyph(Color.White, Color.Black, '?');
        private readonly ArrayView<Tile> _tiles;
        private readonly IFOV _fov;

        private int _moveTimer = 0;

        /// <summary>
        ///  a 2d array of TileTypes for maps
        /// <param name="Width">the width of the map</param>
        /// <param name="Height">the height of the map</param>
        /// </summary>
        public Map(int width, int height) : base(width, height)
        {
            _tiles = new ArrayView<Tile>(width, height);
            _fov = new RecursiveShadowcastingFOV(new LambdaTranslationGridView<Tile, bool>(_tiles, x => x.BlocksView));
        }

        /// <summary>
        /// generates a map with a border and random tiles
        /// <param name="width">the width of the map</param>
        /// <param name="height">the height of the map</param>
        /// </summary>
        public static Map GenerateMap(int width, int height)
        {
            var map = new Map(width, height);
            Random random = new Random();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Generate a random tile
                    Tile tile;

                    // If it's a border tile, set it as a wall
                    if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                    {
                        tile = new Tile(TileType.Wall);
                    }
                    else
                    {
                        // 20% chance of a wall
                        if (random.Next(100) < 20)
                        {
                            tile = new Tile(TileType.Wall);
                        }
                        else
                        {
                            tile = new Tile(TileType.Floor);
                        }
                    }

                    // Set the tile in the map
                    map[x, y] = tile;
                }
            }
            return map;
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
                    player.TryToMove(dx, dy, this);
                    UpdateFOV(player.Position.X, player.Position.Y, radius: 5);
                    Draw();
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