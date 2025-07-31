using GameTestTask;
using GameTestTask.Characters;
using GameTestTask.Weapons;

var engine = new GameEngine();
var warrior = new Warrior();
var mage = new Mage();

Weapon[] warriorWeapons = { new Sword(), new FoolSting() };

Weapon[] mageWeapons = { new MagicStaff(), new FoolSting() };

var random = new Random();
int round = 1;

while (warrior.IsAlive && mage.IsAlive)
{
    Console.WriteLine($"\n=== РАУНД {round} ===");

    if (warrior.IsAlive)
    {
        var weapon = warriorWeapons[random.Next(warriorWeapons.Length)];
        engine.InflictDamage(warrior, weapon, mage);
    }

    if (mage.IsAlive)
    {
        var weapon = mageWeapons[random.Next(mageWeapons.Length)];
        engine.InflictDamage(mage, weapon, warrior);
    }

    round++;
}

Console.WriteLine("\n=== Дуэль окончена ===");
if (!warrior.IsAlive && !mage.IsAlive)
{
    Console.WriteLine("Оба пали! Ничья!");
}
else if (!warrior.IsAlive)
{
    Console.WriteLine($"{mage.Name} выиграл!");
}
else
{
    Console.WriteLine($"{warrior.Name} выиграл!");
}