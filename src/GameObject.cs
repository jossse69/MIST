namespace MIST;
using SadConsole;
using SadRogue.Primitives;


internal class GameObject
{
    public Point Position { get; private set; }

    public ColoredGlyph Appearance { get; set; }

    public IScreenSurface hostingSurface { get; set; }

    public Fighter? fighter { get; set; }

    private readonly ColoredGlyph DEAD = new ColoredGlyph(Color.Crimson, Color.Black, '%');

    public GameObject(ColoredGlyph appearance, Point position, IScreenSurface surface, Fighter? Fighter)
    {
        Appearance = appearance;
        Position = position;
        hostingSurface = surface;
        fighter = null;
        fighter = Fighter;
        // attach the OnDeath event of the fighter
        fighter.OnDeath += Death;

    }

    private void Death()
    {
        System.Console.WriteLine("this thing has died!");
        Appearance = DEAD;
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

    public void TryToMove(int dx, int dy, Map map, List<GameObject> objects)
    {
        // check if the position is within the map
        if (Position.X + dx >= 0 && Position.X + dx < map.Width && Position.Y + dy >= 0 && Position.Y + dy < map.Height)
        {
            // if it goes into a wall, don't move, else move
            if (!IsBlocked(Position.X + dx, Position.Y + dy, map, objects))
            {
                MoveTo(new Point(Position.X + dx, Position.Y + dy ));
            }
        }
    }

    public void MoveAndAttack(int dx, int dy, Map map, List<GameObject> objects)
    {
        var blockedbyobject = false;

        // see if we will run into an object
        foreach (var obj in objects){
            if (obj.Position == new Point(Position.X + dx, Position.Y + dy)){

                // check if we have the fighter, and the traggered object has a fighter
                if (fighter == null || obj.fighter == null){
                    continue;
                }

                obj.fighter?.takeDamage(2, fighter.power);
                blockedbyobject = true;
                break;
            }
        }

        // move if are not blocked
        if (!blockedbyobject){
            TryToMove(dx, dy, map, objects);
        }


    } 

    public bool IsBlocked(int x, int y, Map map, List<GameObject> objects)
    {
        // if ti meets a wall or runs into a object, return true
        if (map[x, y].Impassable == true)
        {
            return true;
        }

         // check for objects
        foreach (var obj in objects)
        {
            if (obj.Position == new Point(x, y))
            {
                return true;
            }
        }
        return false;
    }

}