using GameTestTask.Weapons;

namespace GameTestTask.Characters
{
    public abstract class Character
    {
        public string Name { get; protected set; }
        public int Strength { get; protected set; }
        public int Magic { get; protected set; }
        public int Health { get; protected set; }

        public bool IsAlive => Health > 0;

        public void TakeDamage(int damage)
        {
            Health = Math.Max(0, Health - damage);
        }

        public virtual void ApplyWeaponEffect(Weapon weapon) { }
    }
}
