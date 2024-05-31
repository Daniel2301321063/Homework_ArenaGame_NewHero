using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework_ArenaGame
{
    public class GameEngine
    {
        public class NotificationArgs
        {
            public Hero Attacker { get; set; }
            public Hero Defender { get; set; }
            public double Attack { get; set; }
            public double Damage { get; set; }
        }
        public delegate void GameNotifications(NotificationArgs args);

        private Random random = new Random();
        public Hero HeroA { get; set; }
        public Hero HeroB { get; set; }
        public Hero Winner { get; set; }
        public GameNotifications NotificationsCallBack { get; set; }
        public void Fight()
        {
            Hero attacker;
            Hero defender;

            double probability = random.NextDouble();
            if (probability < 0.5)
            {
                attacker = HeroA;
                defender = HeroB;
            }
            else
            {
                attacker = HeroB;
                defender = HeroA;
            }
            while (attacker.IsAlive && defender.IsAlive)
            {
                double attack = attacker.Attack();
                double actualDamage = defender.Defend(attack);

                if (NotificationsCallBack != null)
                {
                    NotificationsCallBack(new NotificationArgs()
                    {
                        Attacker = attacker,
                        Defender = defender,
                        Attack = attack,
                        Damage = actualDamage,
                    }); 
                }
                Hero tempHero = attacker;
                attacker = defender;
                defender = tempHero;
            }

            Winner = defender;
        }
    }
    public abstract class Hero : IHero
    {
        protected Random random = new Random();
        public string Name { get; private set; }
        public double Health { get; private set; }
        public double Armor { get; private set; }
        public double Strenght { get; private set; }
        public IWeapon Weapon { get; private set; }
        public bool IsAlive
        {
            get
            {
                return Health > 0;
            }
        }
        public Hero(string name, double armor, double strenght, IWeapon weapon)
        {
            Health = 100;
            Name = name;
            Armor = armor;
            Strenght = strenght;
            Weapon = weapon;
        }
        public virtual double Attack()
        {
            double totalDamage = Strenght + Weapon.AttackDamage;
            double coef = random.Next(80, 120 + 1);
            double realDamage = totalDamage * (coef / 100);
            return realDamage;
        }
        public virtual double Defend(double damage)
        {
            double coef = random.Next(80, 120 + 1);
            double defendPower = (Armor + Weapon.BlockingPower) * (coef / 100);
            double realDamage = damage - defendPower;
            if (realDamage < 0)
                realDamage = 0;
            Health -= realDamage;
            return realDamage;
        }
        public void Heal(double amount)
        {
            Health += amount;
            if (Health > 100)
            {
                Health = 100;
            }
        }
        public override string ToString()
        {
            return $"{Name} with health {Math.Round(Health, 2)}";
        }
    }
    public class Assassin : Hero
    {
        public Assassin(string name, double armor, double strenght, IWeapon weapon) : base(name, armor, strenght, weapon)
        {
        }
        public override double Attack()
        {
            double damage = base.Attack();

            double probability = random.NextDouble();
            if (probability < 0.10)
            {
                damage *= 3;
            }
            return damage;
        }
    }
    public class Knight : Hero
    {
        private double hitCount;
        private double damageCoef;
        public Knight(string name, double armor, double strenght, IWeapon weapon) : base(name, armor, strenght, weapon)
        {
            hitCount = 0;
            damageCoef = 0.6;
        }
        public override double Attack()
        {
            double damage = base.Attack();
            double realDamage = damage * damageCoef;
            if (damageCoef < 1) damageCoef += 0.1;
            return realDamage;
        }
        public override double Defend(double damage)
        {
            hitCount++;
            if (hitCount == 3)
            {
                hitCount = 0;
                return 0;
            }
            return base.Defend(damage);
        }
    }
    public class Archer : Hero
    {
        public Archer(string name, double armor, double strenght, IWeapon weapon) : base(name, armor, strenght, weapon)
        {
        }
        public override double Attack()
        {
            double damage = base.Attack();
            if (Weapon is Bow bow)
            {
                damage = bow.CriticalHit();
            }
            return damage;
        }
    }
    public class Mage : Hero
    {
        public Mage(string name, double armor, double strenght, IWeapon weapon) : base(name, armor, strenght, weapon)
        {
        }

        public override double Attack()
        {
            double damage = base.Attack();
            if (Weapon is MagicStaff staff)
            {
                Heal(staff.Heal());
            }
            return damage;
        }
    }
    public class Berserker : Hero
    {
        public Berserker(string name, double armor, double strenght, IWeapon weapon) : base(name, armor, strenght, weapon)
        {
        }
        public override double Attack()
        {
            double damage = base.Attack();
            if (Weapon is Axe axe)
            {
                damage += axe.BerserkBoost();
            }
            return damage;
        }
        public override double Defend(double damage)
        {
            return base.Defend(damage * 0.9);
        }
    }
    public interface IHero
    {
        double Attack();
        double Defend(double damage);
    }
    public interface IWeapon
    {
        string Name { get; set; }
        double AttackDamage { get; }
        double BlockingPower { get; }
    }
    public class Dagger : IWeapon
    {
        public string Name { get; set; }
        public double AttackDamage { get; private set; }
        public double BlockingPower { get; private set; }
        public Dagger(string name)
        {
            Name = name;
            AttackDamage = 30;
            BlockingPower = 1;
        }
    }
    public class Sword : IWeapon
    {
        public string Name { get; set; }
        public double AttackDamage { get; private set; }
        public double BlockingPower { get; private set; }
        public Sword(string name)
        {
            Name = name;
            AttackDamage = 20;
            BlockingPower = 10;
        }
    }
    public class Axe : IWeapon
    {
        public string Name { get; set; }
        public double AttackDamage { get; private set; }
        public double BlockingPower { get; private set; }
        public Axe(string name)
        {
            Name = name;
            AttackDamage = 20;
            BlockingPower = 5;
        }
        public double BerserkBoost()
        {
            return 10;
        }
    }
    public class Bow : IWeapon
    {
        public string Name { get; set; }
        public double AttackDamage { get; private set; }
        public double BlockingPower { get; private set; }
        public Bow(string name)
        {
            Name = name;
            AttackDamage = 15;
            BlockingPower = 2;
        }
        public double CriticalHit()
        {
            double probability = new Random().NextDouble();
            return probability < 1 ? AttackDamage * 2 : AttackDamage;
        }
    }
    public class MagicStaff : IWeapon
    {
        public string Name { get; set; }
        public double AttackDamage { get; private set; }
        public double BlockingPower { get; private set; }
        public MagicStaff(string name)
        {
            Name = name;
            AttackDamage = 20;
            BlockingPower = 5;
        }
        public double Heal()
        {
            return 5;
        }
    }
    class Program
    {
        static void ConsoleNotification(GameEngine.NotificationArgs args)
        {
            Console.WriteLine($"{args.Attacker.Name} attacked {args.Defender.Name} with {Math.Round(args.Attack, 2)} and caused {Math.Round(args.Damage, 2)} damage.");
            Console.WriteLine($"Attacker: {args.Attacker}");
            Console.WriteLine($"Defender: {args.Defender}");
        }
        static void Main(string[] args)
        {
            GameEngine gameEngine = new GameEngine()
            {
                HeroA = new Knight("Knight", 10, 15, new Sword("Sword")),
                HeroB = new Berserker("Berserker", 8, 10, new Axe("Axe")),
                NotificationsCallBack = ConsoleNotification
            };
            gameEngine.Fight();

            Console.WriteLine();
            Console.WriteLine($"And the winner is {gameEngine.Winner.Name}");
        }
    }
}
