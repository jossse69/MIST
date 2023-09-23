namespace MIST;
using SadConsole;
using SadRogue.Primitives;


internal class GameObject
{
    public Point Position { get; private set; }

    public ColoredGlyph Appearance { get; set; }

    public IScreenSurface hostingSurface { get; set; }

    public GameObject(ColoredGlyph appearance, Point position, IScreenSurface surface)
    {
        Appearance = appearance;
        Position = position;
        hostingSurface = surface;
    }

    public void Draw()
    {
        Appearance.CopyAppearanceTo( hostingSurface.Surface[Position]);
        hostingSurface.IsDirty = true;
    }

    public void MoveTo(Point newPosition)
    {
        Position = newPosition;
    }

    public void TryToMove(int dx, int dy, Map map)
    {
        // check if the position is within the map
        if (Position.X + dx >= 0 && Position.X + dx < map.Width && Position.Y + dy >= 0 && Position.Y + dy < map.Height)
        {
            // if it goes into a wall, don't move, else move
            if (map[Position.X + dx, Position.Y + dy].TileType != TileType.Wall)
            {
                MoveTo(new Point(Position.X + dx, Position.Y + dy));
            }
        }
    }
}