using GameTestTask.Weapons;

namespace GameTestTask.Characters
{
    public class Mage : Character
    {
        public Mage()
        {
            Name = "Волшебник";
            Strength = 1;
            Magic = 2;
            Health = 50;
        }

        public override void ApplyWeaponEffect(Weapon weapon)
        {
            if (weapon is FoolSting)
            {
                if (new Random().Next(0, 2) == 1)
                {
                    (Strength, Magic) = (Magic, Strength);
                }
            }
        }
    }
}
