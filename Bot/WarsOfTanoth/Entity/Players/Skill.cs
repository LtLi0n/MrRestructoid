using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MrRestructoid.Bot.WarsOfTanoth.Entity.Players
{
    public enum SkillType { Mining, WoodCutting, Fishing }

    public class Skills
    {
        public Skill Mining { get; set; }
        public Skill WoodCutting { get; set; }
        public Skill Fishing { get; set; }

        public Skills()
        {
            Mining = new Skill(0, SkillType.Mining);
            WoodCutting = new Skill(0, SkillType.WoodCutting);
            Fishing = new Skill(0, SkillType.Fishing);
        }
    }

    public class Skill
    {
        private enum XpTo { XpCap, Level, LevelXp }

        [JsonIgnore] public SkillType Type { get; private set; }

        public int TOTAL_XP { get; set; }

        [JsonIgnore] public int XP => Get_X_FromXp(XpTo.LevelXp);
        [JsonIgnore] public int XpCap => Get_X_FromXp(XpTo.XpCap);
        [JsonIgnore] public int Level => Get_X_FromXp(XpTo.Level);

        public Skill(int XP, SkillType type)
        {
            TOTAL_XP = XP;
            Type = type;
        }

        private int Get_X_FromXp(XpTo x)
        {
            int tempXp = TOTAL_XP;
            int tempLevel = 1;
            int tempXpCap = 50;

            while (tempXp >= tempXpCap)
            {
                tempXp -= tempXpCap;
                tempLevel++;
                tempXpCap += (((tempLevel / 10) + 1) * 25);
            }

            switch (x)
            {
                case XpTo.XpCap: return tempXpCap;
                case XpTo.Level: return tempLevel;
                default: return tempXp;
            }
        }
    }
}