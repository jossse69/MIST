using GoRogue.MapViews;
using SadRogue.Primitives;
using GoRogue;
using SadConsole;

namespace MIST
{

    internal class Map
    {
        
        public ArrayMap2D<TileType>? MapArray;

        public ArrayMap2D<bool>? ExploredArray;

        public ArrayMap2D<bool>? VisbleArray;
        public readonly ColoredGlyph WALL = new ColoredGlyph(Color.AnsiBlue, Color.DarkBlue, '#');
        public readonly ColoredGlyph FLOOR = new ColoredGlyph(Color.DarkBlue, Color.Black, '.');

        public readonly ColoredGlyph UNDEFINED = new ColoredGlyph(Color.White, Color.Black, '?');

        /// <summary>
        /// a emun for what tile type it is
        /// </summary>
        public enum TileType
        {
            Floor,
            Wall
        }

        /// <summary>
        ///  a 2d array of TileTypes for maps
        /// <param name="Width">the width of the map</param>
        /// <param name="Height">the height of the map</param>
        /// </summary>
        public Map(int width, int height)
        {
            MapArray = new ArrayMap2D<TileType>(width, height);
        }

        /// <summary>
        /// generates a map with a border and random tiles
        /// <param name="Width">the width of the map</param>
        /// <param name="Height">the height of the map</param>
        /// </summary>
        public Map GenerateMap(int Width, int Height)
        {
            var map = new Map(Width, Height);
            Random random = new Random();
            for (int x = 0; x < this.MapArray.Width; x++)
            {
                for (int y = 0; y < this.MapArray.Height; y++)
                {
                    // Generate a random tile
                    TileType tileType = TileType.Floor;

                    // 20% chance of a wall
                    if (random.Next(100) < 20)
                    {
                        tileType = TileType.Wall;
                    }

                    // If it's a border tile, set it as a wall
                    if (x == 0 || x == this.MapArray.Width - 1 || y == 0 || y == this.MapArray.Height - 1)
                    {
                        tileType = TileType.Wall;
                    }

                    // Set the tile in the map
                    map.SetTile(x,y, tileType);
                }
            }
            return map;
        }
        /// <summary>
        /// sets a tile in the map
        /// </summary>
        /// <param name="x">the x coordinate to set the tile </param>
        /// <param name="y">the y coordinate to set the tile</param>
        /// <param name="tileType">what type of tile should be set</param>
        private void SetTile(int x, int y, TileType tileType)
        {
            // Set the tile in the map
            this.MapArray[x, y] = tileType;
        }

        public void DrawArray( CellSurface surface)
        {
            for (int x = 0; x < this.MapArray.Width; x++)
            {
                for (int y = 0; y < this.MapArray.Height; y++)
                {
                    if (TileType.Floor == this.MapArray[x, y])
                    {
                        surface.SetCellAppearance(x, y, FLOOR);
                    }
                    else if (TileType.Wall == this.MapArray[x, y])
                    {
                        surface.SetCellAppearance(x, y, WALL);
                    }
                    else
                    {
                        surface.SetCellAppearance(x, y, UNDEFINED);
                    }
                    
                }
            
            }
        }
    }
}