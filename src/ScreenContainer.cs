﻿using SadConsole;
using SadConsole.Input;
using SadRogue.Primitives;

namespace MIST
{
    internal class ScreenContainer : ScreenObject
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public static ScreenContainer Instance { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public readonly GameObject Player;
        public readonly Map Map;
        public readonly int Width, Height;

        public ScreenContainer()
        {
            Instance = this;
            Width = Constants.ScreenWidth;
            Height = Constants.ScreenHeight;

            // generate the map
            Map = Map.GenerateMap(Width, Height);
            Map.Draw();
            Map.IsFocused = true;

            // add the player
            Player = new GameObject(new ColoredGlyph(Color.White, Color.Black, '@'), Map.start, Map);
            Player.Draw();


            Children.Add(Map);
        }
    }
}