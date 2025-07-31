using GameTestTask.Characters;
using GameTestTask.Weapons;

namespace GameTestTask;

public class GameEngine
{
    private bool _isDuelEnded = false;

    public void InflictDamage(Character attacker, Weapon weapon, Character target)
    {
        if (_isDuelEnded) { throw new Exception("GameOver"); }

        if (!attacker.IsAlive || !target.IsAlive) return;

        int damage = weapon.CalculateDamage(attacker);
        target.TakeDamage(damage);

        Console.WriteLine($"{attacker.Name} атаковал {target.Name} использовав {weapon.Name}. Итог урон  равен: {damage}!");
        Console.WriteLine($"{target.Name} здоровье: {target.Health}");

        if (!target.IsAlive)
        {
            _isDuelEnded = true;
            Console.WriteLine($"{target.Name} убит!");
        }
    }
}
