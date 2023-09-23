namespace MIST
{
    internal class Tile
    {
        public TileType TileType;
        public bool Impassable;
        public bool BlocksView;
        public bool Explored;
        public bool Visible;

        public Tile(TileType type)
        {
            TileType = type;
            Visible = false;
            Explored = false;

            if (type == TileType.Floor)
            {
                Impassable = false;
                BlocksView = false;
            }
            else if (type == TileType.Wall)
            {
                Impassable = true;
                BlocksView = true;
            }
        }
    }

    /// <summary>
    /// a enum for what tile type it is
    /// </summary>
    public enum TileType
    {
        Floor,
        Wall
    }
}
