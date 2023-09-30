namespace MIST
{
    public class Fighter
    {
        public int maxHP;
        public int HP;
        public int power;
        public int defense;

        public bool IsDead { get; set; }
        public event Action? OnDeath; // Define the OnDeath event

        public monsterType type;
        public UI ui;

        public Fighter(int maxHP, int HP, int power, int defense, UI UI, monsterType type)
        {
            this.maxHP = maxHP;
            this.HP = HP;
            this.power = power;
            this.defense = defense;
            this.IsDead = false;
            this.ui = UI;
            this.type = type;
        }

        public void takeDamage(int damage, int power)
        {
            var rng = new Random();
            var dmg = Math.Max(rng.Next(damage - defense, damage * power), 0);

            ui.SendMessage("did " + dmg + " physical damage.");

            if (dmg <= 0)
            {
                ui.SendMessage("It's very uneffective...");
            }
            else if (dmg > damage * 1.5)
            {
                ui.SendMessage("Critical Hit!");
                HP -= dmg;
            }  
            else
            {
            HP -= dmg;
            }
                    
                
        

            // check if the fighter is dead
            if (HP <= 0)
            {
                OnDeath?.Invoke(); // Invoke the OnDeath event when HP reaches zero or below
                IsDead = true;
            }
        }
    }
}