using SadRogue.Primitives;
using SadConsole;
using SadRogue.Primitives.GridViews;
using SadConsole.Input;
using GoRogue.Messaging;
namespace MIST
{
    internal class UI
    {
        public ScreenContainer display { get; set; }

        public MessageBus Messages { get; set; }


        public UI(ScreenContainer Display)
        {
            display = Display;
            Messages = new MessageBus();
        }

        public void Draw(GameObject player)
        {
            var Height = 0;
            var surface = display.Map;

            surface.DrawLine(new Point(0,Height ), new Point(display.Width, Height), ' ', Color.White, Color.Black);
            surface.Print(0,Height , "HP: " + player.Fighter.HP + " / " + player.Fighter.maxHP);


        }
    }
}