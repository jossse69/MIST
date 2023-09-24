namespace MIST
{
    internal class Fighter
    {
        public int maxHP;
        public int HP;
        public int power;
        public int defense;

        public bool IsDead { get; set; }
        public event Action? OnDeath; // Define the OnDeath event

        public Fighter(int maxHP, int HP, int power, int defense)
        {
            this.maxHP = maxHP;
            this.HP = HP;
            this.power = power;
            this.defense = defense;
            this.IsDead = false;
        }

        public void takeDamage(int damage, int power)
        {
            var rng = new Random();
            var dmg = Math.Max(rng.Next(damage - defense, damage * power), 0);

            System.Console.WriteLine("did " + dmg + " physical damage.");

            if (dmg <= 0)
            {
                System.Console.WriteLine("It's very uneffective...");
            }
            else
            {
                // 25% chance for a critical hit
                if (rng.Next(100) < 25)
                {
                    dmg *= 2;
                    System.Console.WriteLine("Critical Hit!");
                    HP -= dmg;
                }
                else
                {
                    HP -= dmg;
                }
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