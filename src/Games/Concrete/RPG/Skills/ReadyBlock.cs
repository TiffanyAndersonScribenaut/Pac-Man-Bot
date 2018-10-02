﻿
namespace PacManBot.Games.Concrete.Rpg.Skills
{
    public class ReadyBlock : Skill
    {
        public override string Name => "Ready Block";
        public override string Description => "Increases defense by 50% for three turns.";
        public override string Shortcut => "block";
        public override int ManaCost => 1;
        public override SkillType Type => SkillType.Def;
        public override int SkillGet => 5;

        public override string Effect(RpgGame game)
        {
            game.player.AddBuff(nameof(Buffs.Blocking), 3);

            return $"{game.player} is blocking!";
        }
    }
}