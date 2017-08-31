using MrRestructoid.Bot.WarsOfTanoth.Adventure;
using MrRestructoid.Bot.WarsOfTanoth.Items;
using MrRestructoid.Bot.WarsOfTanoth.Items.Gear;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MrRestructoid.Bot.WarsOfTanoth.Entity.Players
{
    public class Player
    {
        public const int LevelCap = 80;

        public CharacterInfo Character { get; set; }
        public UserInfo User { get; set; }

        public Player()
        {
            User = new UserInfo();
            Character = new CharacterInfo(this);
            Character.Player = this;
        }

        public Task SortTitles()
        {
            if (Character.Titles == null) return Task.CompletedTask;
            if (Character.Titles.Count < 2) return Task.CompletedTask;

            int index = 0;

            while (true)
            {
                for (int i = 0; i < Character.Titles.Count - index; i++)
                {
                    if (i < Character.Titles.Count - index - 1)
                    {
                        if (Character.Titles[i].titleType > Character.Titles[i + 1].titleType)
                        {
                            ITitle tempTitle = Character.Titles[i + 1];
                            Character.Titles[i + 1] = Character.Titles[i];
                            Character.Titles[i] = tempTitle;
                        }
                    }
                }
                index++;

                if (index == Character.Titles.Count) return Task.CompletedTask;
            }
        }

        public async Task Update(Handler handler)
        {
            await ITitle.TitleRewarder(this, handler);

            if (Character.Stats.Work.isWorking)
            {
                Character.Stats.Work.workProgress++;
            }
            if (Character.Stats.Work.isWorking && Character.Stats.Work.workProgress == Character.Stats.Work.workToMake * 60)
            {
                int multiplier = 2;

                multiplier += (Character.Level / 15) * 2;

                int goldGained = Character.Level * multiplier * Character.Stats.Work.workToMake;
                if (Character.Titles.Exists(x => x.name == ITitle.Titles.Supporter.name)) goldGained += goldGained / 5;
                Character.Gold += goldGained;
                Character.Stats.Work.isWorking = false;
                Character.Stats.Work.totalWork += Character.Stats.Work.workToMake;
                await SendMessage.Send(this, SendType.Work, reward: goldGained);
            }
            if (!Character.Stats.Quests.isInQuest && Character.Stats.Quests.questCooldown < 5)
            {
                Character.Stats.Quests.questCooldown++;
                if (Character.Stats.Quests.questCooldown == 5 && User.additionalAlertsOn)
                {
                    await SendMessage.Send(this, SendType.QuestCooldown);
                }
            }

            int player_damage = Character.Gear.Sword.Damage + 1 + Character.Level;
            Random random = new Random();
            Quest quest = Quest.getQuest(Program.handler, Character.Stats.Quests.questID);

            if (!Character.Stats.Quests.isInQuest && !Character.Stats.HP.dead)
            {
                Character.Stats.HP.hpRegenCounter++;
            }
            else
            {
                Character.Stats.HP.hpRegenCounter = 0;
            }

            //--------HP REGENERATION
            if (Character.Stats.HP.hpRegenCounter == 5)
            {
                if (!Character.Stats.Quests.isInQuest && !Character.Stats.HP.dead)
                {
                    if (Character.HP < Character.MAX_HP)
                    {
                        if (Character.HP + Character.Level > Character.MAX_HP)
                        {
                            Character.HP = Character.MAX_HP;
                        }
                        else
                        {
                            Character.HP += Character.Level;
                        }
                    }
                }

                Character.Stats.HP.hpRegenCounter = 0;
            }

            if (Character.HP > Character.MAX_HP) Character.HP = Character.MAX_HP;
            //QUEST_COMPLETED-check
            if (Character.Stats.Quests.isInQuest)
            {
                if (Character.Stats.Quests.minutesInQuest == quest.Duration)
                {
                    Character.Stats.Quests.minutesInQuest = 0;
                    Character.Stats.Quests.questCooldown = 0;
                    Character.Stats.Quests.isInQuest = false;
                    await Quest.OnCompleted(this, quest, handler);
                }
                Character.Stats.Quests.minutesInQuest++;
            }

            if (Character.HP < 0) Character.HP = 0;

            int totalProtection = Character.Protection;
            int totalProtectionMax = totalProtection;

            if (Character.Stats.HP.dead && Character.Stats.HP.deathCooldown < 60) Character.Stats.HP.deathCooldown++;
            if (Character.Stats.HP.dead && Character.Stats.HP.deathCooldown == 60)
            {
                Character.Stats.HP.dead = false;
                Character.Stats.HP.deathCooldown = 0;
                Character.Stats.HP.hpRegenCounter = 0;
                Character.HP = Character.MAX_HP / 10;
            }
            if (Character.Stats.PvP.pvpCooldown != 0) Character.Stats.PvP.pvpCooldown--;

            if (Character.Stats.PvP.bountyTitleTimer > 0) Character.Stats.PvP.bountyTitleTimer--;
            if (Character.Stats.PvP.reasonlessKillTimer > 0) Character.Stats.PvP.reasonlessKillTimer--;
        }
    }

    public class UserInfo
    {
        public ulong UserID { get; set; }
        public ulong ServerID { get; set; }
        public bool alertsOn = true;
        public bool additionalAlertsOn = true;
        public bool phoneMode = false;
    }

    public class CharacterInfo
    {
        private enum XpTo { XpCap, Level, LevelXp }

        [JsonIgnore] public Player Player { get; set; }
        public Gear Gear { get; set; }
        public Inventory Inventory { get; set; }
        public Skills Skills { get; set; }

        //STATS
        /// <summary> Date of the Last name change. </summary>
        public DateTime NameChangedAt { get; set; }
        /// <summary> The displayed player's name. </summary>
        public string Name { get; set; }
        /// <summary> Health of player's character. If it reaches 0, it dies. </summary>
        public int HP { set; get; }
        /// <summary> Total amount of in-game currency the Player has. </summary>
        public long Gold { set; get; }

        /// <summary> Total amount of Xp player has. </summary>
        public long TOTAL_XP { get; set; }
        /// <summary> Xp in that specific level area. </summary>
        public long XP => Get_X_FromXp(XpTo.LevelXp);
        /// <summary> Level xp needed to level up. </summary>
        [JsonIgnore] public long XP_CAP => Get_X_FromXp(XpTo.XpCap);
        /// <summary> Player's level. Essential for progression. </summary>
        [JsonIgnore] public short Level => (short)Get_X_FromXp(XpTo.Level);
        [JsonIgnore] public int MAX_HP { get { return Level * 10; } }

        /// <summary> Total amount of protection player has. </summary>
        [JsonIgnore]
        public int Protection => (
                (Gear.Helmet.Protection + Gear.Helmet.EnchantedLevel) +
                (Gear.Chestplate.Protection + Gear.Chestplate.EnchantedLevel) +
                (Gear.Leggings.Protection + Gear.Leggings.EnchantedLevel) +
                (Gear.Boots.Protection + Gear.Boots.EnchantedLevel) +
                (Gear.Gloves.Protection + Gear.Gloves.EnchantedLevel)
            );

        public ITitle ActiveTitle { get; set; }
        public List<ITitle> Titles { get; set; }
        public Statistics Stats { get; set; }

        private long Get_X_FromXp(XpTo x)
        {
            long tempXp = TOTAL_XP;
            long tempLevel = 1;
            long tempXpCap = 100;

            while (tempXp >= tempXpCap && tempLevel < Player.LevelCap)
            {
                tempLevel++;
                tempXp -= tempXpCap;

                double multiplier = tempLevel / 1.3;
                if (tempLevel < 20) tempXpCap += tempLevel * 250;
                else tempXpCap += (int)Math.Round((tempLevel * tempLevel) * multiplier);
            }

            switch (x)
            {
                case XpTo.XpCap: return tempXpCap;
                case XpTo.Level: return tempLevel;
                default: return tempXp;
            }
        }

        [JsonConstructor]
        public CharacterInfo(Player player)
        {
            Player = player;

            TOTAL_XP = 0;
            HP = MAX_HP;

            ActiveTitle = null;
            Titles = new List<ITitle>();
            Stats = new Statistics();

            Gear = new Gear();
            Inventory = new Inventory(Player);
            Skills = new Skills();
        }

        //HP
        public class Statistics
        {
            public HealthInfo HP { get; set; }
            public PVPInfo PvP { get; set; }
            public QuestInfo Quests { get; set; }
            public WorkInfo Work { get; set; }

            public Statistics()
            {
                HP = new HealthInfo();
                PvP = new PVPInfo();
                Quests = new QuestInfo();
                Work = new WorkInfo();
            }

            public class HealthInfo
            {
                public short hpRegenCounter = 0;
                public short deathCooldown = 0;
                public bool dead = false;
            }

            public class PVPInfo
            {
                /// <summary> Time in minutes player won't be able to engage in combat with other players. </summary>
                public short pvpCooldown = 0;
                /// <summary> Total amount of kills player has obtained without bounty. </summary>
                public short reasonlessKills = 0;
                /// <summary> Bounty timer on player in minutes </summary>
                public int reasonlessKillTimer = 0;
                /// <summary> Total amount of bounty kills player has obtained. </summary>
                public short bountyKills = 0;
                /// <summary> Amount of time Player will have The title in minutes. </summary>
                public int bountyTitleTimer = 0;
            }

            public class QuestInfo
            {
                /// <summary> Defines if player is currently in a quest. </summary>
                public bool isInQuest = false;
                /// <summary> If player is in quest, quest data will be pulled from this ID. </summary>
                public int questID = 0;
                /// <summary> Total amount of quests p[layer has completed successfully. </summary>
                public int questsCompleted = 0;
                /// <summary> Monsters killed throughout all quests. </summary>
                public int monstersKilled = 0;
                /// <summary> Amount of quests unlocked. </summary>
                public short questsUnlocked = 1;
                /// <summary> Minutes in selected quest. </summary>
                public short minutesInQuest = 0;
                /// <summary> Quest cooldown in Minutes where cooldown = 5 - questCooldown. </summary>
                public short questCooldown = 5;
            }

            public class WorkInfo
            {
                /// <summary> Work progress(Minutes) towards WorkToMake(Hours). </summary>
                public short workProgress = 0;
                /// <summary> Player work hours goal. </summary>
                public short workToMake = 0;
                /// <summary> Player Working state. </summary>
                public bool isWorking = false;
                /// <summary> Total amount of successful hours player has spent on working. </summary>
                public int totalWork = 0;
            }
        }
    }
}
