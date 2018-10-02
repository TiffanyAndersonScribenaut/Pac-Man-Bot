﻿using PacManBot.Extensions;

namespace PacManBot.Games.Concrete.Rpg.Weapons
{
    public class TitanHammer : Weapon
    {
        public override string Name => "Titan Hammer";
        public override string Description => "Ignores half of enemy defense.";
        public override int Damage => 14;
        public override double CritChance => 0.05f;
        public override DamageType Type => DamageType.Blunt;
        public override MagicType Magic => MagicType.Water;

        public override int LevelGet => 20;

        public override string AttackEffects(Player wielder, Entity target)
        {
            target.Defense /= 2;
            return "";
        }
    }


    public class Longbow : Weapon
    {
        public override string Name => "Enchanted Longbow";
        public override string Description => "High chance of oof.";
        public override int Damage => 16;
        public override double CritChance => 0.2f;
        public override DamageType Type => DamageType.Pierce;
        public override MagicType Magic => MagicType.Air;

        public override int LevelGet => 24;
    }


    public class EarthSpell : Weapon
    {
        public override string Name => "Forest Trance";
        public override string Description => "Heals the caster.";
        public override int Damage => 18;
        public override double CritChance => 0.05f;
        public override DamageType Type => DamageType.Magic;
        public override MagicType Magic => MagicType.Earth;

        public override int LevelGet => 29;

        public override string AttackEffects(Player wielder, Entity target)
        {
            int heal = Bot.Random.Next(3, 7);
            wielder.Life += heal;
            return $"{wielder} restores {heal} HP!";
        }
    }


    public class ImportantSword : Weapon
    {
        public override string Name => "Sword McGuffin";
        public override string Description => "They say it's legendary,\nbut you don't buy that.";
        public override int Damage => 25;
        public override double CritChance => 0.05f;
        public override DamageType Type => DamageType.Cutting;
        public override MagicType Magic => MagicType.Fire;

        public override int LevelGet => 35;

        public override string AttackEffects(Player wielder, Entity target)
        {
            if (Bot.Random.OneIn(3) && !target.Buffs.ContainsKey(nameof(Buffs.Blinded)))
            {
                target.AddBuff(nameof(Buffs.Blinded), 5);
                return $"{target} is blinded by awesomeness!";
            }
            return "";
        }
    }
}