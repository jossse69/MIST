using GoRogue.MapViews;
using SadRogue.Primitives;
using SadConsole;
using SadRogue.Primitives.GridViews;
using static MIST.Map;


namespace MIST
{



    internal class Map 
    {
        
        public ArrayMap2D<Tile> MapArray;

        public IMapView<bool> FOVMap;


        public readonly ColoredGlyph WALL = new ColoredGlyph(Color.AnsiBlue, Color.DarkBlue, '#');
        public readonly ColoredGlyph WALL_LIT = new ColoredGlyph(Color.DarkGray, Color.Brown, '#');
        public readonly ColoredGlyph FLOOR = new ColoredGlyph(Color.DarkBlue, Color.Black, '.');

        public readonly ColoredGlyph FLOOR_LIT = new ColoredGlyph(Color.Purple, Color.Black, '.');

        public readonly ColoredGlyph UNDEFINED = new ColoredGlyph(Color.White, Color.Black, '?');



        /// <summary>
        /// a emun for what tile type it is
        /// </summary>
        public enum TileType
        {
            Floor,
            Wall
        }


        public class Tile {
            public TileType TileType;

            public bool impassable;

            public bool blocksview;

            public bool explored;

            public bool visible;
            

            public Tile(TileType type)
            {
                TileType = type;

                visible = false;
                
                explored = false;

                if (type == TileType.Floor)
                {
                    impassable = false;
                    blocksview = false;
                }
                else if (type == TileType.Wall)
                {
                    impassable = true;
                    blocksview = true;
                }
            }
        }

        /// <summary>
        ///  a 2d array of TileTypes for maps
        /// <param name="Width">the width of the map</param>
        /// <param name="Height">the height of the map</param>
        /// </summary>
        public Map(int width, int height)
        {
            MapArray = new ArrayMap2D<Tile>(width, height);

            
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
            for (int x = 0; x < MapArray.Width; x++)
            {
                for (int y = 0; y < MapArray.Height; y++)
                {
                    // Generate a random tile
                    Tile tile = new Tile(TileType.Floor);

                    // 20% chance of a wall
                    if (random.Next(100) < 20)
                    {
                        tile = new Tile(TileType.Wall);
                    }

                    // If it's a border tile, set it as a wall
                    if (x == 0 || x == MapArray.Width - 1 || y == 0 || y == MapArray.Height - 1)
                    {
                        tile = new Tile(TileType.Wall);
                    }

                    // Set the tile in the map
                    map.SetTile(x,y, tile);
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
        private void SetTile(int x, int y, Tile tile)
        {
            // Set the tile in the map
            MapArray[x, y] = tile;
        }

        /// <summary>
        /// draws the map
        /// </summary>
        /// <param name="surface"> what surface to draw to</param>
        public void DrawArray( CellSurface surface)
        {
            for (int x = 0; x < this.MapArray.Width; x++)
            {
                for (int y = 0; y < this.MapArray.Height; y++)
                {

                    // not explored, don't draw
                    if (MapArray[x, y].explored == false)
                    {
                        continue;
                    }

                    if (TileType.Floor == MapArray[x, y].TileType)
                    {

                        // if lit
                        if (MapArray[x, y].visible)
                        {
                            surface.SetCellAppearance(x, y, FLOOR_LIT);
                        }
                        else
                        {
                            surface.SetCellAppearance(x, y, FLOOR);
                        }

                    }
                    else if (TileType.Wall == MapArray[x, y].TileType)
                    {
                        // if lit
                        if (MapArray[x, y].visible){
                            surface.SetCellAppearance(x, y, WALL_LIT);
                        } else
                        {
                            surface.SetCellAppearance(x, y, WALL);
                        }
                    }
                    else
                    {
                        surface.SetCellAppearance(x, y, UNDEFINED);
                    }
                    
                }
            
            }
        }

        /// <summary>
        /// updates the FOV
        /// </summary>
        /// <param name="x">the x coordinate to were the source is</param>
        /// <param name="y">the y coordinate to were the source is</param>
        public void UpdateFOV(int x, int y)
        {
        
        }


    }

}