﻿using PacManBot.Extensions;
using PacManBot.Games.Concrete.Rpg.Buffs;

namespace PacManBot.Games.Concrete.Rpg.Enemies
{
    public class Slime : Enemy
    {
        public override string Name => "Green Slime";
        public override string Description => "So common and weak it's boring.";
        public override int Level => 1;
        public override int ExpYield => 1;
        public override int BaseDamage => 1;
        public override int BaseDefense => 0;
        public override double BaseCritChance => 0;

        public override void SetStats()
        {
            MaxLife = 10;
            DamageType = DamageType.Blunt;
        }
    }


    public class BlueSlime : Enemy
    {
        public override string Name => "Blue Slime";
        public override string Description => "Almost as weak as the green variety.";
        public override int Level => 1;
        public override int ExpYield => 2;
        public override int BaseDamage => 2;
        public override int BaseDefense => 0;
        public override double BaseCritChance => 0;

        public override void SetStats()
        {
            MaxLife = 12;
            DamageType = DamageType.Blunt;
        }
    }


    public class Flop : Enemy
    {
        public override string Name => "Flopper";
        public override string Description => "It flops around messily. Weird one.";
        public override int Level => 2;
        public override int ExpYield => 2;
        public override int BaseDamage => 2;
        public override int BaseDefense => 0;
        public override double BaseCritChance => 0.5;

        public override void SetStats()
        {
            MaxLife = 18;
            CritChance = 0.5;
            DamageType = DamageType.Blunt;
            DamageResistance[DamageType.Cutting] = -0.2;
        }
    }


    public class Rat : Enemy
    {
        public override string Name => "Feral Rat";
        public override string Description => "Just your basic rat enemy.";
        public override int Level => 3;
        public override int ExpYield => 3;
        public override int BaseDamage => 5;
        public override int BaseDefense => 0;
        public override double BaseCritChance => 0.05;

        public override void SetStats()
        {
            MaxLife = 30;
            DamageType = DamageType.Cutting;
            DamageResistance[DamageType.Magic] = -0.3;
        }
    }


    public class Oinx : Enemy
    {
        public override string Name => "Oinx";
        public override string Description => "A mix between pig and lynx.";
        public override int Level => 4;
        public override int ExpYield => 3;
        public override int BaseDamage => 7;
        public override int BaseDefense => 0;
        public override double BaseCritChance => 0.02;

        public override void SetStats()
        {
            MaxLife = 21;
            DamageType = DamageType.Magic;
            DamageResistance[DamageType.Pierce] = -0.2;
        }
    }


    public class Bear : Enemy
    {
        public override string Name => "Bear";
        public override string Description => "Grrrr.";
        public override int Level => 9;
        public override int ExpYield => 7;
        public override int BaseDamage => 10;
        public override int BaseDefense => 4;
        public override double BaseCritChance => 0.01;

        public override void SetStats()
        {
            MaxLife = 70;
            DamageType = DamageType.Cutting;
            DamageResistance[DamageType.Blunt] = 0.2;
        }
    }


    public class Goblin : Enemy
    {
        public override string Name => "Goblin";
        public override string Description => "Its spear can expose your weak points.";
        public override int Level => 12;
        public override int ExpYield => 8;
        public override int BaseDamage => 10;
        public override int BaseDefense => 3;
        public override double BaseCritChance => 0.05;

        public override void SetStats()
        {
            MaxLife = 60;
            DamageType = DamageType.Pierce;
        }

        public override string Attack(Entity target)
        {
            string msg = "";
            if (!target.HasBuff<Vulnerable>() && Program.Random.OneIn(3))
            {
                msg = $"{target} is now vulnerable!";
                target.AddBuff<Vulnerable>(3);
            }
            return base.Attack(target) + msg;
        }
    }


    public class Slime3 : Enemy
    {
        public override string Name => "Dark Slime";
        public override string Description => "It's so mushy you can barely damage it.";
        public override int Level => 14;
        public override int ExpYield => 8;
        public override int BaseDamage => 12;
        public override int BaseDefense => 5;
        public override double BaseCritChance => 0.05;

        public override void SetStats()
        {
            MaxLife = 60;
            DamageType = DamageType.Blunt;
            DamageResistance[DamageType.Blunt] = 0.5;
            DamageResistance[DamageType.Cutting] = 0.2;
            DamageResistance[DamageType.Pierce] = 0.5;
            DamageResistance[DamageType.Magic] = -0.25;
        }

        public override string Attack(Entity target)
        {
            string msg = "";
            if (!target.HasBuff<Blinded>() && Program.Random.OneIn(3))
            {
                msg = $"{target} got slime in their eyes!";
                target.AddBuff<Blinded>(3);
            }
            return base.Attack(target) + msg;
        }
    }


    public class MamaOinx : Enemy
    {
        public override string Name => "Mama Oinx";
        public override string Description => "Cute and terrifying.";
        public override int Level => 17;
        public override int ExpYield => 9;
        public override int BaseDamage => 8;
        public override int BaseDefense => 5;
        public override double BaseCritChance => 0.02;

        public override void SetStats()
        {
            MaxLife = 69;
            DamageType = DamageType.Blunt;
            DamageResistance[DamageType.Pierce] = -0.2;
        }

        public override string Attack(Entity target)
        {
            var attacks = new int[5];
            if (!target.HasBuff<Immune>())
            {
                for (int i = 0; i < attacks.Length; i++)
                {
                    int dmg = Program.Random.Next(1, 3);
                    target.Life -= dmg;
                    attacks[i] = dmg;
                }
            }
            return base.Attack(target) + $"\n{this}'s babies attack! {attacks.JoinString(", ")}.";
        }
    }


    public class MechaBear : Enemy
    {
        public override string Name => "Mecha Bear";
        public override string Description => "Grrrrr BEEP grrr BOOP";
        public override int Level => 26;
        public override int ExpYield => 15;
        public override int BaseDamage => 26;
        public override int BaseDefense => 8;
        public override double BaseCritChance => 0.02;

        public override void SetStats()
        {
            MaxLife = 110;
            DamageType = DamageType.Cutting;
            DamageResistance[DamageType.Cutting] = 0.5;
        }
    }


    public class MechaOinx : Enemy
    {
        public override string Name => "Mecha Oinx";
        public override string Description => "Where are these mechas coming from?!";
        public override int Level => 35;
        public override int ExpYield => 20;
        public override int BaseDamage => 35;
        public override int BaseDefense => 7;
        public override double BaseCritChance => 0.2;

        public override void SetStats()
        {
            MaxLife = 80;
            DamageType = DamageType.Pierce;
            DamageResistance[DamageType.Pierce] = -0.2;
        }
    }


    public class MechaFlopper : Enemy
    {
        public override string Name => "Mecha Flopper";
        public override string Description => "EXTERMINATE.";
        public override int Level => 42;
        public override int ExpYield => 25;
        public override int BaseDamage => 32;
        public override int BaseDefense => 10;
        public override double BaseCritChance => 1.0;

        public override void SetStats()
        {
            MaxLife = 90;
            DamageType = DamageType.Blunt;
            DamageResistance[DamageType.Cutting] = -0.2;
        }
    }
}
