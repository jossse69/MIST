
using System;
using GoRogue.GameFramework;
using SadConsole;
using SadConsole.Input;
using SadRogue.Primitives;
using Console = SadConsole.Console;

namespace MIST
{
    class Program
    {

        public const int Width = 80;
        public const int Height = 25;

        public static Console startingConsole = (Console)GameHost.Instance.Screen;

        public static GameObject player = new GameObject(new ColoredGlyph(Color.White, Color.Black, '@'), new Point(0, 0), startingConsole);

        public static Map map = new Map(Width, Height);

        public static int movetimer = 0;


        static void Main(string[] args)
        {
            SadConsole.Settings.WindowTitle = "MIST";

            // Setup the engine and create the main window.
            SadConsole.Game.Create(Width, Height);

            // Hook the start event so we can add consoles to the system.
            SadConsole.Game.Instance.OnStart = Init;

            // Start the game.
            SadConsole.Game.Instance.Run();
            SadConsole.Game.Instance.Dispose();
        }

        private static void Init()
        {
            var mapSurface = new CellSurface(Width, Height);
            map = map.GenerateMap(Width, Height);
            var keyboard = new Keyboard();

            SadConsole.Game.Instance.FrameUpdate += (sender, args) =>
            {
                startingConsole.Clear();
                map.DrawArray(mapSurface);
                mapSurface.Copy(startingConsole.Surface);
                player.Draw();
                
                var inputprocessed = processInput(keyboard);

                // see if we did an input
                if (inputprocessed)
                {
                    System.Console.WriteLine("input processed");
                }

            };
        }

        private static bool processInput(Keyboard keyboard)
        {
            var processed = false;
        
            // update keyboard input
            keyboard.Update(TimeSpan.Zero);
        
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
                    if (movetimer > 0)
                    {
                        movetimer--;
                    }
                    else
                    {
                        player.TryToMove(dx, dy, map);
                        movetimer = 5;
                    }
                    
                }
        
            return processed;
        }

    }
}