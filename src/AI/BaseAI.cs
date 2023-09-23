using SadRogue.Primitives;
using SadConsole;
using SadRogue.Primitives.GridViews;
using GoRogue.FOV;
using SadConsole.Input;
using GoRogue.Pathing;

namespace MIST.AI
{
    internal class monsterAI
    {

        public AStar pathfinder;

        public GoRogue.Pathing.Path? currentpath;
        public Action? OnAITurn;

        public Point? target = new Point(-1, -1);

        private int i = 0;

        public monsterAI(Map map) {
            pathfinder = new AStar(map.GetFOV().TransparencyView, Distance.Chebyshev);
        }

        public void AITurn(GameObject @object, Map map, List<GameObject> objects) {
            // if tis dead, do nothing
            if (@object.fighter.IsDead)
            {
                return;
            }

            
            var rng = new Random();
            // if the target is (-1, -1), find a new one
            if (target.Value.X == -1 && target.Value.Y == -1)
            {
                var X = Math.Clamp(target.Value.X + rng.Next(-7, 7), 0, map.Width - 1);
                var Y = Math.Clamp(target.Value.Y + rng.Next(-7, 7), 0, map.Height - 1);
                target = new Point(X, Y);
                while (@object.IsBlocked(target.Value.X, target.Value.Y, map, objects))
                {
                    X = Math.Clamp(target.Value.X + rng.Next(-7, 7), 0, map.Width - 1);
                    Y = Math.Clamp(target.Value.Y + rng.Next(-7, 7), 0, map.Height - 1);
                    target = new Point(X, Y);
                    currentpath = FindPath(@object.Position, target.Value);

                    // if path is not null, stop
                    if (currentpath != null)
                    {
                        continue;
                    }
                }
                
            }
            
            
            // step the path
            var step = StepPath(currentpath, i);
            @object.MoveAndAttack(step.X, step.Y, map, objects);

            i++;

            System.Console.WriteLine("successful AI turn");

        }

        public GoRogue.Pathing.Path? FindPath(Point start, Point end) {
            var path = pathfinder.ShortestPath(start, end);
            return path;

        }

        public Point StepPath(GoRogue.Pathing.Path path, int idx) {

            // if its the last step, return the target
            if (idx == path.Steps.Count() - 1)
            {
                return target.Value;
            }
            
            var movement = path.Steps.ElementAt(idx);

            return new Point(movement.Y, movement.Y);
        }
    }
}