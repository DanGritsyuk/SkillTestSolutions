using GameTestTask.Characters;

namespace GameTestTask.Weapons
{
    public class Sword : Weapon
    {
        public Sword()
        {
            Name = "Меч";
        }

        public override int CalculateDamage(Character attacker)
        {
            return attacker.Strength * 3;
        }
    }
}
