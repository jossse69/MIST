using GoRogue.Pathing;
using SadRogue.Primitives;

namespace MIST.AI
{
    internal class MonsterAI
    {
        public AStar Pathfinder { get; private set; }
        public Action? OnAITurn { get; set; }
        public GoRogue.Pathing.Path? CurrentPath { get; private set; }
        public Point Target { get; private set; } = new Point(-1, -1);

        private int _currentPathIndex = 0;

        public MonsterAI(Map map) 
        {
            Pathfinder = new AStar(map.GetFOV().TransparencyView, Distance.Chebyshev);
        }

        public void AITurn(GameObject go, Map map, List<GameObject> objects) 
        {
            // if tis dead, do nothing
            if (go == null || go.Fighter == null || go.Fighter.IsDead)
            {
                return;
            }

            if (CurrentPath == null)
            {
                // if the target is (-1, -1), find a new one
                if (Target.X == -1 && Target.Y == -1)
                {
                    int maxAttempts = 100; // You can adjust the number of attempts as needed
                    Point newTarget;

                    var rng = ScreenContainer.Instance.Random;
                    do
                    {
                        int xOffset = rng.Next(-7, 7);
                        int yOffset = rng.Next(-7, 7);

                        int newX = Math.Clamp(go.Position.X + xOffset, 0, map.Width - 1);
                        int newY = Math.Clamp(go.Position.Y + yOffset, 0, map.Height - 1);

                        newTarget = new Point(newX, newY);
                        maxAttempts--;

                    } while (maxAttempts > 0 && go.IsBlocked(newTarget.X, newTarget.Y, map, objects));

                    if (maxAttempts <= 0)
                    {
                        // Handle the case where a valid target couldn't be found after several attempts
                        Console.WriteLine("Could not find a valid path.");
                        return;
                    }
                    else
                    {
                        Target = newTarget;
                    }
                }
                CurrentPath = FindPath(go.Position, Target);
            }
            
            // step the path
            if (CurrentPath == null)
            {
                Console.WriteLine("Could not find a valid path for target " + Target + " | Resetting path!");
                Target = (-1, -1);
                return;
            }

            var step = StepPath(CurrentPath, _currentPathIndex, out bool finished);

            // TODO: Verify if we can actually walk to this step, if not we should finish this path

            go.MoveAndAttack(step.X, step.Y, map, objects);

            if (finished)
            {
                CurrentPath = null;
                Target = (-1, -1);
                _currentPathIndex = 0;
                Console.WriteLine("Finished ai turn!");
            }
            else
            {
                _currentPathIndex++;
                Console.WriteLine("Increased path index, Target: " + Target);
            }
        }

        public GoRogue.Pathing.Path? FindPath(Point start, Point end) 
        {
            var path = Pathfinder.ShortestPath(start, end);
            return path;
        }

        public Point StepPath(GoRogue.Pathing.Path path, int idx, out bool finished) {

            // if its the last step, return the target
            finished = false;
            if (idx == path.Steps.Count() - 1)
            {
                finished = true;
                return Target;
            }

            var movement = path.Steps.ElementAt(idx);
            return new Point(movement.Y, movement.Y);
        }
    }
}