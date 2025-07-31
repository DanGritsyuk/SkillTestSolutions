using GameTestTask.Characters;

namespace GameTestTask.Weapons
{
    public abstract class Weapon
    {
        public string Name { get; protected set; }

        public abstract int CalculateDamage(Character attacker);
    }
}
