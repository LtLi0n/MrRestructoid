using MrRestructoid.Bot.WarsOfTanoth.Entity.Players;
using MrRestructoid.Bot.WarsOfTanoth.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace MrRestructoid.Bot.WarsOfTanoth.System
{
    public class OldPlayer
    {
        public static List<OldPlayer> oldPlayers = new List<OldPlayer>();

        //ALERTS
        public bool alertsOn = true;
        public bool additionalAlertsOn = true;
        public bool phoneMode = false;

        //STATS
        public int level = 1;
        public long xp = 0;
        public int xpCap = 100;
        public int hp = 10;
        public int maxHp = 10;
        public long gold = 0;

        //WORK
        public short workProgress = 0;
        public short workToMake = 0;
        public bool isWorking = false;
        public int totalWork = 0;

        //OTHER
        public string playerName;
        public OldTitle title;
        public List<OldTitle> titles;
        public ulong discordID;
        public ulong serverID;

        //QUESTS
        public bool isOnTrip = false;
        public int tripID = 0;
        public int tripsCompleted = 0;
        public int monstersKilled = 0;
        public short tripsUnlocked = 1;
        public short minutesInTrip = 0;
        public short tripCooldown = 5;

        //HP
        public short hpRegenCounter = 0;
        public short deathCooldown = 0;
        public bool dead = false;

        //PVP
        public short pvpCooldown = 0;
        public short reasonlessKills = 0;
        public int reasonlessKillTimer = 0;
        public short bountyKills = 0;
        public int bountyTitleTimer = 0;

        public OldInventory inventory;

        public Sword sword;
        public Helmet helmet;
        public Body body;
        public Leggings leggings;
        public Boots boots;
        public Gloves gloves;
        public Pet pet;

        public bool betaActive = false;
        //MONSTER KILLS

        public OldPlayer()
        {

        }

        public long GetTotalXP()
        {
            int tempLevel = 1;
            long tempXp = xp;
            long tempXpCap = 100;

            while (tempLevel < level)
            {
                tempXp += tempXpCap;
                tempLevel++;

                double multiplier = tempLevel / 1.3;

                if (tempLevel < 20) tempXpCap += tempLevel * 250;
                else tempXpCap += (int)Math.Round((tempLevel * tempLevel) * multiplier);
            }

            return tempXp;
        }
    }

    public class Body : Gear { public int protection { get; set; } }
    public class Boots : Gear { public int protection { get; set; } }
    public class Gloves : Gear { public int protection { get; set; } }
    public class Helmet : Gear { public int protection { get; set; } }
    public class Leggings : Gear { public int protection { get; set; } }
    public class Sword : Gear { public int damage { get; set; } }

    public class Pet
    {
        public int level;
        public int xp;

        public int ActionsMade
        {
            get
            {
                int tempXp = xp;
                int tempLevel = 1;
                int tempXpCap = 100;

                int actions = 0;

                while (tempLevel < level)
                {
                    tempXp += tempXpCap;
                    actions += tempXpCap / (tempLevel * 10);

                    tempLevel++;
                    tempXpCap += tempLevel * 100;
                }

                if(xp != 0)
                {
                    actions += (tempXp / (tempLevel * 10));
                }
                
                return actions;
            }
        }

        public Pet(int level, int xp)
        {
            this.level = level;
            this.xp = xp;
        }
    }

    public class OldTitle
    {
        public string name { get; set; }
        public string reason { get; set; }
        public TitleType titleType { get; set; }

        public OldTitle(string name, string reason, TitleType titleType)
        {
            this.name = name;
            this.reason = reason;
            this.titleType = titleType;
        }
    }

    public class OldInventory
    {
        public List<InventoryItem> inventory = new List<InventoryItem>();

        public class InventoryItem
        {
            public Item item;
            public int amount;
        }
    }

    public abstract class Gear
    {
        public long TotalXP
        {
            get
            {
                int tempLevel = 0;
                long tempXP = xp;
                int tempXpCap = 1000;

                while (tempLevel < enchantedLevel)
                {
                    tempLevel++;

                    tempXP += tempXpCap;

                    tempXpCap += tempLevel * 1000;
                }

                return tempXP;
            }
        }

        //silverlight
        //
        public string name { get; set; }
        public int ID { get; set; }

        public int level { get; set; }

        public int enchantedLevel { get; set; }
        public long xp { get; set; }
        public int xpCap { get; set; }

        public string enchantedName { get; set; }

        public Gear(int price, int level, string name, int ID)
        {
            this.level = level;
            this.name = name;
            this.ID = ID;
            enchantedLevel = 1;
            xp = 0;
            xpCap = 1000;
            enchantedName = "";
        }

        public Gear(Gear originalSword)
        {
            level = originalSword.level;
            name = originalSword.name;
            ID = originalSword.ID;
            enchantedLevel = 1;
            xp = 0;
            xpCap = 1000;
            enchantedName = "";
        }

        public Gear() { }
    }
}
