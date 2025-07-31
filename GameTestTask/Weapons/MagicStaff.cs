using GameTestTask.Characters;

namespace GameTestTask.Weapons
{
    public class MagicStaff : Weapon
    {
        public MagicStaff()
        {
            Name = "Посох";
        }

        public override int CalculateDamage(Character attacker)
        {
            return attacker.Magic * 2 + 2;
        }
    }
}
