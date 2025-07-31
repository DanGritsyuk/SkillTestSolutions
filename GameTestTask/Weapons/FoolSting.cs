using GameTestTask.Characters;

namespace GameTestTask.Weapons
{
    public class FoolSting : Weapon
    {
        public FoolSting()
        {
            Name = "Жало дурака";
        }

        public override int CalculateDamage(Character attacker)
        {
            attacker.ApplyWeaponEffect(this);
            return new Random().Next(0, 11);
        }
    }
}