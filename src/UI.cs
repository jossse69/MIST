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

            display.Map.Print(0,0, "HP: " + player.Fighter.HP + " / " + player.Fighter.maxHP);

        }
    }
}