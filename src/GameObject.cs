namespace MIST;

using System;
using MIST.items.Mellees;
using SadConsole;
using SadRogue.Primitives;


public class GameObject
{
    public Point Position { get; private set; }

    public ColoredGlyph Appearance { get; set; }

    public IScreenSurface hostingSurface { get; set; }

    public Fighter? Fighter { get; set; }

    public Info Info { get; set; }

    public AI.BasicMonsterAI? AI { get; set; }
    private readonly ColoredGlyph DEAD = new ColoredGlyph(Color.Crimson, Color.Black, '%');

    private UI UI;
    public GameObject(ColoredGlyph appearance, Point position, IScreenSurface surface, Fighter? fighter, Info info,  AI.BasicMonsterAI? monsterAI, UI ui)
    {
        Appearance = appearance;
        Position = position;
        hostingSurface = surface;
        Fighter = fighter;
        Info = info;
        AI = monsterAI;
        UI = ui;
        // attach the OnDeath event of the fighter
        if (Fighter != null)
            Fighter.OnDeath += Death;
    }


    private void Death()
    {
        // check if its was dead before this call

        UI.SendMessage(Info.name + " died!");
        Appearance = DEAD;
        Info.description = "the gross guts of " + Info.name + ", R.I.P.";
        Info.name = Info.name + " corpse";
         
    }

    public void Draw()
    {
        if(Position == new Point(-1,-1)) return;
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
        foreach (var obj in objects)
        {
            if (obj.Position == new Point(Position.X + dx, Position.Y + dy))
            {
                // check if we have the fighter, and the traggered object has a fighter
                if (Fighter == null || obj.Fighter == null)
                {
                    continue;
                }

                // if the traget is a corpse, move it
                if (obj.Fighter.IsDead == true)
                {
                    continue;
                }

                // if the monster is not of same type, attack
                if (obj.Fighter.type != Fighter.type)
                {
                    if (IsVisible(map)){
                    UI.SendMessage(Info.name + " attacks!");
                    }
                    // see if its the player and what item he is holding
                    if (obj.Fighter.type !=  monsterType.player && Fighter.type == monsterType.player)
                    {
                        var item = UI.handitem;
                        var dmg = 2;
                        if (item != null && item is IMelleeItem)
                        {
                            dmg = (int)item.GetType().GetMethod("WeaponAttack").Invoke(item, new object[] { obj });
                        }

                        obj.Fighter?.takeDamage(dmg, Fighter.power, obj, map);
                    }
                    else{
                        obj.Fighter?.takeDamage(2, Fighter.power, obj, map);
                    }
                    
                }
                
                blockedbyobject = true;
                break;
            }
        }

        // move if are not blocked
        if (!blockedbyobject){
            TryToMove(dx, dy, map, objects);
        }


    }

    public bool IsVisible(Map map)
    {
        return map[Position].Visible;
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
            if (obj.Position == new Point(x, y) && obj.Fighter?.type != monsterType.UNDEFINED)
            {
                // see if the object has a fighter, if yes, see if the fighter is dead
                if (obj.Fighter != null)
                {
                    return !obj.Fighter.IsDead;
                }
                else
                {
                    return true;
                }
            }
        }

        // check for chests
        foreach (var chest in Map.chests)
        {
            if (chest.Position == new Point(x, y))
            {
                return true;
            }
        }

        return false;
    }

}